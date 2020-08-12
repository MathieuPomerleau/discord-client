﻿using System;
using Injhinuity.Client.Core.Configuration;
using Injhinuity.Client.Core.Interfaces;
using Injhinuity.Client.Core.Validation.Builders;
using Injhinuity.Client.Core.Validation.Factories;
using Injhinuity.Client.Core.Validation.Validators;
using Injhinuity.Client.Core.Validation.Validators.SubValidators;
using Microsoft.Extensions.DependencyInjection;

namespace Injhinuity.Client.Core
{
    public class CoreRegistry : IRegistry
    {
        public void Register(IServiceCollection services)
        {
            services.AddSingleton<IAssemblyProvider, AssemblyProvider>();

            services.AddSingleton<IValidationResultBuilder, ValidationResultBuilder>()
                .AddTransient<IValidationResourceFactory, ValidationResourceFactory>();

            services.AddTransient(BuildCommandValidator);
        }

        private ICommandValidator BuildCommandValidator(IServiceProvider provider)
        {
            var config = provider.GetService<IClientConfig>().Validation;
            var resultBuilder = provider.GetService<IValidationResultBuilder>();

            ICommandValidator commandValidator = new CommandValidator();
            commandValidator.AddRoot(new NameValidator(resultBuilder, config.Command.CommandNameMaxLength))
                .AddNext(new BodyValidator(resultBuilder, config.Command.CommandBodyMaxLength));

            return commandValidator;
        }
    }
}
