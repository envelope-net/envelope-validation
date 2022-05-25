namespace Envelope.Validation;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public sealed class ValidatorRegisterAttribute : Attribute
{
	public Type[] RegisteredTypes { get; }

	public ValidatorRegisterAttribute(params Type[] types)
	{
		if (types == null)
			throw new ArgumentNullException(nameof(types));

		if (types.Length == 0)
			throw new ArgumentException("No type set.", nameof(types));

		RegisteredTypes = types;
	}
}
