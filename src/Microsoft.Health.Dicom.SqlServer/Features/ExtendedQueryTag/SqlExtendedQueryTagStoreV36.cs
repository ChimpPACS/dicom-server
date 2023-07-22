// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Health.Dicom.Core;
using Microsoft.Health.Dicom.Core.Exceptions;
using Microsoft.Health.Dicom.Core.Features.ExtendedQueryTag;
using Microsoft.Health.Dicom.SqlServer.Extensions;
using Microsoft.Health.Dicom.SqlServer.Features.Schema;
using Microsoft.Health.Dicom.SqlServer.Features.Schema.Model;
using Microsoft.Health.SqlServer.Features.Client;
using Microsoft.Health.SqlServer.Features.Storage;

namespace Microsoft.Health.Dicom.SqlServer.Features.ExtendedQueryTag;

internal class SqlExtendedQueryTagStoreV36 : SqlExtendedQueryTagStoreV2
{
    public SqlExtendedQueryTagStoreV36(SqlConnectionWrapperFactory sqlConnectionWrapperFactory, ILogger<SqlExtendedQueryTagStoreV36> logger)
        : base(sqlConnectionWrapperFactory, logger)
    {
    }

    public override SchemaVersion Version => SchemaVersion.V36;

    public override async Task<IReadOnlyList<ExtendedQueryTagStoreJoinEntry>> GetExtendedQueryTagsAsync(int limit, long offset = 0, CancellationToken cancellationToken = default)
    {
        EnsureArg.IsGte(limit, 1, nameof(limit));
        EnsureArg.IsGte(offset, 0, nameof(offset));

        var results = new List<ExtendedQueryTagStoreJoinEntry>();

        using (SqlConnectionWrapper sqlConnectionWrapper = await ConnectionWrapperFactory.ObtainSqlConnectionWrapperAsync(cancellationToken))
        using (SqlCommandWrapper sqlCommandWrapper = sqlConnectionWrapper.CreateRetrySqlCommand())
        {
            VLatest.GetExtendedQueryTagsV36.PopulateCommand(sqlCommandWrapper, limit, offset);

            var executionTimeWatch = Stopwatch.StartNew();
            using (var reader = await sqlCommandWrapper.ExecuteReaderAsync(CommandBehavior.SequentialAccess, cancellationToken))
            {
                while (await reader.ReadAsync(cancellationToken))
                {
                    (int tagKey, string tagPath, string tagVR, string tagPrivateCreator, int tagLevel, int tagStatus, byte queryStatus, int errorCount, Guid? operationId) = reader.ReadRow(
                        VLatest.ExtendedQueryTag.TagKey,
                        VLatest.ExtendedQueryTag.TagPath,
                        VLatest.ExtendedQueryTag.TagVR,
                        VLatest.ExtendedQueryTag.TagPrivateCreator,
                        VLatest.ExtendedQueryTag.TagLevel,
                        VLatest.ExtendedQueryTag.TagStatus,
                        VLatest.ExtendedQueryTag.QueryStatus,
                        VLatest.ExtendedQueryTag.ErrorCount,
                        VLatest.ExtendedQueryTagOperation.OperationId.AsNullable());

                    results.Add(new ExtendedQueryTagStoreJoinEntry(tagKey, tagPath, tagVR, tagPrivateCreator, (QueryTagLevel)tagLevel, (ExtendedQueryTagStatus)tagStatus, (QueryStatus)queryStatus, errorCount, operationId));
                }

                executionTimeWatch.Stop();
                Logger.StoredProcedureSucceeded(nameof(VLatest.GetExtendedQueryTags), executionTimeWatch);
            }
        }

        return results;
    }

    public override async Task<ExtendedQueryTagStoreJoinEntry> GetExtendedQueryTagAsync(string tagPath, CancellationToken cancellationToken = default)
    {
        using (SqlConnectionWrapper sqlConnectionWrapper = await ConnectionWrapperFactory.ObtainSqlConnectionWrapperAsync(cancellationToken))
        using (SqlCommandWrapper sqlCommandWrapper = sqlConnectionWrapper.CreateRetrySqlCommand())
        {
            VLatest.GetExtendedQueryTag.PopulateCommand(sqlCommandWrapper, tagPath);

            var executionTimeWatch = Stopwatch.StartNew();
            using (var reader = await sqlCommandWrapper.ExecuteReaderAsync(CommandBehavior.SequentialAccess, cancellationToken))
            {
                if (!await reader.ReadAsync(cancellationToken))
                {
                    throw new ExtendedQueryTagNotFoundException(string.Format(CultureInfo.CurrentCulture, DicomCoreResource.ExtendedQueryTagNotFound, tagPath));
                }

                (int tagKey, string path, string tagVR, string tagPrivateCreator, int tagLevel, int tagStatus, byte queryStatus, int errorCount, Guid? operationId) = reader.ReadRow(
                    VLatest.ExtendedQueryTag.TagKey,
                    VLatest.ExtendedQueryTag.TagPath,
                    VLatest.ExtendedQueryTag.TagVR,
                    VLatest.ExtendedQueryTag.TagPrivateCreator,
                    VLatest.ExtendedQueryTag.TagLevel,
                    VLatest.ExtendedQueryTag.TagStatus,
                    VLatest.ExtendedQueryTag.QueryStatus,
                    VLatest.ExtendedQueryTag.ErrorCount,
                    VLatest.ExtendedQueryTagOperation.OperationId.AsNullable());

                executionTimeWatch.Stop();
                Logger.StoredProcedureSucceeded(nameof(VLatest.GetExtendedQueryTag), executionTimeWatch);

                return new ExtendedQueryTagStoreJoinEntry(tagKey, path, tagVR, tagPrivateCreator, (QueryTagLevel)tagLevel, (ExtendedQueryTagStatus)tagStatus, (QueryStatus)queryStatus, errorCount, operationId);
            }

        }
    }

    public override async Task<IReadOnlyList<ExtendedQueryTagStoreJoinEntry>> GetExtendedQueryTagsAsync(IReadOnlyCollection<int> queryTagKeys, CancellationToken cancellationToken = default)
    {
        EnsureArg.HasItems(queryTagKeys, nameof(queryTagKeys));

        var results = new List<ExtendedQueryTagStoreJoinEntry>();

        using (SqlConnectionWrapper sqlConnectionWrapper = await ConnectionWrapperFactory.ObtainSqlConnectionWrapperAsync(cancellationToken))
        using (SqlCommandWrapper sqlCommandWrapper = sqlConnectionWrapper.CreateRetrySqlCommand())
        {
            IEnumerable<ExtendedQueryTagKeyTableTypeV1Row> rows = queryTagKeys.Select(x => new ExtendedQueryTagKeyTableTypeV1Row(x));
            VLatest.GetExtendedQueryTagsByKey.PopulateCommand(sqlCommandWrapper, rows);

            var executionTimeWatch = Stopwatch.StartNew();
            using (var reader = await sqlCommandWrapper.ExecuteReaderAsync(CommandBehavior.SequentialAccess, cancellationToken))
            {
                while (await reader.ReadAsync(cancellationToken))
                {
                    (int tagKey, string tagPath, string tagVR, string tagPrivateCreator, int tagLevel, int tagStatus, byte queryStatus, int errorCount, Guid? operationId) = reader.ReadRow(
                        VLatest.ExtendedQueryTag.TagKey,
                        VLatest.ExtendedQueryTag.TagPath,
                        VLatest.ExtendedQueryTag.TagVR,
                        VLatest.ExtendedQueryTag.TagPrivateCreator,
                        VLatest.ExtendedQueryTag.TagLevel,
                        VLatest.ExtendedQueryTag.TagStatus,
                        VLatest.ExtendedQueryTag.QueryStatus,
                        VLatest.ExtendedQueryTag.ErrorCount,
                        VLatest.ExtendedQueryTagOperation.OperationId.AsNullable());

                    results.Add(new ExtendedQueryTagStoreJoinEntry(tagKey, tagPath, tagVR, tagPrivateCreator, (QueryTagLevel)tagLevel, (ExtendedQueryTagStatus)tagStatus, (QueryStatus)queryStatus, errorCount, operationId));
                }

                executionTimeWatch.Stop();
                Logger.StoredProcedureSucceeded(nameof(VLatest.GetExtendedQueryTagsByKey), executionTimeWatch);
            }
        }

        return results;
    }

    public override async Task<IReadOnlyList<ExtendedQueryTagStoreEntry>> GetExtendedQueryTagsAsync(Guid operationId, CancellationToken cancellationToken = default)
    {
        var results = new List<ExtendedQueryTagStoreEntry>();

        using (SqlConnectionWrapper sqlConnectionWrapper = await ConnectionWrapperFactory.ObtainSqlConnectionWrapperAsync(cancellationToken))
        using (SqlCommandWrapper sqlCommandWrapper = sqlConnectionWrapper.CreateRetrySqlCommand())
        {
            VLatest.GetExtendedQueryTagsByOperation.PopulateCommand(sqlCommandWrapper, operationId);

            using (SqlDataReader reader = await sqlCommandWrapper.ExecuteReaderAsync(CommandBehavior.SequentialAccess, cancellationToken))
            {
                while (await reader.ReadAsync(cancellationToken))
                {
                    (int tagKey, string tagPath, string tagVR, string tagPrivateCreator, int tagLevel, int tagStatus, byte queryStatus, int errorCount) = reader.ReadRow(
                        VLatest.ExtendedQueryTag.TagKey,
                        VLatest.ExtendedQueryTag.TagPath,
                        VLatest.ExtendedQueryTag.TagVR,
                        VLatest.ExtendedQueryTag.TagPrivateCreator,
                        VLatest.ExtendedQueryTag.TagLevel,
                        VLatest.ExtendedQueryTag.TagStatus,
                        VLatest.ExtendedQueryTag.QueryStatus,
                        VLatest.ExtendedQueryTag.ErrorCount);

                    results.Add(new ExtendedQueryTagStoreEntry(tagKey, tagPath, tagVR, tagPrivateCreator, (QueryTagLevel)tagLevel, (ExtendedQueryTagStatus)tagStatus, (QueryStatus)queryStatus, errorCount));
                }
            }
        }

        return results;
    }

    public override async Task<IReadOnlyList<ExtendedQueryTagStoreEntry>> AddExtendedQueryTagsAsync(
        IReadOnlyCollection<AddExtendedQueryTagEntry> extendedQueryTagEntries,
        int maxAllowedCount,
        bool ready = false,
        CancellationToken cancellationToken = default)
    {
        EnsureArg.IsNotNull(extendedQueryTagEntries, nameof(extendedQueryTagEntries));
        EnsureArg.IsGt(maxAllowedCount, 0, nameof(maxAllowedCount));

        using SqlConnectionWrapper sqlConnectionWrapper = await ConnectionWrapperFactory.ObtainSqlConnectionWrapperAsync(cancellationToken);
        using SqlCommandWrapper sqlCommandWrapper = sqlConnectionWrapper.CreateRetrySqlCommand();

        IEnumerable<AddExtendedQueryTagsInputTableTypeV1Row> rows = extendedQueryTagEntries.Select(ToAddExtendedQueryTagsInputTableTypeV1Row);
        VLatest.AddExtendedQueryTags.PopulateCommand(sqlCommandWrapper, maxAllowedCount, ready, new VLatest.AddExtendedQueryTagsTableValuedParameters(rows));

        try
        {
            var results = new List<ExtendedQueryTagStoreEntry>();
            using SqlDataReader reader = await sqlCommandWrapper.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                (int tagKey, string tagPath, string tagVR, string tagPrivateCreator, int tagLevel, int tagStatus, byte queryStatus, int errorCount) = reader.ReadRow(
                        VLatest.ExtendedQueryTag.TagKey,
                        VLatest.ExtendedQueryTag.TagPath,
                        VLatest.ExtendedQueryTag.TagVR,
                        VLatest.ExtendedQueryTag.TagPrivateCreator,
                        VLatest.ExtendedQueryTag.TagLevel,
                        VLatest.ExtendedQueryTag.TagStatus,
                        VLatest.ExtendedQueryTag.QueryStatus,
                        VLatest.ExtendedQueryTag.ErrorCount);

                results.Add(new ExtendedQueryTagStoreEntry(tagKey, tagPath, tagVR, tagPrivateCreator, (QueryTagLevel)tagLevel, (ExtendedQueryTagStatus)tagStatus, (QueryStatus)queryStatus, errorCount));
            }

            return results;
        }
        catch (SqlException ex)
        {
            throw ex.Number switch
            {
                SqlErrorCodes.Conflict => ex.State == 1
                    ? new ExtendedQueryTagsExceedsMaxAllowedCountException(maxAllowedCount)
                    : new ExtendedQueryTagsAlreadyExistsException(),
                _ => new DataStoreException(ex),
            };
        }
    }

    public override async Task<IReadOnlyList<ExtendedQueryTagStoreEntry>> AssignReindexingOperationAsync(
        IReadOnlyCollection<int> queryTagKeys,
        Guid operationId,
        bool returnIfCompleted = false,
        CancellationToken cancellationToken = default)
    {
        EnsureArg.HasItems(queryTagKeys, nameof(queryTagKeys));

        using SqlConnectionWrapper sqlConnectionWrapper = await ConnectionWrapperFactory.ObtainSqlConnectionWrapperAsync(cancellationToken);
        using SqlCommandWrapper sqlCommandWrapper = sqlConnectionWrapper.CreateRetrySqlCommand();

        IEnumerable<ExtendedQueryTagKeyTableTypeV1Row> rows = queryTagKeys.Select(x => new ExtendedQueryTagKeyTableTypeV1Row(x));
        VLatest.AssignReindexingOperation.PopulateCommand(sqlCommandWrapper, rows, operationId, returnIfCompleted);

        try
        {
            var queryTags = new List<ExtendedQueryTagStoreEntry>();
            using SqlDataReader reader = await sqlCommandWrapper.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                (int tagKey, string tagPath, string tagVR, string tagPrivateCreator, byte tagLevel, byte tagStatus, byte queryStatus, int errorCount) = reader.ReadRow(
                    VLatest.ExtendedQueryTag.TagKey,
                    VLatest.ExtendedQueryTag.TagPath,
                    VLatest.ExtendedQueryTag.TagVR,
                    VLatest.ExtendedQueryTag.TagPrivateCreator,
                    VLatest.ExtendedQueryTag.TagLevel,
                    VLatest.ExtendedQueryTag.TagStatus,
                    VLatest.ExtendedQueryTag.QueryStatus,
                    VLatest.ExtendedQueryTag.ErrorCount);

                queryTags.Add(new ExtendedQueryTagStoreEntry(
                    tagKey,
                    tagPath,
                    tagVR,
                    tagPrivateCreator,
                    (QueryTagLevel)tagLevel,
                    (ExtendedQueryTagStatus)tagStatus,
                    (QueryStatus)queryStatus,
                    errorCount));
            }

            return queryTags;
        }
        catch (SqlException ex)
        {
            throw new DataStoreException(ex);
        }
    }

    public override async Task<IReadOnlyList<int>> CompleteReindexingAsync(IReadOnlyCollection<int> queryTagKeys, CancellationToken cancellationToken = default)
    {
        EnsureArg.HasItems(queryTagKeys, nameof(queryTagKeys));

        using SqlConnectionWrapper sqlConnectionWrapper = await ConnectionWrapperFactory.ObtainSqlConnectionWrapperAsync(cancellationToken);
        using SqlCommandWrapper sqlCommandWrapper = sqlConnectionWrapper.CreateRetrySqlCommand();

        IEnumerable<ExtendedQueryTagKeyTableTypeV1Row> rows = queryTagKeys.Select(x => new ExtendedQueryTagKeyTableTypeV1Row(x));
        VLatest.CompleteReindexing.PopulateCommand(sqlCommandWrapper, rows);

        try
        {
            var keys = new List<int>();
            using SqlDataReader reader = await sqlCommandWrapper.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                keys.Add(reader.ReadRow(VLatest.ExtendedQueryTagString.TagKey));
            }

            return keys;
        }
        catch (SqlException ex)
        {
            throw new DataStoreException(ex);
        }
    }

    ///<inheritdoc/>
    public override async Task<ExtendedQueryTagStoreJoinEntry> UpdateQueryStatusAsync(string tagPath, QueryStatus queryStatus, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNullOrWhiteSpace(tagPath, nameof(tagPath));
        EnsureArg.EnumIsDefined(queryStatus, nameof(queryStatus));

        using SqlConnectionWrapper sqlConnectionWrapper = await ConnectionWrapperFactory.ObtainSqlConnectionWrapperAsync(cancellationToken);
        using SqlCommandWrapper sqlCommandWrapper = sqlConnectionWrapper.CreateRetrySqlCommand();

        VLatest.UpdateExtendedQueryTagQueryStatus.PopulateCommand(sqlCommandWrapper, tagPath, (byte)queryStatus);

        try
        {
            using SqlDataReader reader = await sqlCommandWrapper.ExecuteReaderAsync(cancellationToken);
            if (!await reader.ReadAsync(cancellationToken))
            {
                throw new ExtendedQueryTagNotFoundException(string.Format(CultureInfo.InvariantCulture, DicomSqlServerResource.ExtendedQueryTagNotFound, tagPath));
            }

            (int rTagKey, string rTagPath, string rTagVR, string rTagPrivateCreator, byte rTagLevel, byte rTagStatus, byte rQueryStatus, int errorCount, Guid? operationId) = reader.ReadRow(
                VLatest.ExtendedQueryTag.TagKey,
                VLatest.ExtendedQueryTag.TagPath,
                VLatest.ExtendedQueryTag.TagVR,
                VLatest.ExtendedQueryTag.TagPrivateCreator,
                VLatest.ExtendedQueryTag.TagLevel,
                VLatest.ExtendedQueryTag.TagStatus,
                VLatest.ExtendedQueryTag.QueryStatus,
                VLatest.ExtendedQueryTag.ErrorCount,
                VLatest.ExtendedQueryTagOperation.OperationId.AsNullable());

            return new ExtendedQueryTagStoreJoinEntry(rTagKey, rTagPath, rTagVR, rTagPrivateCreator, (QueryTagLevel)rTagLevel, (ExtendedQueryTagStatus)rTagStatus, (QueryStatus)rQueryStatus, errorCount, operationId);

        }
        catch (SqlException ex)
        {
            throw new DataStoreException(ex);
        }
    }
    public override async Task DeleteExtendedQueryTagAsync(string tagPath, string vr, CancellationToken cancellationToken = default)
    {
        using (SqlConnectionWrapper sqlConnectionWrapper = await ConnectionWrapperFactory.ObtainSqlConnectionWrapperAsync(cancellationToken))
        using (SqlCommandWrapper sqlCommandWrapper = sqlConnectionWrapper.CreateRetrySqlCommand())
        {
            VLatest.DeleteExtendedQueryTagV16.PopulateCommand(sqlCommandWrapper, tagPath, (byte)ExtendedQueryTagLimit.ExtendedQueryTagVRAndDataTypeMapping[vr], batchSize: 1000);

            try
            {
                await sqlCommandWrapper.ExecuteNonQueryAsync(cancellationToken);
            }
            catch (SqlException ex)
            {
                throw ex.Number switch
                {
                    SqlErrorCodes.NotFound => new ExtendedQueryTagNotFoundException(
                        string.Format(CultureInfo.InvariantCulture, DicomSqlServerResource.ExtendedQueryTagNotFound, tagPath)),
                    _ => new DataStoreException(ex),
                };
            }
        }
    }
}
