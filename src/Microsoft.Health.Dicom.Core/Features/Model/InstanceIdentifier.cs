﻿// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using EnsureThat;
using Microsoft.Health.Dicom.Core.Features.Partition;

namespace Microsoft.Health.Dicom.Core.Features.Model;

public class InstanceIdentifier
{
    private const StringComparison EqualsStringComparison = StringComparison.Ordinal;

    public InstanceIdentifier(
        string studyInstanceUid,
        string seriesInstanceUid,
        string sopInstanceUid,
        PartitionEntry partitionEntry)
        : this(
            studyInstanceUid,
            seriesInstanceUid,
            sopInstanceUid,
            partitionEntry?.PartitionKey ?? throw new ArgumentNullException(nameof(partitionEntry)),
            partitionEntry?.PartitionName ?? throw new ArgumentNullException(nameof(partitionEntry)))
    {
        EnsureArg.IsNotNull(partitionEntry, nameof(partitionEntry));
    }

    public InstanceIdentifier(
        string studyInstanceUid,
        string seriesInstanceUid,
        string sopInstanceUid,
        int partitionKey = default,
        string partitionName = default)
    {
        EnsureArg.IsNotNullOrWhiteSpace(studyInstanceUid, nameof(studyInstanceUid));
        EnsureArg.IsNotNullOrWhiteSpace(seriesInstanceUid, nameof(seriesInstanceUid));
        EnsureArg.IsNotNullOrWhiteSpace(sopInstanceUid, nameof(sopInstanceUid));

        StudyInstanceUid = studyInstanceUid;
        SeriesInstanceUid = seriesInstanceUid;
        SopInstanceUid = sopInstanceUid;
        PartitionKey = partitionKey;
        PartitionName = partitionName;
    }

    public string StudyInstanceUid { get; }

    public string SeriesInstanceUid { get; }

    public string SopInstanceUid { get; }

    public int PartitionKey { get; }

    public string PartitionName { get; }

    public PartitionEntry PartitionEntry { get; }

    public override bool Equals(object obj)
    {
        if (obj is InstanceIdentifier identifier)
        {
            return StudyInstanceUid.Equals(identifier.StudyInstanceUid, EqualsStringComparison) &&
                    SeriesInstanceUid.Equals(identifier.SeriesInstanceUid, EqualsStringComparison) &&
                    SopInstanceUid.Equals(identifier.SopInstanceUid, EqualsStringComparison);
        }
        return false;
    }

    public override int GetHashCode()
        => (PartitionKey + StudyInstanceUid + SeriesInstanceUid + SopInstanceUid).GetHashCode(EqualsStringComparison);

    public override string ToString()
        => $"PartitionKey: {PartitionKey}, StudyInstanceUID: {StudyInstanceUid}, SeriesInstanceUID: {SeriesInstanceUid}, SOPInstanceUID: {SopInstanceUid}";
}
