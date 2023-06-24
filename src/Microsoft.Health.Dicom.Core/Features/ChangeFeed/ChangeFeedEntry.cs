﻿// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Text.Json.Serialization;
using EnsureThat;
using FellowOakDicom;

namespace Microsoft.Health.Dicom.Core.Features.ChangeFeed;

/// <summary>
/// Represents each change feed entry of a change has retrieved from the store
/// </summary>
public class ChangeFeedEntry
{
    public ChangeFeedEntry(
        long sequence,
        DateTimeOffset timestamp,
        ChangeFeedAction action,
        string studyInstanceUid,
        string seriesInstanceUid,
        string sopInstanceUid,
        long originalVersion,
        long? currentVersion,
        ChangeFeedState state,
        string partitionName = default,
        DicomDataset metadata = null,
        string filePath = null)
    {
        EnsureArg.IsNotNull(studyInstanceUid);
        EnsureArg.IsNotNull(seriesInstanceUid);
        EnsureArg.IsNotNull(sopInstanceUid);

        Sequence = sequence;
        StudyInstanceUid = studyInstanceUid;
        SeriesInstanceUid = seriesInstanceUid;
        SopInstanceUid = sopInstanceUid;
        Action = action;
        Timestamp = timestamp;
        State = state;
        OriginalVersion = originalVersion;
        CurrentVersion = currentVersion;
        PartitionName = partitionName;
        Metadata = metadata;
        FilePath = filePath;
    }

    public long Sequence { get; }

    public string PartitionName { get; }

    public string FilePath { get; }

    public string StudyInstanceUid { get; }

    public string SeriesInstanceUid { get; }

    public string SopInstanceUid { get; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ChangeFeedAction Action { get; }

    public DateTimeOffset Timestamp { get; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ChangeFeedState State { get; }

    [JsonIgnore]
    public long OriginalVersion { get; }

    [JsonIgnore]
    public long? CurrentVersion { get; }

    public DicomDataset Metadata { get; set; }

    /// <summary>
    /// Control variable to determine whether or not the <see cref="Metadata"/> property should be included in a serialized view.
    /// </summary>
    [JsonIgnore]
    public bool IncludeMetadata { get; set; }

    /// <summary>
    /// Json.Net method for determining whether or not to serialize the <see cref="Metadata"/> property
    /// </summary>
    /// <returns>A boolean representing if the <see cref="Metadata"/> should be serialized.</returns>
    public bool ShouldSerializeMetadata()
    {
        return IncludeMetadata;
    }
}
