using Envelope.Validation.Client;
using Envelope.Validation.Internal;
using Envelope.Validation.Results;
using Envelope.Extensions;
using Envelope.Reflection.ObjectPaths;
using Envelope.Text;

namespace Envelope.Validation.Validators;

public interface IPropertyValidator<T> { }

public class PropertyValidator<T, TProperty> : Validator<T>, IPropertyValidator<T>, IValidator<T>
{
	internal Func<T, TProperty> ValueGetter { get; }
	public Func<T?, bool>? Condition { get; }
	internal Func<T?, string?>? FailureInfoFunc { get; }

	protected Func<T?, TProperty?, string, string?>? MessageGetter { get; }
	protected Func<T?, TProperty?, string, string?>? MessageWithPropertyGetter { get; }
	protected virtual string DefaultValidationMessage { get; }
	protected virtual string DefaultValidationMessageWithProperty { get; }
	protected TemplateFormatter TemplateFormatter { get; }

	internal PropertyValidator(
		ValidatorType validatorType,
		Func<T, TProperty> valueGetter,
		IObjectPath objectPath,
		Func<T?, bool>? serverCondition,
		IClientConditionDefinition? clientConditionDefinition,
		Func<T?, string?>? failureInfoFunc,
		Func<T?, TProperty?, string, string?>? messageGetter,
		Func<T?, TProperty?, string, string?>? messageWithPropertyGetter)
		: base(validatorType, objectPath, serverCondition != null, clientConditionDefinition)
	{
		ValueGetter = valueGetter ?? throw new ArgumentNullException(nameof(valueGetter));
		Condition = serverCondition;
		FailureInfoFunc = failureInfoFunc;

		DefaultValidationMessage = "";
		DefaultValidationMessageWithProperty = "";
		MessageGetter = messageGetter;
		MessageWithPropertyGetter = messageWithPropertyGetter;
		TemplateFormatter = new TemplateFormatter();
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

		var propertyValue = ctx.InstanceToValidate != null
			? ValueGetter(ctx.InstanceToValidate)
			: default;

		var propertyCtx = new ValidationContext<T, TProperty>(ctx.InstanceToValidate, propertyValue, context);
			//.SetObjectPath(ObjectPath);

		foreach (var validator in NestedValidators)
		{
			var nestedValidationResult = validator.Validate(propertyCtx, options);
			result.Merge(nestedValidationResult);
			if (nestedValidationResult?.Interrupted == true)
				return result;
		}

		return result;
	}

	protected virtual IDictionary<string, object?> GetPlaceholderValues()
		=> new Dictionary<string, object?>();

	protected Func<T?, TProperty?, string, string?>? GetMessageGetter(Func<object?, object?, string, string?>? func)
	{
		if (MessageGetter != null)
			return MessageGetter;

		if (func == null)
			return default;

		return (instanceToValidate, valueToValidate, resourceKey) => func(instanceToValidate, valueToValidate, resourceKey);
	}

	protected Func<T?, TProperty?, string, string?>? GetMessageWithPropertyGetter(Func<object?, object?, string, string?>? func)
	{
		if (MessageWithPropertyGetter != null)
			return MessageWithPropertyGetter;

		if (func == null)
			return default;

		return (instanceToValidate, valueToValidate, resourceKey) => func(instanceToValidate, valueToValidate, resourceKey);
	}

	protected string GetValidationMessage(T? instanceToValidate, TProperty? valueToValidate, string resourceKey, Func<object?, object?, string, string?>? optionsMessageGetterFunc)
		=> GetFormattedMessage(
			instanceToValidate,
			valueToValidate,
			GetMessageGetter(optionsMessageGetterFunc),
			resourceKey,
			DefaultValidationMessage,
			GetPlaceholderValues());

	protected string GetValidationMessageWithProperty(T? instanceToValidate, TProperty? valueToValidate, string resourceKey, Func<object?, object?, string, string?>? optionsMessageWithPropertyGetterFunc)
		=> GetFormattedMessage(
			instanceToValidate,
			valueToValidate,
			GetMessageWithPropertyGetter(optionsMessageWithPropertyGetterFunc),
			resourceKey,
			DefaultValidationMessageWithProperty,
			GetPlaceholderValues());

	protected string GetFormattedMessage(
		T? instanceToValidate,
		TProperty? valueToValidate,
		Func<T?, TProperty?, string, string?>? resourceGetter,
		string resourceKey,
		string defaultMessage,
		IDictionary<string, object?>? placeholderValues = null)
	{
		string? template = null;

		if (resourceGetter != null)
			template = resourceGetter.Invoke(instanceToValidate, valueToValidate, resourceKey);

		if (string.IsNullOrWhiteSpace(template))
			template = ValidatorConfiguration.Localizer?.GetLocalizedString(resourceKey, defaultMessage) ?? defaultMessage;

		return TemplateFormatter.Format(template!, placeholderValues) ?? "?Error";
	}

	protected string? GetDisplayName()
		=> ObjectPath?.PropertyName; // ValidatorConfiguration.DisplayNameResolver?.Invoke(typeof(T), PropertyValidator.Expression, PropertyValidator.Expression) ?? PropertyValidator.ValidationFrame.PropertyName;
}
