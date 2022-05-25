namespace Envelope.Validation;

public class ValidationOptions
{
	public Func<object?, object?, string, string?>? DefaultOrEmptyMessageGetter { get; set; }
	public Func<object?, object?, string, string?>? DefaultOrEmptyMessageWithPropertyGetter { get; set; }
	public Func<object?, object?, string, string?>? NotDefaultOrEmptyMessageGetter { get; set; }
	public Func<object?, object?, string, string?>? NotDefaultOrEmptyMessageWithPropertyGetter { get; set; }
	public Func<object?, object?, string, string?>? EmailMessageGetter { get; set; }
	public Func<object?, object?, string, string?>? EmailMessageWithPropertyGetter { get; set; }
	public Func<object?, object?, string, string?>? EqualMessageGetter { get; set; }
	public Func<object?, object?, string, string?>? EqualMessageWithPropertyGetter { get; set; }
	public Func<object?, object?, string, string?>? NotEqualMessageGetter { get; set; }
	public Func<object?, object?, string, string?>? NotEqualMessageWithPropertyGetter { get; set; }
	public Func<object?, object?, string, string?>? MultiEqualMessageGetter { get; set; }
	public Func<object?, object?, string, string?>? MultiEqualMessageWithPropertyGetter { get; set; }
	public Func<object?, object?, string, string?>? MultiNotEqualMessageGetter { get; set; }
	public Func<object?, object?, string, string?>? MultiNotEqualMessageWithPropertyGetter { get; set; }
	public Func<object?, object?, string, string?>? NullMessageGetter { get; set; }
	public Func<object?, object?, string, string?>? NullMessageWithPropertyGetter { get; set; }
	public Func<object?, object?, string, string?>? NotNullMessageGetter { get; set; }
	public Func<object?, object?, string, string?>? NotNullMessageWithPropertyGetter { get; set; }
	public Func<object?, object?, string, string?>? LengthMessageGetter { get; set; }
	public Func<object?, object?, string, string?>? LengthMessageWithPropertyGetter { get; set; }
	public Func<object?, object?, string, string?>? PrecisionScaleMessageGetter { get; set; }
	public Func<object?, object?, string, string?>? PrecisionScaleMessageWithPropertyGetter { get; set; }
	public Func<object?, object?, string, string?>? RangeMessageGetter { get; set; }
	public Func<object?, object?, string, string?>? RangeMessageWithPropertyGetter { get; set; }
	public Func<object?, object?, string, string?>? RegExMessageGetter { get; set; }
	public Func<object?, object?, string, string?>? RegExMessageWithPropertyGetter { get; set; }
}
