using Envelope.Validation.Client;
using Envelope.Validation.Internal;
using Envelope.Validation.Results;
using Envelope.Extensions;
using Envelope.Reflection.ObjectPaths;

namespace Envelope.Validation.Validators;

public interface IEnumerableValidator<T> { }

public class EnumerableValidator<T, TItem> : Validator<TItem>, IEnumerableValidator<T>, IValidator<TItem>
{
	internal Func<T, IEnumerable<TItem>> ValueGetter { get; }
	public Func<T?, bool>? Condition { get; }
	internal Func<T?, string?>? FailureInfoFunc { get; }

	public EnumerableValidator(
		Func<T, IEnumerable<TItem>> valueGetter,
		IObjectPath objectPath,
		Func<T?, bool>? condition,
		Func<T?, string?>? failureInfoFunc)
		: base(ValidatorType.EnumerableValidator, objectPath, condition != null, null)
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

		var enumerableValue = ctx.InstanceToValidate != null
			? ValueGetter(ctx.InstanceToValidate)
			: default;

		if (enumerableValue != null)
		{
			using var enumerator = enumerableValue.GetEnumerator();
			int index = -1;
			while (enumerator.MoveNext())
			{
				index++;

				var item = enumerator.Current;

				var enumerableCtx =
					new ValidationContext<TItem>(item, context);
						//.SetObjectPath(ObjectPath);

				enumerableCtx.Indexes[ObjectPath.Depth] = index;

				foreach (var validator in NestedValidators)
				{
					var nestedValidationResult = validator.Validate(enumerableCtx, options);
					result.Merge(nestedValidationResult);
					if (nestedValidationResult?.Interrupted == true)
						return result;
				}
			}
		}

		return result;
	}
}
