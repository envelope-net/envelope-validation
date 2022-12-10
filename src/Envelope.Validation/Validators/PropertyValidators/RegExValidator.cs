using Envelope.Validation.Client;
using Envelope.Validation.Internal;
using Envelope.Validation.Results;
using Envelope.Extensions;
using Envelope.Reflection.ObjectPaths;
using System.Text.RegularExpressions;

namespace Envelope.Validation.Validators.PropertyValidators;

internal class RegExValidator<T> : PropertyValidator<T, string?>
{
	private const string DEFAULT_ValidationMessage = "Is not in the correct format.";
	private const string DEFAULT_ValidationMessageWithProperty = "'{PropertyName}' is not in the correct format.";

	protected override string DefaultValidationMessage => DEFAULT_ValidationMessage;
	protected override string DefaultValidationMessageWithProperty => DEFAULT_ValidationMessageWithProperty;

	private readonly Regex? _regex;
	public string? Pattern { get; }

	public RegExValidator(
		Func<T, string> valueGetter,
		IObjectPath objectPath,
		Func<T?, bool>? condition,
		IClientConditionDefinition? clientConditionDefinition,
		Func<T?, string?>? failureInfoFunc,
		string? pattern,
		Func<T?, string?, string, string?>? messageGetter,
		Func<T?, string?, string, string?>? messageWithPropertyGetter)
		: base(ValidatorType.RegEx, valueGetter, objectPath, condition, clientConditionDefinition, failureInfoFunc, messageGetter, messageWithPropertyGetter)
	{
		Pattern = pattern;
		if (Pattern != null)
			_regex = new Regex(Pattern, RegexOptions.None, TimeSpan.FromSeconds(2.0));
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

		if (ctx.ValueToValidate == null || _regex == null || _regex.IsMatch(ctx.ValueToValidate))
			return null;
		else
			return new ValidationResult(
				new ValidationFailure(
					ObjectPath,
					context,
					ValidatorType,
					HasServerCondition,
					ClientConditionDefinition,
					GetValidationMessage(ctx.InstanceToValidate, ctx.ValueToValidate, Resources.Validation.__Keys.RegEx, options?.RegExMessageGetter),
					GetValidationMessageWithProperty(ctx.InstanceToValidate, ctx.ValueToValidate, Resources.Validation.__Keys.RegEx_WithProperty, options?.RegExMessageWithPropertyGetter),
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
			GetValidationMessage(default, default, Resources.Validation.__Keys.RegEx, null),
			GetValidationMessageWithProperty(default, default, Resources.Validation.__Keys.RegEx_WithProperty, null))
		{
			Pattern = Pattern
		};
}
