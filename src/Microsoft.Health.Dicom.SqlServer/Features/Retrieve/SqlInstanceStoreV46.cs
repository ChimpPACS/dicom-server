﻿// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Health.Dicom.Core.Features.Common;
using Microsoft.Health.Dicom.Core.Features.Model;
using Microsoft.Health.Dicom.Core.Features.Partitioning;
using Microsoft.Health.Dicom.Core.Models;
using Microsoft.Health.Dicom.SqlServer.Extensions;
using Microsoft.Health.Dicom.SqlServer.Features.Schema;
using Microsoft.Health.Dicom.SqlServer.Features.Schema.Model;
using Microsoft.Health.SqlServer.Features.Client;
using Microsoft.Health.SqlServer.Features.Storage;

namespace Microsoft.Health.Dicom.SqlServer.Features.Retrieve;

internal class SqlInstanceStoreV46 : SqlInstanceStoreV34
{
    public SqlInstanceStoreV46(SqlConnectionWrapperFactory sqlConnectionWrapperFactory) : base(sqlConnectionWrapperFactory)
    {
    }

    public override SchemaVersion Version => SchemaVersion.V46;


    public override async Task<IReadOnlyList<InstanceMetadata>> GetInstanceIdentifierWithPropertiesAsync(Partition partition, string studyInstanceUid, string seriesInstanceUid = null, string sopInstanceUid = null, CancellationToken cancellationToken = default)
    {
        var results = new List<InstanceMetadata>();

        using (SqlConnectionWrapper sqlConnectionWrapper = await SqlConnectionWrapperFactory.ObtainSqlConnectionWrapperAsync(cancellationToken))
        using (SqlCommandWrapper sqlCommandWrapper = sqlConnectionWrapper.CreateRetrySqlCommand())
        {
            VLatest.GetInstanceWithPropertiesV46.PopulateCommand(
                sqlCommandWrapper,
                validStatus: (byte)IndexStatus.Created,
                partition.Key,
                studyInstanceUid,
                seriesInstanceUid,
                sopInstanceUid);

            using (var reader = await sqlCommandWrapper.ExecuteReaderAsync(CommandBehavior.SequentialAccess, cancellationToken))
            {
                while (await reader.ReadAsync(cancellationToken))
                {
                    (string rStudyInstanceUid,
                        string rSeriesInstanceUid,
                        string rSopInstanceUid,
                        long watermark,
                        string rTransferSyntaxUid,
                        bool rHasFrameMetadata,
                        long? originalWatermark,
                        long? newWatermark,
                        string filePath,
                        string eTag) = reader.ReadRow(
                          VLatest.Instance.StudyInstanceUid,
                          VLatest.Instance.SeriesInstanceUid,
                          VLatest.Instance.SopInstanceUid,
                          VLatest.Instance.Watermark,
                          VLatest.Instance.TransferSyntaxUid,
                          VLatest.Instance.HasFrameMetadata,
                          VLatest.Instance.OriginalWatermark,
                          VLatest.Instance.NewWatermark,
                          VLatest.FileProperty.FilePath.AsNullable(),
                          VLatest.FileProperty.ETag.AsNullable());

                    results.Add(
                        new InstanceMetadata(
                            new VersionedInstanceIdentifier(
                                rStudyInstanceUid,
                                rSeriesInstanceUid,
                                rSopInstanceUid,
                                watermark,
                                partition),
                            new InstanceProperties()
                            {
                                TransferSyntaxUid = rTransferSyntaxUid,
                                HasFrameMetadata = rHasFrameMetadata,
                                OriginalVersion = originalWatermark,
                                NewVersion = newWatermark,
                                fileProperties = string.IsNullOrEmpty(eTag) || string.IsNullOrEmpty(filePath)
                                    ? null
                                    : new FileProperties { ETag = eTag, Path = filePath }
                            }));
                }
            }
        }

        return results;
    }
}
