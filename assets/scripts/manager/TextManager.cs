using Godot;
using Godot.Collections;
public partial class TextManager : Node

{
    #region 单例声明与初始化
    public static TextManager Instance { get; private set; }
    public override void _Ready()
    {
        Instance = this;
        GD.Print(BuildEffectDescriptionbyId(0));
    }
    #endregion

	// 根据卡牌的效果列表拼接一个描述字符串
	public string BuildEffectDescriptionbyId(int id)
	{
		string description = "";
		Array effects = DataManager.Instance.GetEffectsbyId(id);
		if (effects.Count > 0)
		{
			foreach (var effect in effects)
			{
				description += MixEffectDescbyFormat(effect.AsGodotDictionary()) + "\n";
			}
		}
		return description;
	}
	private static string MixEffectDescbyFormat(Dictionary effect)
	{
		string result;
		string condition = "";
		string effectDesc = "";
        if (effect.TryGetValue("condition", out var _condition)
        && effect.TryGetValue("value_c", out var value_c))
            condition += string.Format(_condition.ToString().Localized(),value_c.GetArray());
        
        if (effect.TryGetValue("effect", out var _effect)
        && effect.TryGetValue("value_e", out var value_e)
        && effect.TryGetValue("value_type", out var value_type))
            effectDesc += string.Format(_effect.ToString().Localized(),value_e.GetSplicedArray(value_type.Localized()));
        result = condition + "," + effectDesc;
        return result;
	}

}