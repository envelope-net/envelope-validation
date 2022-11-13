namespace Envelope.Validation;

public enum ValidatorType
{
	Validator = 0,
	NavigationValidator,
	EnumerableValidator,
	PropertyValidator,
	Email,
	DefaultOrEmpty,
	NotDefaultOrEmpty,
	Equal,
	NotEqual,
	MultiEqual,
	MultiNotEqual,
	Length,
	Range,
	Null,
	NotNull,
	PrecisionScale,
	RegEx,
	ErrorObject,
	ErrorProperty,
	ExactLength,
}
