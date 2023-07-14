// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using EnsureThat;
using FellowOakDicom;
using Microsoft.Health.Dicom.Client;
using Microsoft.Health.Dicom.Core.Extensions;
using Microsoft.Health.Dicom.Core.Messages;
using Microsoft.Health.Dicom.Tests.Common;
using Microsoft.Health.Dicom.Tests.Common.Serialization;
using Microsoft.Health.Dicom.Web.Tests.E2E.Common;
using Xunit;

namespace Microsoft.Health.Dicom.Web.Tests.E2E.Rest;

public class DicomRetrieveMetadataTransactionTests : IClassFixture<HttpIntegrationTestFixture<Startup>>, IAsyncLifetime
{
    private readonly IDicomWebClient _client;
    private readonly DicomInstancesManager _instancesManager;
    private readonly string _apiVersion;

    public DicomRetrieveMetadataTransactionTests(HttpIntegrationTestFixture<Startup> fixture)
    {
        EnsureArg.IsNotNull(fixture, nameof(fixture));
        _client = fixture.GetDicomWebClient();
        _instancesManager = new DicomInstancesManager(_client);
        _apiVersion = DicomApiVersions.V2;
    }

    [Theory]
    [InlineData("application/data")]
    public async Task GivenAnIncorrectAcceptHeader_WhenRetrievingResource_ThenNotAcceptableIsReturned(string acceptHeader)
    {
        // Study
        await _client.ValidateResponseStatusCodeAsync(
            GenerateRequestUri(string.Format(CultureInfo.InvariantCulture, DicomWebConstants.BaseRetrieveStudyMetadataUriFormat, Guid.NewGuid())),
            acceptHeader,
            HttpStatusCode.NotAcceptable);

        // Series
        await _client.ValidateResponseStatusCodeAsync(
            GenerateRequestUri(string.Format(CultureInfo.InvariantCulture, DicomWebConstants.BaseRetrieveSeriesMetadataUriFormat, Guid.NewGuid(), Guid.NewGuid())),
            acceptHeader,
            HttpStatusCode.NotAcceptable);

        // Instance
        await _client.ValidateResponseStatusCodeAsync(
            GenerateRequestUri(string.Format(CultureInfo.InvariantCulture, DicomWebConstants.BaseRetrieveInstanceMetadataUriFormat, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid())),
            acceptHeader,
            HttpStatusCode.NotAcceptable);
    }

    private Uri GenerateRequestUri(string relativePath)
    {
        return new Uri($"/{_apiVersion}{relativePath}", UriKind.Relative);
    }

    [Fact]
    public async Task GivenRetrieveStudyMetadataRequest_WhenStudyInstanceUidDoesnotExists_ThenNotFoundIsReturned()
    {
        DicomWebException exception = await Assert.ThrowsAsync<DicomWebException>(() => _client.RetrieveStudyMetadataAsync(TestUidGenerator.Generate()));
        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
    }

    [Fact]
    public async Task GivenRetrieveSeriesMetadataRequest_WhenSeriesInstanceUidDoesnotExists_ThenNotFoundIsReturned()
    {
        string studyInstanceUid = TestUidGenerator.Generate();
        await PostDicomFileAsync(ResourceType.Series, studyInstanceUid, TestUidGenerator.Generate());

        DicomWebException exception = await Assert.ThrowsAsync<DicomWebException>(() => _client.RetrieveSeriesMetadataAsync(studyInstanceUid, TestUidGenerator.Generate()));
        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
    }

    [Fact]
    public async Task GivenRetrieveSopInstanceMetadataRequest_WhenSopInstanceUidDoesnotExists_ThenNotFoundIsReturned()
    {
        string studyInstanceUid = TestUidGenerator.Generate();
        string seriesInstanceUid = TestUidGenerator.Generate();

        await PostDicomFileAsync(ResourceType.Instance, studyInstanceUid, seriesInstanceUid, TestUidGenerator.Generate());

        DicomWebException exception = await Assert.ThrowsAsync<DicomWebException>(() => _client.RetrieveInstanceMetadataAsync(studyInstanceUid, seriesInstanceUid, TestUidGenerator.Generate()));
        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
    }

    [Fact]
    [Trait("Category", "bvt")]
    public async Task GivenStoredDicomFile_WhenRetrievingMetadataForStudy_ThenMetadataIsRetrievedCorrectly()
    {
        string studyInstanceUid = TestUidGenerator.Generate();

        DicomDataset firstStoredInstance = await PostDicomFileAsync(ResourceType.Study, studyInstanceUid, dataSet: GenerateNewDataSet());
        DicomDataset secondStoredInstance = await PostDicomFileAsync(ResourceType.Study, studyInstanceUid, dataSet: GenerateNewDataSet());

        using DicomWebAsyncEnumerableResponse<DicomDataset> response = await _client.RetrieveStudyMetadataAsync(studyInstanceUid);
        await ValidateResponseMetadataDatasetAsync(response, firstStoredInstance, secondStoredInstance);
    }

    [Fact]
    public async Task GivenStoredDicomFile_WhenRetrievingMetadataForSeries_ThenMetadataIsRetrievedCorrectly()
    {
        string studyInstanceUid = TestUidGenerator.Generate();
        string seriesInstanceUid = TestUidGenerator.Generate();

        DicomDataset firstStoredInstance = await PostDicomFileAsync(ResourceType.Series, studyInstanceUid, seriesInstanceUid, dataSet: GenerateNewDataSet());
        DicomDataset secondStoredInstance = await PostDicomFileAsync(ResourceType.Series, studyInstanceUid, seriesInstanceUid, dataSet: GenerateNewDataSet());

        using DicomWebAsyncEnumerableResponse<DicomDataset> response = await _client.RetrieveSeriesMetadataAsync(studyInstanceUid, seriesInstanceUid);
        await ValidateResponseMetadataDatasetAsync(response, firstStoredInstance, secondStoredInstance);
    }

    [Fact]
    public async Task GivenStoredDicomFile_WhenRetrievingMetadataForInstance_ThenMetadataIsRetrievedCorrectly()
    {
        string studyInstanceUid = TestUidGenerator.Generate();
        string seriesInstanceUid = TestUidGenerator.Generate();
        string sopInstanceUid = TestUidGenerator.Generate();

        DicomDataset storedInstance = await PostDicomFileAsync(ResourceType.Instance, studyInstanceUid, seriesInstanceUid, sopInstanceUid, dataSet: GenerateNewDataSet());

        using DicomWebAsyncEnumerableResponse<DicomDataset> response = await _client.RetrieveInstanceMetadataAsync(studyInstanceUid, seriesInstanceUid, sopInstanceUid);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/dicom+json", response.ContentHeaders.ContentType.MediaType);

        DicomDataset[] datasets = await response.ToArrayAsync();

        Assert.Single(datasets);
        ValidateResponseMetadataDataset(storedInstance, datasets[0]);
    }

    private static DicomDataset GenerateNewDataSet()
    {
        return new DicomDataset
        {
            { DicomTag.SeriesDescription, "A Test Series" },
            { DicomTag.PixelData, new byte[] { 1, 2, 3 } },
            new DicomSequence(
                DicomTag.RegistrationSequence,
                new DicomDataset
                {
                    { DicomTag.PatientName, "Test^Patient" },
                    { DicomTag.PixelData, new byte[] { 1, 2, 3 } },
                }),
            { DicomTag.StudyDate, DateTime.UtcNow },
            { DicomVR.LO, new DicomTag(0007, 0008), "Private Tag" },
        };
    }

    private static async Task ValidateResponseMetadataDatasetAsync(DicomWebAsyncEnumerableResponse<DicomDataset> response, DicomDataset storedInstance1, DicomDataset storedInstance2)
    {
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/dicom+json", response.ContentHeaders.ContentType.MediaType);

        DicomDataset[] datasets = await response.ToArrayAsync();

        Assert.Equal(2, datasets.Length);

        // Trim the stored dataset to the expected items in the response metadata dataset (remove non-supported value representations).
        DicomDataset expectedDataset1 = storedInstance1.CopyWithoutBulkDataItems();
        DicomDataset expectedDataset2 = storedInstance2.CopyWithoutBulkDataItems();

        DicomDataset retrievedDataset1 = datasets[0];
        DicomDataset retrievedDataset2 = datasets[1];

        // Compare result datasets by serializing.
        string serializedExpectedDataset1 = JsonSerializer.Serialize(expectedDataset1, AppSerializerOptions.Json);
        string serializedExpectedDataset2 = JsonSerializer.Serialize(expectedDataset2, AppSerializerOptions.Json);

        string serializedRetrievedDataset1 = JsonSerializer.Serialize(retrievedDataset1, AppSerializerOptions.Json);
        string serializedRetrievedDataset2 = JsonSerializer.Serialize(retrievedDataset2, AppSerializerOptions.Json);

        if (string.Equals(serializedExpectedDataset1, serializedRetrievedDataset1, StringComparison.InvariantCultureIgnoreCase) && string.Equals(serializedExpectedDataset2, serializedRetrievedDataset2, StringComparison.InvariantCultureIgnoreCase))
        {
            Assert.Equal(expectedDataset1.Count(), retrievedDataset1.Count());
            Assert.Equal(expectedDataset2.Count(), retrievedDataset2.Count());
            return;
        }
        else if (string.Equals(serializedExpectedDataset2, serializedRetrievedDataset1, StringComparison.InvariantCultureIgnoreCase) && string.Equals(serializedExpectedDataset1, serializedRetrievedDataset2, StringComparison.InvariantCultureIgnoreCase))
        {
            Assert.Equal(expectedDataset2.Count(), retrievedDataset1.Count());
            Assert.Equal(expectedDataset1.Count(), retrievedDataset2.Count());
            return;
        }

        Assert.Fail("Retrieved dataset does not match the stored dataset");
    }

    private static void ValidateResponseMetadataDataset(DicomDataset storedDataset, DicomDataset retrievedDataset)
    {
        // Trim the stored dataset to the expected items in the response metadata dataset (remove non-supported value representations).
        DicomDataset expectedDataset = storedDataset.CopyWithoutBulkDataItems();

        // Compare result datasets by serializing.
        Assert.Equal(
            JsonSerializer.Serialize(expectedDataset, AppSerializerOptions.Json),
            JsonSerializer.Serialize(retrievedDataset, AppSerializerOptions.Json));
        Assert.Equal(expectedDataset.Count(), retrievedDataset.Count());
    }

    private async Task<DicomDataset> PostDicomFileAsync(ResourceType resourceType, string studyInstanceUid, string seriesInstanceUid = null, string sopInstanceUid = null, DicomDataset dataSet = null)
    {
        DicomFile dicomFile = null;

        switch (resourceType)
        {
            case ResourceType.Study:
                dicomFile = Samples.CreateRandomDicomFile(studyInstanceUid);
                break;
            case ResourceType.Series:
                dicomFile = Samples.CreateRandomDicomFile(studyInstanceUid, seriesInstanceUid);
                break;
            case ResourceType.Instance:
                dicomFile = Samples.CreateRandomDicomFile(studyInstanceUid, seriesInstanceUid, sopInstanceUid);
                break;
        }

        if (dataSet != null)
        {
            dicomFile.Dataset.AddOrUpdate(dataSet);
        }

        using (DicomWebResponse<DicomDataset> response = await _instancesManager.StoreAsync(new[] { dicomFile }))
        {
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        return dicomFile.Dataset;
    }

    public Task InitializeAsync()
       => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        await _instancesManager.DisposeAsync();
    }
}
