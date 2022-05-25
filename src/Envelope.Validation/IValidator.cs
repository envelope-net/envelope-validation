using Envelope.Reflection.ObjectPaths;
using Envelope.Validation;

namespace Envelope.Validation;

public interface IValidator
{
	ValidatorType ValidatorType { get; }
	IObjectPath ObjectPath { get; }
	IValidatorDescriptor ToDescriptor();
	internal void AddValidator(ValidatorBase validator);
}

public interface IValidator<T> : IValidator
{
	IValidationResult Validate(T? obj, ValidationOptions? options = null);
}