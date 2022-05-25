using Envelope.Validation.Internal;
using Envelope.Validation.Results;
using Envelope.Reflection.ObjectPaths;

namespace Envelope.Validation.Validators;

public interface INavigationValidator<T> { }

public class NavigationValidator<T, TNavigation> : Validator<TNavigation>, INavigationValidator<T>, IValidator<TNavigation>
{
	internal Func<T, TNavigation> ValueGetter { get; }
	public Func<T?, bool>? Condition { get; }
	internal Func<T?, string?>? FailureInfoFunc { get; }

	public NavigationValidator(
		Func<T, TNavigation> valueGetter,
		IObjectPath objectPath,
		Func<T?, bool>? condition,
		Func<T?, string?>? failureInfoFunc)
		: base(ValidatorType.NavigationValidator, objectPath, condition != null, null)
	{
		ValueGetter = valueGetter ?? throw new ArgumentNullException(nameof(valueGetter));
		Condition = condition;
		FailureInfoFunc = failureInfoFunc;
	}

	internal override ValidationResult? Validate(ValidationContext context, ValidationOptions? options)
	{
		var result = new ValidationResult();

		//if (string.IsNullOrWhiteSpace(ObjectPath.PropertyName))
		//	throw new InvalidOperationException($"{nameof(ObjectPath)}.{nameof(ObjectPath.PropertyName)} == null");

		if (context is not ValidationContext<T> ctx)
			throw new ArgumentException($"{nameof(context)} must be type of {typeof(ValidationContext<T>).FullName}", nameof(context));

		if (Condition != null)
		{
			if (!Condition.Invoke(ctx.InstanceToValidate))
				result.SkipNestedValidation = true;
		}
		else if (ClientConditionDefinition != null)
		{
			if (!ClientConditionDefinition.Execute(ctx.InstanceToValidate))
				result.SkipNestedValidation = true;
		}

		if (result.SkipNestedValidation)
			return result;

		var navigationValue = ctx.InstanceToValidate != null
			? ValueGetter(ctx.InstanceToValidate)
			: default;

		var navigationCtx = new ValidationContext<TNavigation>(navigationValue, context);
			//.SetObjectPath(ObjectPath);

		foreach (var validator in NestedValidators)
		{
			var nestedValidationResult = validator.Validate(navigationCtx, options);
			result.Merge(nestedValidationResult);
			if (nestedValidationResult?.Interrupted == true)
				return result;
		}

		return result;
	}
}
