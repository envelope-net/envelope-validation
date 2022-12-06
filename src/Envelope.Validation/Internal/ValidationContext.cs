namespace Envelope.Validation.Internal;

internal class ValidationContext
{
	public Dictionary<int, int> Indexes { get; } //Dictionary<Depth, Index>

	public ValidationContext(Dictionary<int, int>? indexes)
	{
		Indexes = indexes ?? new Dictionary<int, int>();
	}
}

internal class ValidationContext<T> : ValidationContext
{
	//private IObjectPath? _objectPath;

	public T? InstanceToValidate { get; }

	public ValidationContext(T? instanceToValidate, ValidationContext? parent)
		: base(parent?.Indexes)
	{
		InstanceToValidate = instanceToValidate;
	}

	//public ValidationContext<T> SetObjectPath(IObjectPath objectPath)
	//{
	//	_objectPath = objectPath ?? throw new ArgumentNullException(nameof(objectPath));

	//	if (string.IsNullOrWhiteSpace(_objectPath.PropertyName))
	//		throw new InvalidOperationException($"{nameof(_objectPath)}.{nameof(_objectPath.PropertyName)} == null");

	//	return this;
	//}

	//public IObjectPath GetObjectPath()
	//	=> _objectPath ?? throw new InvalidOperationException($"{nameof(_objectPath)} == null");
}

internal class ValidationContext<T, TProperty> : ValidationContext<T>
{
	public TProperty? ValueToValidate { get; }

	public ValidationContext(T? instanceToValidate, TProperty? value, ValidationContext? parent)
		: base(instanceToValidate, parent)
	{
		ValueToValidate = value;
	}

	//public new ValidationContext<T,TProperty> SetObjectPath(IObjectPath objectPath)
	//{
	//	base.SetObjectPath(objectPath);
	//	return this;
	//}
}
