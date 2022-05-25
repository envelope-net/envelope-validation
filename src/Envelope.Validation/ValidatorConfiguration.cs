using Microsoft.Extensions.Localization;

namespace Envelope.Validation;

public static class ValidatorConfiguration
{
	public static IStringLocalizer? Localizer { get; set; }
}
