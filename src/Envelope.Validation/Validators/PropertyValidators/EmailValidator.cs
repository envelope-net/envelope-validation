using Envelope.Validation.Client;
using Envelope.Validation.Internal;
using Envelope.Validation.Results;
using Envelope.Extensions;
using Envelope.Reflection.ObjectPaths;
using System.Text.RegularExpressions;

namespace Envelope.Validation.Validators.PropertyValidators;

internal class EmailValidator<T> : PropertyValidator<T, string?>
{
	private const string _expression = @"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-||_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+([a-z]+|\d|-|\.{0,1}|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])?([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))$";
	private static readonly Lazy<Regex> _regex = new(() => CreateRegEx());

	private static Regex CreateRegEx()
	{
		const RegexOptions options = RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture;
		return new Regex(_expression, options, TimeSpan.FromSeconds(2.0));
	}

	private const string DEFAULT_ValidationMessage = "Is not a valid email address.";
	private const string DEFAULT_ValidationMessageWithProperty = "'{PropertyName}' is not a valid email address.";

	protected override string DefaultValidationMessage => DEFAULT_ValidationMessage;
	protected override string DefaultValidationMessageWithProperty => DEFAULT_ValidationMessageWithProperty;

	public EmailValidator(
		Func<T, string> valueGetter,
		IObjectPath objectPath,
		Func<T?, bool>? condition,
		IClientConditionDefinition? clientConditionDefinition,
		Func<T?, string?>? failureInfoFunc,
		Func<T?, string?, string, string?>? messageGetter,
		Func<T?, string?, string, string?>? messageWithPropertyGetter)
		: base(ValidatorType.Email, valueGetter, objectPath, condition, clientConditionDefinition, failureInfoFunc, messageGetter, messageWithPropertyGetter)
	{
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

		if (context is not ValidationContext<T, string?> ctx)
			throw new ArgumentException($"{nameof(context)} must be type of {typeof(ValidationContext<T>).FullName}", nameof(context));

		if (ctx.ValueToValidate == null || _regex.Value.IsMatch(ctx.ValueToValidate))
			return null;
		else
			return new ValidationResult(
				new ValidationFailure(
					ObjectPath,
					context,
					ValidatorType,
					HasServerCondition,
					ClientConditionDefinition,
					GetValidationMessage(ctx.InstanceToValidate, ctx.ValueToValidate, Resources.Validation.__Keys.Email, options?.EmailMessageGetter),
					GetValidationMessageWithProperty(ctx.InstanceToValidate, ctx.ValueToValidate, Resources.Validation.__Keys.Email_WithProperty, options?.EmailMessageWithPropertyGetter),
					FailureInfoFunc?.Invoke(ctx.InstanceToValidate)));
	}

	public override IValidatorDescriptor ToDescriptor()
		=> new ValidationDescriptor(
			typeof(T),
			ObjectPath,
			ValidatorType,
			GetType().ToFriendlyFullName(),
			HasServerCondition,
			ClientConditionDefinition,
			GetValidationMessage(default, default, Resources.Validation.__Keys.Email, null),
			GetValidationMessageWithProperty(default, default, Resources.Validation.__Keys.Email_WithProperty, null))
		{
		};
}
