using Godot;
using Godot.Collections;
enum Locale
{
    zh_CN,
}

public partial class Localization : Node

{
    [Export] private Dictionary _localizationRoot;

    #region 单例声明与初始化
    public static Localization Instance { get; private set; }
    public override void _Ready()
    {
        Instance = this;
        SetLocale("zh_CN");
    }
    #endregion
   
    #region 接口
    // 设置当前语言环境，加载对应的本地化JSON文件（目前就一个）
    public void SetLocale(string locale)
    {
        string filePath = $"res://assets/data/localizations/{locale}.json";
        var newLocalization = DataManager.LoadJsonFile(filePath);
        if (newLocalization != null)
        {
            _localizationRoot = newLocalization;
            GD.Print($"Locale set to {locale}");
        }
        else
        {
            GD.PrintErr($"Failed to load localization for {locale}");
        }
    }
    
    // 根据键名获取本地化字符串，如果未找到则返回一个提示字符串
    public string GetLocalizedString(string key)
    {
        if (!_localizationRoot.TryGetValue(key, out var value))
        {
            foreach (var _value in _localizationRoot.Values)
            {
                string result = GetLocalizedString(_value.AsGodotDictionary(),key);
                if (!result.StartsWith("[Missing localization"))
                    return result;
            }
        }
        else return value.ToString();
        return $"[Missing localization for {key}]";
    }
    #endregion
   
    #region  私有方法
    // 递归搜索嵌套的字典以找到对应的本地化字符串
    private static string GetLocalizedString(Dictionary item, string key)
    {
        if(item.Values.Count > 0)
            if (!item.TryGetValue(key, out var value))
            {

                foreach (var _value in item.Values)
                {
                    string result = GetLocalizedString(_value.AsGodotDictionary(),key);
                    if (!result.StartsWith("[Missing localization"))
                        return result;
                }
            }
            else return value.ToString();
        return $"[Missing localization for {key}]";
    }
    #endregion
}