﻿using Injhinuity.Client.Core.Validation.Entities.Resources.Interfaces;

namespace Injhinuity.Client.Core.Validation.Entities.Resources
{
    public record RoleResource(string? Name) : IValidationResource, INameResource {}
}
