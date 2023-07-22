// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

namespace Microsoft.Health.DicomCast.Core.Configurations;

public class FeatureConfiguration
{
    /// <summary>
    /// Do not sync values that are invalid and are not required
    /// </summary>
    public bool EnforceValidationOfTagValues { get; set; }

    /// <summary>
    /// Generate observations from dcm
    /// </summary>
    public bool GenerateObservations { get; set; }

    /// <summary>
    /// Ignore json parsing errors from DicomWebClient
    /// </summary>
    public bool IgnoreJsonParsingErrors { get; set; }
}
