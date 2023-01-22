using Envelope.Validation.Client;
using Envelope.Validation.Internal;
using Envelope.Validation.Results;
using Envelope.Extensions;
using Envelope.Reflection.ObjectPaths;

namespace Envelope.Validation.Validators.PropertyValidators;

internal class ErrorValidator<T, TProperty> : PropertyValidator<T, TProperty?>
{
	public ErrorValidator(
		Func<T, TProperty> valueGetter,
		IObjectPath objectPath,
		Func<T?, bool> serverCondition,
		IClientConditionDefinition? clientConditionDefinition,
		Func<T?, string?>? failureInfoFunc,
		Func<T?, TProperty?, string, string?>? messageGetter,
		Func<T?, TProperty?, string, string?>? messageWithPropertyGetter)
		: base(ValidatorType.ErrorProperty, valueGetter, objectPath, serverCondition, clientConditionDefinition, failureInfoFunc, messageGetter, messageWithPropertyGetter)
	{
		if (serverCondition == null)
			throw new ArgumentNullException(nameof(serverCondition));
	}

	protected override IDictionary<string, object?> GetPlaceholderValues()
		=> new Dictionary<string, object?>
			{
				{ "PropertyName", GetDisplayName() }
			};

	internal override ValidationResult? Validate(ValidationContext context, ValidationOptions? options)
	{
		//if (string.IsNullOrWhiteSpace(ObjectPath.PropertyName))
		//	throw new InvalidOperationException($"{nameof(ObjectPath)}.{nameof(ObjectPath.PropertyName)} == null");

		if (context is not ValidationContext<T> ctx)
			throw new ArgumentException($"{nameof(context)} must be type of {typeof(ValidationContext<T>).FullName}", nameof(context));

		if (Condition != null && Condition.Invoke(ctx.InstanceToValidate))
		{
			var value = ctx.InstanceToValidate != null
				? ValueGetter(ctx.InstanceToValidate)
				: default;

			return new ValidationResult(
				new ValidationFailure(
					ObjectPath,
					context,
					ValidatorType,
					HasServerCondition,
					ClientConditionDefinition,
					GetValidationMessage(ctx.InstanceToValidate, value, string.Empty, null),
					GetValidationMessageWithProperty(ctx.InstanceToValidate, value, string.Empty, null),
					FailureInfoFunc?.Invoke(ctx.InstanceToValidate)));
		}
		else if (ClientConditionDefinition != null && ClientConditionDefinition.Execute(ctx.InstanceToValidate))
		{
			var value = ctx.InstanceToValidate != null
				? ValueGetter(ctx.InstanceToValidate)
				: default;

			return new ValidationResult(
				new ValidationFailure(
					ObjectPath,
					context,
					ValidatorType,
					HasServerCondition,
					ClientConditionDefinition,
					GetValidationMessage(ctx.InstanceToValidate, value, string.Empty, null),
					GetValidationMessageWithProperty(ctx.InstanceToValidate, value, string.Empty, null),
					FailureInfoFunc?.Invoke(ctx.InstanceToValidate)));
		}

		return null;
	}

	public override IValidatorDescriptor ToDescriptor()
		=> new ValidationDescriptor(
			typeof(T),
			ObjectPath,
			ValidatorType,
			GetType().ToFriendlyFullName(),
			HasServerCondition,
			ClientConditionDefinition,
			GetValidationMessage(default, default, string.Empty, null),
			GetValidationMessageWithProperty(default, default, string.Empty, null))
		{
		};
}
