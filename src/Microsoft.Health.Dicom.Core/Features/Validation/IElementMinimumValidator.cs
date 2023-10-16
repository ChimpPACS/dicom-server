﻿// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using FellowOakDicom;
using Microsoft.Health.Dicom.Core.Exceptions;

namespace Microsoft.Health.Dicom.Core.Features.Validation;

/// <summary>
/// Minimum validator on Dicom Element
/// </summary>
public interface IElementMinimumValidator
{
    /// <summary>
    /// Validate Dicom Element.
    /// </summary>
    /// <param name="dicomElement">The Dicom Element</param>
    /// <param name="validationLevel">Style of validation to enforce on running rules</param>
    /// <exception cref="ElementValidationException"/>
    void Validate(DicomElement dicomElement, ValidationLevel validationLevel = ValidationLevel.Strict);
}
