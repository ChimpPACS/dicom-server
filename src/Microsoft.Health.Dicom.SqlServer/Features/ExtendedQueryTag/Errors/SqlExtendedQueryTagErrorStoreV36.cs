// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Health.Dicom.Core.Exceptions;
using Microsoft.Health.Dicom.Core.Extensions;
using Microsoft.Health.Dicom.Core.Features.ExtendedQueryTag;
using Microsoft.Health.Dicom.Core.Features.Validation;
using Microsoft.Health.Dicom.SqlServer.Features.Schema;
using Microsoft.Health.Dicom.SqlServer.Features.Schema.Model;
using Microsoft.Health.SqlServer.Features.Client;
using Microsoft.Health.SqlServer.Features.Storage;

namespace Microsoft.Health.Dicom.SqlServer.Features.ExtendedQueryTag.Errors;

internal class SqlExtendedQueryTagErrorStoreV36 : SqlExtendedQueryTagErrorStoreV4
{
    public SqlExtendedQueryTagErrorStoreV36(SqlConnectionWrapperFactory sqlConnectionWrapperFactory, ILogger<SqlExtendedQueryTagErrorStoreV36> logger)
        : base(sqlConnectionWrapperFactory, logger)
    {
    }

    public override SchemaVersion Version => SchemaVersion.V36;

    public override async Task<IReadOnlyList<ExtendedQueryTagError>> GetExtendedQueryTagErrorsAsync(string tagPath, int limit, long offset, CancellationToken cancellationToken = default)
    {
        EnsureArg.IsGte(limit, 1, nameof(limit));
        EnsureArg.IsGte(offset, 0, nameof(offset));

        List<ExtendedQueryTagError> results = new List<ExtendedQueryTagError>();

        using SqlConnectionWrapper sqlConnectionWrapper = await ConnectionWrapperFactory.ObtainSqlConnectionWrapperAsync(cancellationToken);
        using SqlCommandWrapper sqlCommandWrapper = sqlConnectionWrapper.CreateRetrySqlCommand();

        VLatest.GetExtendedQueryTagErrorsV36.PopulateCommand(sqlCommandWrapper, tagPath, limit, offset);

        try
        {
            using SqlDataReader reader = await sqlCommandWrapper.ExecuteReaderAsync(CommandBehavior.SequentialAccess, cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                (int tagkey, short errorCode, DateTime createdTime, string partitionName, string studyInstanceUid, string seriesInstanceUid, string sopInstanceUid) = reader.ReadRow(
                    VLatest.ExtendedQueryTagError.TagKey,
                    VLatest.ExtendedQueryTagError.ErrorCode,
                    VLatest.ExtendedQueryTagError.CreatedTime,
                    VLatest.Partition.PartitionName,
                    VLatest.Instance.StudyInstanceUid,
                    VLatest.Instance.SeriesInstanceUid,
                    VLatest.Instance.SopInstanceUid);

                results.Add(new ExtendedQueryTagError(createdTime, studyInstanceUid, seriesInstanceUid, sopInstanceUid, ((ValidationErrorCode)errorCode).GetMessage(), partitionName));
            }
        }
        catch (SqlException e)
        {
            if (e.Number == SqlErrorCodes.NotFound)
            {
                throw new ExtendedQueryTagNotFoundException(
                    string.Format(CultureInfo.InvariantCulture, DicomSqlServerResource.ExtendedQueryTagNotFound, tagPath));
            }

            throw new DataStoreException(e);
        }

        return results;
    }
}
