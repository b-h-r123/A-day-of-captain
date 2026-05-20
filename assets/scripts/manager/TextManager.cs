using Godot;
using Godot.Collections;
using System.Linq;

/// <summary>
/// 文本管理器
/// 
/// 负责：
/// - 根据卡牌效果生成描述文本
/// </summary>
public partial class TextManager : Node
{
	private const string TriggerKeySeparator = "::";
    #region 单例声明与初始化
    public static TextManager Instance { get; private set; }
    public override void _Ready()
    {
        Instance = this;
        GD.Print(BuildEffectDescriptionbyId(0));
    }
    #endregion

	/// <summary>
	/// 根据卡牌ID获取卡牌效果描述文本。
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	public static string GetCardDescriptionById(int id)
	{
		return BuildEffectDescriptionbyId(id);
	}
	private static string BuildEffectDescriptionbyId(int id)
	{
		string description = "";
		Array effects = DataManager.Instance.GetEffectsbyId(id);
		Array limits = DataManager.Instance.GetLimitsbyId(id);

		Dictionary t_effectIndex = GetEffectIndexByTrigger(effects);
		for (int i = 0; i < t_effectIndex.Count; i++)
		{
			string triggerKey = t_effectIndex.Keys.ToArray()[i].ToString();
			string trigger = triggerKey;

			int separatorIndex = triggerKey.IndexOf(TriggerKeySeparator);
			if (separatorIndex >= 0) trigger = triggerKey[..separatorIndex];

			Array effectList = t_effectIndex[triggerKey].AsGodotArray();
			Array val_trig = BuildValueTrig(effectList);

			string triggerDesc = BuildTriggerDescription(trigger, val_trig);

			description += triggerDesc + "，";

			string effectDesc = BuildEffectDescription(effectList);

			description += effectDesc;

			if(i < t_effectIndex.Count - 1) description += "；\n";
			else description += "。\n";

		}
		string limitDesc = BuildLimitDescription(limits);
		if (!string.IsNullOrEmpty(limitDesc)) description += limitDesc;

		return description;
	}
	#region 文本拼接黑箱
	private static Array BuildValueTrig(Array effectList)
		{
			Array val_trig = [];
			for (int j = 0; j < effectList.Count; j++)
			{
				Dictionary effectEntry = effectList[j].AsGodotDictionary();
				if (effectEntry.TryGetValue("value_trig", out var val))
				{
					val_trig = val.AsGodotArray();
					if (!(val_trig.Count == 0))
						break;
				}
			}
			return val_trig;
		}
	private static Dictionary GetEffectIndexByTrigger(Array effects)
	{
		Dictionary<string, Array<Dictionary>> index = [];
		for (int i = 0; i < effects.Count; i++)
		{
			Dictionary effectItem = effects[i].AsGodotDictionary();
			if (!effectItem.TryGetValue("trigger", out var triggerValue)) continue;
			string trigger = triggerValue.ToString();
			Array valueTrigArr = [];
			string groupKey = trigger;
			if (effectItem.TryGetValue("value_trig", out var valueTrigValue))
			{
				valueTrigArr = valueTrigValue.AsGodotArray();
				if (valueTrigArr.Count > 0)
				{
					string valueTrigKey = string.Join("|", valueTrigArr.Select(v => v.ToString()));
					groupKey = $"{trigger}{TriggerKeySeparator}{valueTrigKey}";
				}
			}
			if (!index.TryGetValue(groupKey, out var list))
			{
				list = [];
				index[groupKey] = list;
			}

			Dictionary effectEntry = [];
			if (effectItem.TryGetValue("effect", out var effectValue)) effectEntry["effect"] = effectValue;
			if (effectItem.TryGetValue("value_typ", out var typeValue)) effectEntry["value_typ"] = typeValue;
			if (effectItem.TryGetValue("num", out var numValue)) effectEntry["num"] = numValue;
			if (valueTrigArr.Count > 0) effectEntry["value_trig"] = valueTrigArr;

			list.Add(effectEntry);
		}
		return (Dictionary)index;
	}
	private static string BuildTriggerDescription(string trigger, Array val_trig)
	{
		string trig = trigger.Localized();
		if (val_trig.Count > 0)
		{
			for (int i = 0; i < val_trig.Count; i++)
			{
				string val = val_trig[i].ToString();
				switch (trigger)
				{
					case "on_event":
						trig = trig.Replace("{event}", val.Localized());
						GD.Print("Trigger has event: " + val.Localized());
						break;
					default:
						GD.PrintErr($"Unknown trigger value: {trigger}");
						break;
				}
			}
		}
		return trig;
	}
	private static string BuildEffectDescription(Array effectlist)
	{
		string effectDesc = "";
		for (int i = 0; i < effectlist.Count; i++)
		{
			string effectPiece = "";
			Dictionary effect = effectlist[i].AsGodotDictionary();
			if (!effect.TryGetValue("effect", out var effectTypeValue)) continue;
			if (!effect.TryGetValue("value_typ", out var valueTypValue)) valueTypValue = "";
			string effectType = effectTypeValue.ToString().Localized();
			Array valueTypArr = valueTypValue.AsGodotArray();

			string valueTyp = "";
			object[] num = effect["num"].GetArray();
			switch (effectTypeValue.ToString())
			{
				case "base_plus":
				case "multiply":
				case "activate":
				case "enable_sp_opts":
					valueTyp = valueTypArr.Count > 0 ? valueTypArr[0].ToString().Localized() : "";
					effectPiece += effectType.Replace("{value}", valueTyp);
					break;
				case "multi_plus":
					for (int j = 0; j < valueTypArr.Count; j++)
					{
						string valTyp = valueTypArr[j].ToString().Localized();
						valueTyp += valTyp;
						valueTyp +=(j == valueTypArr.Count - 3) ? "、" : 
									(j == valueTypArr.Count - 2) ? "和" : "";
					}
					effectPiece += effectType.Replace("{values}", valueTyp);
					break;
				default:
					effectPiece += $"[未知效果, type={effectTypeValue}]";
					break;
			}
			effectPiece = string.Format(effectPiece, num);
			effectDesc += effectPiece;
			if(i < effectlist.Count - 1)effectDesc += "，";
        }
		return effectDesc;
	}
	private static string BuildLimitDescription(Array limits)
	{
		if (limits.Count == 0) return "";
		string limitDesc = "";
		for (int i = 0; i < limits.Count; i++)
		{
			string limitPiece = limits[i].ToString().Localized();
			limitDesc += limitPiece;
			if(i < limits.Count - 1)limitDesc += "\n";
		}
		return limitDesc;
	}
	#endregion

}