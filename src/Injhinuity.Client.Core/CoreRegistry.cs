using System;
using Discord;
using Injhinuity.Client.Core.Configuration;
using Injhinuity.Client.Core.Interfaces;
using Injhinuity.Client.Core.Validation.Builders;
using Injhinuity.Client.Core.Validation.Entities.Resources.Interfaces;
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
            services.AddTransient(BuildRoleValidator);
        }

        private ICommandValidator BuildCommandValidator(IServiceProvider provider)
        {
            var config = provider.GetService<IClientConfig>()!.Validation;
            var resultBuilder = provider.GetService<IValidationResultBuilder>()!;

            ICommandValidator validator = new CommandValidator();
            validator.AddRoot(new NameValidator(resultBuilder, config.Command.NameMaxLength))
                .AddNext(new BodyValidator(resultBuilder, config.Command.BodyMaxLength));

            return validator;
        }

        private IRoleValidator BuildRoleValidator(IServiceProvider provider)
        {
            var config = provider.GetService<IClientConfig>()!.Validation;
            var resultBuilder = provider.GetService<IValidationResultBuilder>()!;

            IRoleValidator validator = new RoleValidator();
            validator.AddRoot(new NameValidator(resultBuilder, int.MaxValue))
                .AddNext(new EmoteValidator(resultBuilder, TryParseEmote));

            return validator;
        }

        private bool TryParseEmote(IEmoteResource resource) =>
            Emote.TryParse(resource.Emote, out var _);
    }
}
