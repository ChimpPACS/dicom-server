// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnsureThat;
using FellowOakDicom;
using Microsoft.Health.Dicom.Core.Exceptions;
using Microsoft.Health.Dicom.Core.Features.ChangeFeed;
using Microsoft.Health.Dicom.Core.Features.ExtendedQueryTag;
using Microsoft.Health.Dicom.Core.Features.Model;
using Microsoft.Health.Dicom.Core.Features.Partition;
using Microsoft.Health.Dicom.Core.Features.Query;
using Microsoft.Health.Dicom.Core.Features.Retrieve;
using Microsoft.Health.Dicom.Core.Features.Store;
using Microsoft.Health.Dicom.Core.Models;
using Microsoft.Health.Dicom.SqlServer.Features.ExtendedQueryTag;
using Microsoft.Health.Dicom.Tests.Common;
using Microsoft.Health.Dicom.Tests.Common.Extensions;
using Microsoft.Health.Dicom.Tests.Integration.Persistence.Models;
using Xunit;

namespace Microsoft.Health.Dicom.Tests.Integration.Persistence;

/// <summary>
///  Tests for InstanceStore.
/// </summary>
public partial class InstanceStoreTests : IClassFixture<SqlDataStoreTestsFixture>
{
    private readonly IInstanceStore _instanceStore;
    private readonly IIndexDataStore _indexDataStore;
    private readonly IExtendedQueryTagStore _extendedQueryTagStore;
    private readonly IIndexDataStoreTestHelper _indexDataStoreTestHelper;
    private readonly IExtendedQueryTagStoreTestHelper _extendedQueryTagStoreTestHelper;
    private readonly IPartitionStore _partitionStore;
    private readonly IQueryStore _queryStore;
    private readonly IChangeFeedStore _changeFeedStore;
    private readonly SqlDataStoreTestsFixture _fixture;

    public InstanceStoreTests(SqlDataStoreTestsFixture fixture)
    {
        _fixture = EnsureArg.IsNotNull(fixture, nameof(fixture));
        _instanceStore = EnsureArg.IsNotNull(fixture?.InstanceStore, nameof(fixture.InstanceStore));
        _indexDataStore = EnsureArg.IsNotNull(fixture?.IndexDataStore, nameof(fixture.IndexDataStore));
        _extendedQueryTagStore = EnsureArg.IsNotNull(fixture?.ExtendedQueryTagStore, nameof(fixture.ExtendedQueryTagStore));
        _indexDataStoreTestHelper = EnsureArg.IsNotNull(fixture?.IndexDataStoreTestHelper, nameof(fixture.IndexDataStoreTestHelper));
        _extendedQueryTagStoreTestHelper = EnsureArg.IsNotNull(fixture?.ExtendedQueryTagStoreTestHelper, nameof(fixture.ExtendedQueryTagStoreTestHelper));
        _partitionStore = EnsureArg.IsNotNull(fixture?.PartitionStore, nameof(fixture.PartitionStore));
        _queryStore = EnsureArg.IsNotNull(fixture?.QueryStore, nameof(fixture.QueryStore));
        _changeFeedStore = EnsureArg.IsNotNull(fixture?.ChangeFeedStore, nameof(fixture.ChangeFeedStore));
    }

    [Fact]
    public async Task GivenInstances_WhenGetInstanceIdentifiersByWatermarkRange_ThenItShouldReturnInstancesInRange()
    {
        await AddRandomInstanceAsync();
        var instance1 = await AddRandomInstanceAsync();
        var instance2 = await AddRandomInstanceAsync();
        var instance3 = await AddRandomInstanceAsync();
        var instance4 = await AddRandomInstanceAsync();
        await AddRandomInstanceAsync();

        IReadOnlyList<VersionedInstanceIdentifier> instances = await _instanceStore.GetInstanceIdentifiersByWatermarkRangeAsync(
            new WatermarkRange(instance1.Version, instance4.Version),
            IndexStatus.Creating);

        Assert.Equal(instances, new[] { instance1, instance2, instance3, instance4 });
    }

    [Fact]
    public async Task GivenStudyTag_WhenReindexWithNewInstance_ThenTagValueShouldBeUpdated()
    {
        DicomTag tag = DicomTag.DeviceSerialNumber;
        string tagValue1 = "test1";
        string tagValue2 = "test2";
        string tagValue3 = "test3";

        string studyUid = TestUidGenerator.Generate();

        DicomDataset dataset1 = Samples.CreateRandomInstanceDataset(studyUid);
        dataset1.Add(tag, tagValue1);
        DicomDataset dataset2 = Samples.CreateRandomInstanceDataset(studyUid);
        dataset2.Add(tag, tagValue2);
        DicomDataset dataset3 = Samples.CreateRandomInstanceDataset(studyUid);
        dataset3.Add(tag, tagValue3);

        Instance instance1 = await CreateInstanceIndexAsync(dataset1);
        Instance instance2 = await CreateInstanceIndexAsync(dataset2);
        Instance instance3 = await CreateInstanceIndexAsync(dataset3);

        var tagStoreEntry = await AddExtendedQueryTagAsync(tag.BuildAddExtendedQueryTagEntry(level: QueryTagLevel.Study));
        QueryTag queryTag = new QueryTag(tagStoreEntry);

        // Simulate re-indexing, which may re-index an instance which may re-index
        // the instances for a particular study or series out-of-order
        await _indexDataStore.ReindexInstanceAsync(dataset2, instance2.Watermark, new[] { queryTag });
        ExtendedQueryTagDataRow row = (await _extendedQueryTagStoreTestHelper.GetExtendedQueryTagDataAsync(ExtendedQueryTagDataType.StringData, tagStoreEntry.Key, instance1.StudyKey, null, null)).Single();
        Assert.Equal(tagValue2, row.TagValue); // Added

        await _indexDataStore.ReindexInstanceAsync(dataset3, instance3.Watermark, new[] { queryTag });
        row = (await _extendedQueryTagStoreTestHelper.GetExtendedQueryTagDataAsync(ExtendedQueryTagDataType.StringData, tagStoreEntry.Key, instance1.StudyKey, null, null)).Single();
        Assert.Equal(tagValue3, row.TagValue); // Overwrite

        await _indexDataStore.ReindexInstanceAsync(dataset1, instance1.Watermark, new[] { queryTag });
        row = (await _extendedQueryTagStoreTestHelper.GetExtendedQueryTagDataAsync(ExtendedQueryTagDataType.StringData, tagStoreEntry.Key, instance1.StudyKey, null, null)).Single();
        Assert.Equal(tagValue3, row.TagValue); // Do not overwrite
    }

    [Fact]
    public async Task GivenSeriesTag_WhenReindexWithOldInstance_ThenTagValueShouldNotBeUpdated()
    {
        DicomTag tag = DicomTag.AcquisitionDeviceProcessingCode;
        string tagValue1 = "test1";
        string tagValue2 = "test2";

        string studyUid = TestUidGenerator.Generate();
        string seriesUid = TestUidGenerator.Generate();

        DicomDataset dataset1 = Samples.CreateRandomInstanceDataset(studyUid, seriesUid);
        dataset1.Add(tag, tagValue1);
        DicomDataset dataset2 = Samples.CreateRandomInstanceDataset(studyUid, seriesUid);
        dataset2.Add(tag, tagValue2);
        Instance instance1 = await CreateInstanceIndexAsync(dataset1);
        Instance instance2 = await CreateInstanceIndexAsync(dataset2);

        var tagStoreEntry = await AddExtendedQueryTagAsync(tag.BuildAddExtendedQueryTagEntry(level: QueryTagLevel.Series));
        QueryTag queryTag = new QueryTag(tagStoreEntry);

        await _indexDataStore.ReindexInstanceAsync(dataset2, instance2.Watermark, new[] { queryTag });
        await _indexDataStore.ReindexInstanceAsync(dataset1, instance1.Watermark, new[] { queryTag });

        var row = (await _extendedQueryTagStoreTestHelper.GetExtendedQueryTagDataAsync(ExtendedQueryTagDataType.StringData, tagStoreEntry.Key, instance1.StudyKey, instance1.SeriesKey, null)).First();
        Assert.Equal(tagValue2, row.TagValue);
    }

    [Fact]
    public async Task GivenInstanceTag_WhenReindexWithNotIndexedInstance_ThenTagValueShouldBeInserted()
    {
        DicomTag tag = DicomTag.AcquisitionDeviceProcessingDescription;
        string tagValue = "test";

        DicomDataset dataset = Samples.CreateRandomInstanceDataset();
        dataset.Add(tag, tagValue);

        Instance instance = await CreateInstanceIndexAsync(dataset);

        var tagStoreEntry = await AddExtendedQueryTagAsync(tag.BuildAddExtendedQueryTagEntry(level: QueryTagLevel.Instance));

        await _indexDataStore.ReindexInstanceAsync(dataset, instance.Watermark, new[] { new QueryTag(tagStoreEntry) });

        var row = (await _extendedQueryTagStoreTestHelper.GetExtendedQueryTagDataAsync(ExtendedQueryTagDataType.StringData, tagStoreEntry.Key, instance.StudyKey, instance.SeriesKey, instance.InstanceKey)).First();
        Assert.Equal(tagValue, row.TagValue);
    }

    [Fact]
    public async Task GivenInstanceTag_WhenReindexWithIndexedInstance_ThenTagValueShouldNotBeUpdated()
    {
        DicomTag tag = DicomTag.DeviceLabel;
        string tagValue = "test";
        var tagStoreEntry = await AddExtendedQueryTagAsync(tag.BuildAddExtendedQueryTagEntry(level: QueryTagLevel.Instance));

        DicomDataset dataset = Samples.CreateRandomInstanceDataset();
        dataset.Add(tag, tagValue);
        var instance = await CreateInstanceIndexAsync(dataset);

        await _indexDataStore.ReindexInstanceAsync(dataset, instance.Watermark, new[] { new QueryTag(tagStoreEntry) });

        var row = (await _extendedQueryTagStoreTestHelper.GetExtendedQueryTagDataAsync(ExtendedQueryTagDataType.StringData, tagStoreEntry.Key, instance.StudyKey, instance.SeriesKey, instance.InstanceKey)).First();
        Assert.Equal(tagValue, row.TagValue);

    }

    [Fact]
    public async Task GivenInstanceNotExist_WhenReindex_ThenShouldThrowException()
    {
        DicomTag tag = DicomTag.DeviceID;
        var tagStoreEntry = await AddExtendedQueryTagAsync(tag.BuildAddExtendedQueryTagEntry(level: QueryTagLevel.Instance));

        DicomDataset dataset = Samples.CreateRandomInstanceDataset();
        await Assert.ThrowsAsync<InstanceNotFoundException>(() => _indexDataStore.ReindexInstanceAsync(dataset, 0, new[] { new QueryTag(tagStoreEntry) }));
    }

    [Fact]
    public async Task GivenPendingInstance_WhenReindex_ThenShouldThrowException()
    {
        DicomTag tag = DicomTag.DeviceDescription;
        var tagStoreEntry = await AddExtendedQueryTagAsync(tag.BuildAddExtendedQueryTagEntry(level: QueryTagLevel.Instance));

        DicomDataset dataset = Samples.CreateRandomInstanceDataset();

        long watermark = await _indexDataStore.BeginCreateInstanceIndexAsync(1, dataset);
        await Assert.ThrowsAsync<PendingInstanceException>(() => _indexDataStore.ReindexInstanceAsync(dataset, watermark, new[] { new QueryTag(tagStoreEntry) }));
    }

    [Fact]
    public async Task GivenInstances_WhenGettingInstanceBatches_ThenStartAtEnd()
    {
        var instances = new List<VersionedInstanceIdentifier>
        {
            await AddRandomInstanceAsync(),
            await AddRandomInstanceAsync(),
            await AddRandomInstanceAsync(),
            await AddRandomInstanceAsync(),
            await AddRandomInstanceAsync(),
            await AddRandomInstanceAsync(), // Deleted
            await AddRandomInstanceAsync(),
            await AddRandomInstanceAsync(),
        };

        // Create a gap within the data
        await _indexDataStore.DeleteInstanceIndexAsync(
            new InstanceIdentifier(
                instances[^3].StudyInstanceUid,
                instances[^3].SeriesInstanceUid,
                instances[^3].SopInstanceUid,
                DefaultPartition.Key));

        IReadOnlyList<WatermarkRange> batches;

        // No Max Watermark
        batches = await _instanceStore.GetInstanceBatchesAsync(3, 2, IndexStatus.Creating);

        Assert.Equal(2, batches.Count);
        Assert.Equal(new WatermarkRange(instances[^4].Version, instances[^1].Version), batches[0]);
        Assert.Equal(new WatermarkRange(instances[^7].Version, instances[^5].Version), batches[1]);

        // With Max Watermark
        batches = await _instanceStore.GetInstanceBatchesAsync(3, 2, IndexStatus.Creating, instances[^2].Version);

        Assert.Equal(2, batches.Count);
        Assert.Equal(new WatermarkRange(instances[^5].Version, instances[^2].Version), batches[0]);
        Assert.Equal(new WatermarkRange(instances[^8].Version, instances[^6].Version), batches[1]);
    }

    [Fact]
    public async Task WhenAddingTheSameInstanceToTwoPartitions_ThenTheyAreRetrievedCorrectly()
    {
        var partition1 = "partition1";
        var partition2 = "partition2";

        var partitionEntry1 = await _partitionStore.AddPartitionAsync(partition1);
        var partitionEntry2 = await _partitionStore.AddPartitionAsync(partition2);

        string studyInstanceUID = TestUidGenerator.Generate();

        DicomDataset dataset1 = Samples.CreateRandomInstanceDataset(studyInstanceUID);
        DicomDataset dataset2 = Samples.CreateRandomInstanceDataset(studyInstanceUID);

        Instance instance1 = await CreateInstanceIndexAsync(dataset1, partitionEntry1.PartitionKey);
        Instance instance2 = await CreateInstanceIndexAsync(dataset2, partitionEntry2.PartitionKey);

        Assert.Equal(partitionEntry1.PartitionKey, instance1.PartitionKey);
        Assert.Equal(partitionEntry2.PartitionKey, instance2.PartitionKey);
    }

    [Fact]
    public async Task WhenRetrievingAnInstanceFromTheWrongPartition_ThenResultSetIsEmpty()
    {
        var partition = "partition3";
        var partitionEntry = await _partitionStore.AddPartitionAsync(partition);

        var identifier = await AddRandomInstanceAsync(partitionEntry.PartitionKey);

        var instances = await _indexDataStoreTestHelper.GetInstancesAsync(identifier.StudyInstanceUid, identifier.SeriesInstanceUid, identifier.SopInstanceUid, DefaultPartition.Key);
        Assert.Empty(instances);
    }

    [Fact]
    public async Task GivenInstances_WhenBulkUpdateInstancesInAStudy_ThenItShouldBeSuccessful()
    {
        var studyInstanceUID1 = TestUidGenerator.Generate();
        DicomDataset dataset1 = Samples.CreateRandomInstanceDataset(studyInstanceUID1);
        DicomDataset dataset2 = Samples.CreateRandomInstanceDataset(studyInstanceUID1);
        DicomDataset dataset3 = Samples.CreateRandomInstanceDataset(studyInstanceUID1);
        DicomDataset dataset4 = Samples.CreateRandomInstanceDataset(studyInstanceUID1);
        dataset4.AddOrUpdate(DicomTag.PatientName, "FirstName_LastName");

        var instance1 = await CreateInstanceIndexAsync(dataset1);
        var instance2 = await CreateInstanceIndexAsync(dataset2);
        var instance3 = await CreateInstanceIndexAsync(dataset3);
        var instance4 = await CreateInstanceIndexAsync(dataset4);

        var instances = new List<Instance> { instance1, instance2, instance3, instance4 };

        // Update the instances with newWatermark
        await _indexDataStore.BeginUpdateInstancesAsync(instance1.PartitionKey, studyInstanceUID1);

        var dicomDataset = new DicomDataset();
        dicomDataset.AddOrUpdate(DicomTag.PatientName, "FirstName_NewLastName");

        await _indexDataStore.EndUpdateInstanceAsync(DefaultPartition.Key, studyInstanceUID1, dicomDataset);

        var instanceMetadata = (await _instanceStore.GetInstanceIdentifierWithPropertiesAsync(DefaultPartition.PartitionEntry, studyInstanceUID1)).ToList();

        // Verify the instances are updated with updated information
        Assert.Equal(instances.Count, instanceMetadata.Count());

        for (int i = 0; i < instances.Count; i++)
        {
            Assert.Equal(instances[i].SopInstanceUid, instanceMetadata[i].VersionedInstanceIdentifier.SopInstanceUid);
            Assert.Equal(instances[i].Watermark, instanceMetadata[i].InstanceProperties.OriginalVersion);
            Assert.False(instanceMetadata[i].InstanceProperties.NewVersion.HasValue);
        }

        // Verify if the new patient name is updated
        var result = await _queryStore.GetStudyResultAsync(DefaultPartition.Key, new long[] { instanceMetadata.First().VersionedInstanceIdentifier.Version });

        Assert.True(result.Any());
        Assert.Equal("FirstName_NewLastName", result.First().PatientName);

        // Verify Changefeed entries are inserted
        var changeFeedEntries = await _fixture.IndexDataStoreTestHelper.GetUpdatedChangeFeedRowsAsync(4);
        Assert.True(changeFeedEntries.Any());
        for (int i = 0; i < instances.Count; i++)
        {
            Assert.Equal(instanceMetadata[i].VersionedInstanceIdentifier.Version, changeFeedEntries[i].OriginalWatermark);
            Assert.Equal(instanceMetadata[i].VersionedInstanceIdentifier.Version, changeFeedEntries[i].CurrentWatermark);
            Assert.Equal(instanceMetadata[i].VersionedInstanceIdentifier.SopInstanceUid, changeFeedEntries[i].SopInstanceUid);
        }
    }

    private async Task<ExtendedQueryTagStoreEntry> AddExtendedQueryTagAsync(AddExtendedQueryTagEntry addExtendedQueryTagEntry)
        => (await _extendedQueryTagStore.AddExtendedQueryTagsAsync(new[] { addExtendedQueryTagEntry }, 128))[0];

    private async Task<Instance> CreateInstanceIndexAsync(DicomDataset dataset, int partitionKey = DefaultPartition.Key)
    {
        string studyUid = dataset.GetString(DicomTag.StudyInstanceUID);
        string seriesUid = dataset.GetString(DicomTag.SeriesInstanceUID);
        string sopInstanceUid = dataset.GetString(DicomTag.SOPInstanceUID);
        long watermark = await _indexDataStore.BeginCreateInstanceIndexAsync(partitionKey, dataset);
        await _indexDataStore.EndCreateInstanceIndexAsync(partitionKey, dataset, watermark);

        return await _indexDataStoreTestHelper.GetInstanceAsync(studyUid, seriesUid, sopInstanceUid, watermark);
    }

    private async Task<VersionedInstanceIdentifier> AddRandomInstanceAsync(int partitionKey = DefaultPartition.Key)
    {
        DicomDataset dataset = Samples.CreateRandomInstanceDataset();

        string studyInstanceUid = dataset.GetString(DicomTag.StudyInstanceUID);
        string seriesInstanceUid = dataset.GetString(DicomTag.SeriesInstanceUID);
        string sopInstanceUid = dataset.GetString(DicomTag.SOPInstanceUID);

        long version = await _indexDataStore.BeginCreateInstanceIndexAsync(partitionKey, dataset);
        return new VersionedInstanceIdentifier(studyInstanceUid, seriesInstanceUid, sopInstanceUid, version, partitionKey);
    }
}
