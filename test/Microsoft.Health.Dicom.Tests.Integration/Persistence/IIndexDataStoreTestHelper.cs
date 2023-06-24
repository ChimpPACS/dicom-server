// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Health.Dicom.Core.Features.Partition;
using Microsoft.Health.Dicom.Tests.Integration.Persistence.Models;

namespace Microsoft.Health.Dicom.Tests.Integration.Persistence;

public interface IIndexDataStoreTestHelper
{
    Task<IReadOnlyList<StudyMetadata>> GetStudyMetadataAsync(string studyInstanceUid, int partitionKey = DefaultPartition.Key);

    Task<IReadOnlyList<SeriesMetadata>> GetSeriesMetadataAsync(string seriesInstanceUid, int partitionKey = DefaultPartition.Key);

    Task<IReadOnlyList<Instance>> GetInstancesAsync(string studyInstanceUid, string seriesInstanceUid, string sopInstanceUid, int partitionKey = DefaultPartition.Key);

    Task<IReadOnlyList<FileProperty>> GetFilePropertiesAsync(long watermark);

    Task<Instance> GetInstanceAsync(string studyInstanceUid, string seriesInstanceUid, string sopInstanceUid, long version);

    Task<IReadOnlyList<DeletedInstance>> GetDeletedInstanceEntriesAsync(string studyInstanceUid, string seriesInstanceUid, string sopInstanceUid, int partitionKey = DefaultPartition.Key);

    Task<IReadOnlyList<ChangeFeedRow>> GetChangeFeedRowsAsync(string studyInstanceUid, string seriesInstanceUid, string sopInstanceUid);

    Task<IReadOnlyList<ChangeFeedRow>> GetUpdatedChangeFeedRowsAsync(int limit);

    Task ClearDeletedInstanceTableAsync();

    Task ClearIndexTablesAsync();
}
