using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Common
{
	public static T GetLast<T>(this List<T> list)
	{
		return list[list.Count - 1];
	}

	public static T PopLast<T>(this List<T> list)
	{
		T last = GetLast(list);
		list.RemoveAt(list.Count - 1);

		return last;
	}

	public static void Append<T>(this List<T> list, List<T> target)
	{
		list.InsertRange(list.Count, target);
	}

	public static bool Empty<T>(this List<T> list)
	{
		return list.Count == 0;
	}

	public static int GetEnumCount<T>()
	{
		return System.Enum.GetNames(typeof(T)).Length;
	}

	public static bool IsEmpty<T>(this IEnumerable<T> @this)
	{
		return !@this.Any();
	}

	public static bool IsNotEmpty<T>(this IEnumerable<T> @this)
	{
		return @this.Any();
	}

	public static bool IsNotNull<T>(this IEnumerable<T> @this)
	{
		return @this != null;
	}

	public static bool IsNotNullOrEmpty<T>(this IEnumerable<T> @this)
	{
		return @this != null && @this.Any();
	}

	public static T GetLast<T>(this T[] array)
	{
		return array[array.Length - 1];
	}

	public static bool IsNullOrEmpty<T>(this IEnumerable<T> @this)
	{
		return @this == null || !@this.Any();
	}

	public static void SetVisible(this UnityEngine.EventSystems.UIBehaviour ui, bool visible)
	{
		if (ui == null)
			return;

		ui.gameObject.SetActive(visible);
	}

	public static T GetSome<T>(this HashSet<T> set)
	{
		var enumerator = set.GetEnumerator();
		enumerator.MoveNext();

		return enumerator.Current;
	}

	public static System.Func<int, int, int> RandomRange = UnityEngine.Random.Range;

	static System.Collections.IEnumerator WaitForSecondsCo(float seconds)
	{
		yield return new UnityEngine.WaitForSeconds(seconds);
	}

	public static T RandomPick<T>(this List<T> list)
	{
		var index = RandomRange(0, list.Count);
		return list[index];
	}

	public static Vector3 Clone(this Vector3 ori)
	{
		return new Vector3(ori.x, ori.y, ori.z);
	}

	// use UTC
	public static int ToUnixTimestamp(this DateTime value)
	{
		return (int)Math.Truncate((value.ToUniversalTime().Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
	}

	// use UTC
	public static DateTime FromUnixTimestamp(this DateTime value, int unixtimestamp)
	{
		return value.AddSeconds(value.ToUnixTimestamp() - unixtimestamp);
	}

	// use UTC
	public static int UnixTimestamp(this DateTime ignored)
	{
		return (int)Math.Truncate((DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
	}

	public static int UnixTimestamp()
	{
		return (int)Math.Truncate((DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
	}

	private static readonly char[] FlagDelimiter = new[] { ',' };

	public static bool TryParseEnum<TEnum>(string value, out TEnum result) where TEnum : struct
	{
		if (string.IsNullOrEmpty(value))
		{
			result = default(TEnum);
			return false;
		}

		var enumType = typeof(TEnum);

		if (!enumType.IsEnum)
			throw new ArgumentException(string.Format("Type '{0}' is not an enum", enumType.FullName));


		result = default(TEnum);

		// Try to parse the value directly 
		if (Enum.IsDefined(enumType, value))
		{
			result = (TEnum)Enum.Parse(enumType, value);
			return true;
		}

		// Get some info on enum
		var enumValues = Enum.GetValues(enumType);
		if (enumValues.Length == 0)
			return false;  // probably can't happen as you cant define empty enum?
		var enumTypeCode = Type.GetTypeCode(enumValues.GetValue(0).GetType());

		// Try to parse it as a flag 
		if (value.IndexOf(',') != -1)
		{
			if (!Attribute.IsDefined(enumType, typeof(FlagsAttribute)))
				return false;  // value has flags but enum is not flags

			// todo: cache this for efficiency
			var enumInfo = new Dictionary<string, object>();
			var enumNames = Enum.GetNames(enumType);
			for (var i = 0; i < enumNames.Length; i++)
				enumInfo.Add(enumNames[i], enumValues.GetValue(i));

			ulong retVal = 0;
			foreach (var name in value.Split(FlagDelimiter))
			{
				var trimmedName = name.Trim();
				if (!enumInfo.ContainsKey(trimmedName))
					return false;   // Enum has no such flag

				var enumValueObject = enumInfo[trimmedName];
				ulong enumValueLong;
				switch (enumTypeCode)
				{
					case TypeCode.Byte:
						enumValueLong = (byte)enumValueObject;
						break;
					case TypeCode.SByte:
						enumValueLong = (byte)((sbyte)enumValueObject);
						break;
					case TypeCode.Int16:
						enumValueLong = (ushort)((short)enumValueObject);
						break;
					case TypeCode.Int32:
						enumValueLong = (uint)((int)enumValueObject);
						break;
					case TypeCode.Int64:
						enumValueLong = (ulong)((long)enumValueObject);
						break;
					case TypeCode.UInt16:
						enumValueLong = (ushort)enumValueObject;
						break;
					case TypeCode.UInt32:
						enumValueLong = (uint)enumValueObject;
						break;
					case TypeCode.UInt64:
						enumValueLong = (ulong)enumValueObject;
						break;
					default:
						return false;   // should never happen
				}
				retVal |= enumValueLong;
			}
			result = (TEnum)Enum.ToObject(enumType, retVal);
			return true;
		}

		// the value may be a number, so parse it directly
		switch (enumTypeCode)
		{
			case TypeCode.SByte:
				sbyte sb;
				if (!SByte.TryParse(value, out sb))
					return false;
				result = (TEnum)Enum.ToObject(enumType, sb);
				break;
			case TypeCode.Byte:
				byte b;
				if (!Byte.TryParse(value, out b))
					return false;
				result = (TEnum)Enum.ToObject(enumType, b);
				break;
			case TypeCode.Int16:
				short i16;
				if (!Int16.TryParse(value, out i16))
					return false;
				result = (TEnum)Enum.ToObject(enumType, i16);
				break;
			case TypeCode.UInt16:
				ushort u16;
				if (!UInt16.TryParse(value, out u16))
					return false;
				result = (TEnum)Enum.ToObject(enumType, u16);
				break;
			case TypeCode.Int32:
				int i32;
				if (!Int32.TryParse(value, out i32))
					return false;
				result = (TEnum)Enum.ToObject(enumType, i32);
				break;
			case TypeCode.UInt32:
				uint u32;
				if (!UInt32.TryParse(value, out u32))
					return false;
				result = (TEnum)Enum.ToObject(enumType, u32);
				break;
			case TypeCode.Int64:
				long i64;
				if (!Int64.TryParse(value, out i64))
					return false;
				result = (TEnum)Enum.ToObject(enumType, i64);
				break;
			case TypeCode.UInt64:
				ulong u64;
				if (!UInt64.TryParse(value, out u64))
					return false;
				result = (TEnum)Enum.ToObject(enumType, u64);
				break;
			default:
				return false; // should never happen
		}

		return true;
	}
}
