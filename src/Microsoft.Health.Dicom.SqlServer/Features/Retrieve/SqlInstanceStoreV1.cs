// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.Health.Dicom.Core.Exceptions;
using Microsoft.Health.Dicom.Core.Features.Model;
using Microsoft.Health.Dicom.Core.Features.Partition;
using Microsoft.Health.Dicom.Core.Models;
using Microsoft.Health.Dicom.SqlServer.Features.Schema;
using Microsoft.Health.Dicom.SqlServer.Features.Schema.Model;
using Microsoft.Health.SqlServer.Features.Client;
using Microsoft.Health.SqlServer.Features.Storage;

namespace Microsoft.Health.Dicom.SqlServer.Features.Retrieve;

internal class SqlInstanceStoreV1 : ISqlInstanceStore
{
    public SqlInstanceStoreV1(SqlConnectionWrapperFactory sqlConnectionWrapperFactory)
    {
        EnsureArg.IsNotNull(sqlConnectionWrapperFactory, nameof(sqlConnectionWrapperFactory));

        SqlConnectionWrapperFactory = sqlConnectionWrapperFactory;
    }

    protected SqlConnectionWrapperFactory SqlConnectionWrapperFactory { get; }

    public virtual SchemaVersion Version => SchemaVersion.V1;

    public virtual Task<IReadOnlyList<VersionedInstanceIdentifier>> GetInstanceIdentifierAsync(
        PartitionEntry partitionEntry,
        string studyInstanceUid,
        string seriesInstanceUid,
        string sopInstanceUid,
        CancellationToken cancellationToken)
    {
        return GetInstanceIdentifierImp(partitionEntry, studyInstanceUid, cancellationToken, seriesInstanceUid, sopInstanceUid);
    }

    public virtual Task<IReadOnlyList<VersionedInstanceIdentifier>> GetInstanceIdentifiersByWatermarkRangeAsync(
        WatermarkRange watermarkRange,
        IndexStatus indexStatus,
        CancellationToken cancellationToken = default)
    {
        throw new BadRequestException(DicomSqlServerResource.SchemaVersionNeedsToBeUpgraded);
    }

    public virtual Task<IReadOnlyList<VersionedInstanceIdentifier>> GetInstanceIdentifiersInSeriesAsync(
        PartitionEntry partitionEntry,
        string studyInstanceUid,
        string seriesInstanceUid,
        CancellationToken cancellationToken)
    {
        return GetInstanceIdentifierImp(partitionEntry, studyInstanceUid, cancellationToken, seriesInstanceUid);
    }

    public virtual Task<IReadOnlyList<VersionedInstanceIdentifier>> GetInstanceIdentifiersInStudyAsync(
        PartitionEntry partitionEntry,
        string studyInstanceUid,
        CancellationToken cancellationToken)
    {
        return GetInstanceIdentifierImp(partitionEntry, studyInstanceUid, cancellationToken);
    }

    public virtual Task<IReadOnlyList<WatermarkRange>> GetInstanceBatchesAsync(
        int batchSize,
        int batchCount,
        IndexStatus indexStatus,
        long? maxWatermark = null,
        CancellationToken cancellationToken = default)
    {
        throw new BadRequestException(DicomSqlServerResource.SchemaVersionNeedsToBeUpgraded);
    }

    private async Task<IReadOnlyList<VersionedInstanceIdentifier>> GetInstanceIdentifierImp(
        PartitionEntry partitionEntry,
        string studyInstanceUid,
        CancellationToken cancellationToken,
        string seriesInstanceUid = null,
        string sopInstanceUid = null)
    {
        var results = new List<VersionedInstanceIdentifier>();

        using (SqlConnectionWrapper sqlConnectionWrapper = await SqlConnectionWrapperFactory.ObtainSqlConnectionWrapperAsync(cancellationToken))
        using (SqlCommandWrapper sqlCommandWrapper = sqlConnectionWrapper.CreateRetrySqlCommand())
        {
            VLatest.GetInstance.PopulateCommand(
                sqlCommandWrapper,
                validStatus: (byte)IndexStatus.Created,
                studyInstanceUid,
                seriesInstanceUid,
                sopInstanceUid);

            using (var reader = await sqlCommandWrapper.ExecuteReaderAsync(CommandBehavior.SequentialAccess, cancellationToken))
            {
                while (await reader.ReadAsync(cancellationToken))
                {
                    (string rStudyInstanceUid, string rSeriesInstanceUid, string rSopInstanceUid, long watermark) = reader.ReadRow(
                       VLatest.Instance.StudyInstanceUid,
                       VLatest.Instance.SeriesInstanceUid,
                       VLatest.Instance.SopInstanceUid,
                       VLatest.Instance.Watermark);

                    results.Add(new VersionedInstanceIdentifier(
                            rStudyInstanceUid,
                            rSeriesInstanceUid,
                            rSopInstanceUid,
                            watermark,
                            partitionEntry));
                }
            }
        }

        return results;
    }

    public virtual Task<IReadOnlyList<InstanceMetadata>> GetInstanceIdentifierWithPropertiesAsync(PartitionEntry partitionEntry, string studyInstanceUid, string seriesInstanceUid = null, string sopInstanceUid = null, CancellationToken cancellationToken = default)
    {
        throw new BadRequestException(DicomSqlServerResource.SchemaVersionNeedsToBeUpgraded);
    }

    public virtual Task<IReadOnlyList<WatermarkRange>> GetInstanceBatchesByTimeStampAsync(int batchSize, int batchCount, IndexStatus indexStatus, DateTimeOffset startTimeStamp, DateTimeOffset endTimeStamp, long? maxWatermark = null, CancellationToken cancellationToken = default)
    {
        throw new BadRequestException(DicomSqlServerResource.SchemaVersionNeedsToBeUpgraded);
    }
}
