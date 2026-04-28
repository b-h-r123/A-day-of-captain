using Godot;
using Godot.Collections;
public static class GodotCollectionExtensions
{
	// 获取字典某一key对应的字符串
	public static string GetString(this Dictionary dict, string key)
	{
		if (dict.TryGetValue(key, out var value))
		{
			return value.ToString();
		}
		return null;
	}
	public static string GetString(this Dictionary dict, string key, string defaultValue)
	{
		if (dict.TryGetValue(key, out var value))
		{
			return value.ToString();
		}
		return defaultValue;
	}
	
	// 将Godot数组转换为对象数组
	public static object[] GetArray(this Array array)
	{
		if (array == null)
		{
			return [];
		}

		object[] result = new object[array.Count];
		for (int i = 0; i < array.Count; i++)
		{
			result[i] = array[i].Obj;
		}
		return result;
	}
	public static object[] GetArray(this Variant varValue)
	{
		switch (varValue.VariantType)
		{
			case Variant.Type.Array:
			case Variant.Type.Dictionary:
				return varValue.AsGodotArray().GetArray();
			default:
				return [];
		}
	}
	
	// 在数组后输入一个数组，拼到原数组后
#region object[] GetSplicedArray(this arg1, arg2)方法的重载
	public static object[] GetSplicedArray(this object[] array, object[] arrayToAdd)
	{
		if (!(array == null) && !(arrayToAdd == null))
		{
			object[] result = new object[array.Length + arrayToAdd.Length];
			for (int i = 0; i < array.Length; i++)
				result[i] = array[i];
			for (int i = 0; i < arrayToAdd.Length; i++)
				result[array.Length + i] = arrayToAdd[i];
			return result;
		}
		return [];
	}
	public static object[] GetSplicedArray(this Array array, Array arrayToAdd)
	{
		return GetSplicedArray(array.GetArray(), arrayToAdd.GetArray());
	}
	public static object[] GetSplicedArray(this Array array, object[] arrayToAdd)
	{
		return GetSplicedArray(array.GetArray(), arrayToAdd);
	}
	public static object[] GetSplicedArray(this object[] array, Array arrayToAdd)
	{
		return GetSplicedArray(array, arrayToAdd.GetArray());
	}
    public static object[] GetSplicedArray(this Variant varValue, object[] arrayToAdd)
	{
		return GetSplicedArray(varValue.GetArray(), arrayToAdd);
	}
	public static object[] GetSplicedArray(this Variant varValue, Array arrayToAdd)
	{
		return GetSplicedArray(varValue, arrayToAdd.GetArray());
	}
	public static object[] GetSplicedArray(this Array array, Variant varValue)
	{
		return GetSplicedArray(array.GetArray(), varValue.GetArray());
	}
	public static object[] GetSplicedArray(this object[] array, Variant varValue)
	{
		return GetSplicedArray(array, varValue.GetArray());
	}
	public static object[] GetSplicedArray(this Variant varValue1, Variant varValue2)
	{
		return GetSplicedArray(varValue1.GetArray(), varValue2.GetArray());
	}

#endregion


}