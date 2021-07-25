﻿// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Health.Dicom.Core.Exceptions;
using Microsoft.Health.Dicom.Core.Features.ExtendedQueryTag;
using Microsoft.Health.Dicom.SqlServer.Features.Schema;

namespace Microsoft.Health.Dicom.SqlServer.Features.ExtendedQueryTag
{
    internal class SqlExtendedQueryTagStoreV1 : ISqlExtendedQueryTagStore
    {
        public virtual SchemaVersion Version => SchemaVersion.V1;

        public virtual Task<IReadOnlyList<int>> AddExtendedQueryTagsAsync(
            IEnumerable<AddExtendedQueryTagEntry> extendedQueryTagEntries,
            int maxCount,
            bool ready = false,
            CancellationToken cancellationToken = default)
        {
            throw new BadRequestException(DicomSqlServerResource.SchemaVersionNeedsToBeUpgraded);
        }

        public virtual Task<IReadOnlyList<ExtendedQueryTagStoreEntry>> GetExtendedQueryTagsAsync(string path, CancellationToken cancellationToken = default)
        {
            throw new BadRequestException(DicomSqlServerResource.SchemaVersionNeedsToBeUpgraded);
        }

        public virtual Task<IReadOnlyList<ExtendedQueryTagStoreEntry>> GetExtendedQueryTagsByOperationAsync(string operationId, CancellationToken cancellationToken = default)
        {
            throw new BadRequestException(DicomSqlServerResource.SchemaVersionNeedsToBeUpgraded);
        }

        public virtual Task DeleteExtendedQueryTagAsync(string tagPath, string vr, CancellationToken cancellationToken = default)
        {
            throw new BadRequestException(DicomSqlServerResource.SchemaVersionNeedsToBeUpgraded);
        }

        public virtual Task<IReadOnlyList<ExtendedQueryTagStoreEntry>> GetExtendedQueryTagsAsync(IReadOnlyList<int> tagKeys, CancellationToken cancellationToken = default)
        {
            throw new BadRequestException(DicomSqlServerResource.SchemaVersionNeedsToBeUpgraded);
        }

        public virtual Task<IReadOnlyList<ExtendedQueryTagStoreEntry>> AssignReindexingOperationAsync(IReadOnlyList<int> queryTagKeys, string operationId, bool returnIfCompleted = false, CancellationToken cancellationToken = default)
        {
            throw new BadRequestException(DicomSqlServerResource.SchemaVersionNeedsToBeUpgraded);
        }

        public virtual Task<IReadOnlyList<int>> CompleteReindexingAsync(IReadOnlyList<int> queryTagKeys, CancellationToken cancellationToken = default)
        {
            throw new BadRequestException(DicomSqlServerResource.SchemaVersionNeedsToBeUpgraded);
        }

        public virtual Task<IReadOnlyList<ExtendedQueryTagError>> GetExtendedQueryTagErrorsAsync(string tagPath, CancellationToken cancellationToken = default)
        {
            throw new BadRequestException(DicomSqlServerResource.SchemaVersionNeedsToBeUpgraded);
        }

        public virtual Task<int> AddExtendedQueryTagErrorAsync(
            int tagKey,
            DateTime timestamp,
            int errorCode,
            string studyInstanceUid,
            string seriesInstanceUid,
            string sopInstanceUid,
            long sopInstanceKey,
            CancellationToken cancellationToken = default)
        {
            throw new BadRequestException(DicomSqlServerResource.SchemaVersionNeedsToBeUpgraded);
        }
    }
}
