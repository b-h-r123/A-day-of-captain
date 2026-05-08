using Godot;
using Godot.Collections;

public static class LocalizationExtension
{
    // 获取本地化字符串
    public static string Localized(this string key)
	{
		return Localization.Instance.GetLocalizedString(key);
	}
    public static object[] Localized(this object[] value)
    {
        object[] result = new object[value.Length];
        for (int i = 0; i < value.Length; i++){
            if (value[i] is string strValue)
                result[i] = strValue.Localized();
            else
                result[i] = value[i];
        }
        return result;
    }
    public static object[] Localized(this Variant varValue)
    {
        switch (varValue.VariantType)
        {
            case Variant.Type.String:
                return [varValue.ToString().Localized()];
            case Variant.Type.Array:
                return ((Array)varValue).GetArray().Localized();
            default:
                return ["[Unsupported type for localization]"];
        }
    }
}