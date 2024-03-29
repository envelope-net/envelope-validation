﻿using Envelope.Reflection.ObjectPaths;

namespace Envelope.Validation;

public interface IValidator
{
	ValidatorType ValidatorType { get; }
	IObjectPath ObjectPath { get; }
	IValidatorDescriptor ToDescriptor();
	void AddValidatorInternal(ValidatorBase validator);
}

public interface IValidator<T> : IValidator
{
	IValidationResult Validate(T? obj, ValidationOptions? options = null);
}