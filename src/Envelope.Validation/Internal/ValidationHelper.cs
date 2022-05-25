﻿using System.Collections;

namespace Envelope.Validation.Internal;

internal class ValidationHelper
{
	public static T? Unbox<T>(object @object)
		where T : struct, IConvertible
	{
		if (@object == null)
			return null;

		return (T)Convert.ChangeType(@object, typeof(T));
	}

	public static bool IsDefault<T>(T? value)
		where T : class
		=> Equals(value, default);

	public static bool IsDefault<T>(T? nullableValue)
		where T : struct
		=> !nullableValue.HasValue;

	public static bool IsDefaultOrEmpty<T>(T? value)
		where T : class
	{
		switch (value)
		{
			case null:
			case string s when string.IsNullOrWhiteSpace(s):
			case ICollection c when c.Count == 0:
			case Array a when a.Length == 0:
			case IEnumerable e when !e.Cast<object>().Any():
				return true;
			default:
				return false;
		}
	}

	public static bool IsDefaultOrEmpty<T>(T? value, object? defaultValue)
	{
		if (Equals(value, defaultValue))
			return true;

		switch (value)
		{
			case null:
			case string s when string.IsNullOrWhiteSpace(s):
			case ICollection c when c.Count == 0:
			case Array a when a.Length == 0:
			case IEnumerable e when !e.Cast<object>().Any():
				return true;
			default:
				return false;
		}
	}
}
