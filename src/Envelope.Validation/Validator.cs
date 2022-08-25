using Envelope.Validation.Client;
using Envelope.Validation.Internal;
using Envelope.Validation.Results;
using Envelope.Extensions;
using Envelope.Reflection.ObjectPaths;
using Envelope.Validation;

namespace Envelope.Validation;

public abstract class ValidatorBase : IValidator
{
	protected List<ValidatorBase> NestedValidators { get; }
	public ValidatorType ValidatorType { get; }
	public IObjectPath ObjectPath { get; private set; }

	internal ValidatorBase(ValidatorType validatorType, IObjectPath objectPath)
	{
		ValidatorType = validatorType;
		ObjectPath = objectPath ?? throw new ArgumentNullException(nameof(objectPath));
		NestedValidators = new List<ValidatorBase>();
	}

	internal abstract ValidationResult? Validate(ValidationContext context, ValidationOptions? options);

	void IValidator.AddValidatorInternal(ValidatorBase validator)
	{
		if (validator == null)
			throw new ArgumentNullException(nameof(validator));

		NestedValidators.Add(validator);
	}

	//void IValidator.AttachValidator(ValidatorBase validator)
	//{
	//	if (validator == null)
	//		throw new ArgumentNullException(nameof(validator));

	//	var clonedObjectPath = ObjectPath.Clone(ObjectPathCloneMode.BottomUp);
	//	var clonedValidatorObjectPath = validator.ObjectPath.Clone(ObjectPathCloneMode.BottomUp);
	//	clonedObjectPath.SetDescendant(clonedValidatorObjectPath, clonedValidatorObjectPath.PropertyName!, true);
	//	validator.ObjectPath = clonedObjectPath.Descendant!;
	//	NestedValidators.Add(validator);
	//}

	public abstract IValidatorDescriptor ToDescriptor();
}

public class Validator<T> : ValidatorBase, IValidator<T>
{
	public bool HasServerCondition { get; }
	public IClientConditionDefinition? ClientConditionDefinition { get; }

	internal Validator(
		ValidatorType validatorType,
		IObjectPath objectPath,
		bool hasServerCondition,
		IClientConditionDefinition? clientConditionDefinition)
		: base(validatorType, objectPath)
	{
		HasServerCondition = hasServerCondition;
		ClientConditionDefinition = clientConditionDefinition;
	}

	public IValidationResult Validate(T? obj, ValidationOptions? options = null)
	{
		var ctx = new ValidationContext<T>(obj, null);
		return Validate(ctx, options) ?? new ValidationResult();
	}

	internal override ValidationResult? Validate(ValidationContext context, ValidationOptions? options)
	{
		var result = new ValidationResult();

		foreach (var validator in NestedValidators)
		{
			var nestedValidationResult = validator.Validate(context, options);
			result.Merge(nestedValidationResult);
			if (nestedValidationResult?.Interrupted == true)
				return result;
		}

		return result;
	}

	public override IValidatorDescriptor ToDescriptor()
		=> new ValidationDescriptor(
			typeof(T),
			ObjectPath,
			ValidatorType,
			GetType().ToFriendlyFullName(),
			HasServerCondition,
			ClientConditionDefinition,
			null,
			null)
			.AddValidators(NestedValidators);

	public override string ToString()
		=> $"{ValidatorType}<{typeof(T).FullName?.GetLastSplitSubstring(".")}> | {ObjectPath} | Conditional={HasServerCondition} | Validators={NestedValidators?.Count ?? 0}";
}
