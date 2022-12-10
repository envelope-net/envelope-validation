using Envelope.Extensions;
using Envelope.Validation.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace Envelope.Validation.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddEnvelopeValidation<TSearchBaseAssembly>(this IServiceCollection services)
		=> AddEnvelopeValidation(services, typeof(TSearchBaseAssembly).Assembly);

	public static IServiceCollection AddEnvelopeValidation(this IServiceCollection services, params Assembly[] assemblies)
	{
		if (!assemblies.Any())
			throw new ArgumentNullException(nameof(assemblies), "At least one assembly is requred to scan for handlers.");

		var validatorManager = new ValidatorManager();

		var validatorDescriptorBuilderType = typeof(IValidatorDescriptorBuilder);

		var typesToScan =
			assemblies
				.Distinct()
				.SelectMany(a => a.DefinedTypes)
				.Where(type =>
					!type.IsInterface
					&& !type.IsAbstract
					&& validatorDescriptorBuilderType.IsAssignableFrom(type));

		bool found = false;
		foreach (var descriptorBuilderTypeInfo in typesToScan)
		{
			var attribute = descriptorBuilderTypeInfo.GetCustomAttribute<ValidatorRegisterAttribute>();
			if (0 < attribute?.RegisteredTypes?.Length)
			{
				foreach (var commandType in attribute.RegisteredTypes)
				{
					IValidatorDescriptorBuilder? validatorDescriptorBuilder = null;

					var defaultCtor = descriptorBuilderTypeInfo.GetDefaultConstructor();
					if (defaultCtor == null)
					{
						validatorDescriptorBuilder = (IValidatorDescriptorBuilder)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(descriptorBuilderTypeInfo);
						if (validatorDescriptorBuilder == null)
							throw new InvalidOperationException($"Cannot create instance of {descriptorBuilderTypeInfo}");
					}
					else
					{
						validatorDescriptorBuilder = (IValidatorDescriptorBuilder)defaultCtor.Invoke(null);
						if (validatorDescriptorBuilder == null)
							throw new InvalidOperationException($"Cannot create instance of {descriptorBuilderTypeInfo}");
					}

					found = validatorManager.RegisterValidatorDescriptorFor(validatorDescriptorBuilder.ObjectType, commandType, validatorDescriptorBuilder) || found;
				}
			}
		}

		//if (!found)
		//	throw new ConfigurationException("No validator was found.");

		services.TryAddSingleton<IValidatorManager>(validatorManager);

		return services;
	}
}
