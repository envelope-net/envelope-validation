namespace Envelope.Validation;

public interface IValidatorManager
{
	Dictionary<Type, IValidatorDescriptorBuilder>? GetValidatorDescriptorBuilderFor<T>();

	Dictionary<Type, IValidatorDescriptorBuilder>? GetValidatorDescriptorBuilderFor(Type objectType);

	IValidatorDescriptorBuilder? GetValidatorDescriptorBuilderFor<T, TCommand>();

	IValidatorDescriptorBuilder? GetValidatorDescriptorBuilderFor(Type objectType, Type commandType);

	IValidatorDescriptor? GetValidatorDescriptorFor(Type objectType, Type commandType, IServiceProvider serviceProvider, object? state = null);
}
