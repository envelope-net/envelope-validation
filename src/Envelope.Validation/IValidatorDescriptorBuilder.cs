namespace Envelope.Validation;

public interface IValidatorDescriptorBuilder
{
	Type ObjectType { get; }

	IValidatorDescriptor ToDescriptor(IServiceProvider serviceProvider);

	IValidatorDescriptor ToDescriptor(IServiceProvider serviceProvider, object? state = null);
}
