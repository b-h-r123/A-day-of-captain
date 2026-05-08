using Godot;
using Godot.Collections;
public partial class DataManager : Node
{
	// 卡牌数据的根节点，加载后会包含整个JSON文件的内容
	[Export] private Dictionary _cardRoot;

	#region 单例声明与初始化
	public static DataManager Instance { get; private set; }
	public override void _Ready()
	{
		Instance = this;
		_cardRoot = LoadJsonFile("res://assets/data/cards/cards.json") ?? [];
	}
	#endregion

	#region JSON加载方法
	public static Dictionary LoadJsonFile(string filePath)
	{
		if (FileAccess.FileExists(filePath))
		{
			FileAccess file = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);
			var jsonContent = Json.ParseString(file.GetAsText()).As<Dictionary>();

			if (jsonContent is Dictionary)
			{
				return jsonContent;
			}
		}
		else
		{
			GD.PrintErr($"File not found: {filePath}");
		}
		return null;
	}
	#endregion

	#region 卡牌数据查询方法
	// 根据卡牌ID返回对应的字典，如果未找到则返回null
	public Dictionary GetCardbyId(int id)
	{
		if(_cardRoot.TryGetValue("cards", out var array))
		{
			foreach (Dictionary item in (Array)array)
			{
				if (item.TryGetValue("id", out var itemId)
					&& (int)itemId == id)
				{
					return item;
				}
			}
			GD.PrintErr($"Card with ID {id} not found.");
		}
		return null;
	}
	public Array GetEffectsbyId(int id)
	{
		var card = GetCardbyId(id);
		if (card != null && card.TryGetValue("effects", out var effectList))
			return effectList.AsGodotArray();

		GD.PrintErr($"No effects found for card with ID {id}.");
		return [];
	}

	// 根据不同key值返回对应的卡牌ID数组，如果未找到则返回一个空数组
	public Array<int> GetIdsbyType(Array<string> type)
	{
		var ids = new Array<int>();
		if (_cardRoot.TryGetValue("cards", out var array))
		{
			foreach (Dictionary item in (Array)array)
			{
				if (item.TryGetValue("type", out var itemType))
				{
					foreach (var t in (Array)itemType)
					{
						if (type.Contains((string)t) 
							&& item.TryGetValue("id", out var itemId))
						{
							ids.Add((int)itemId);
						}
						break;
					}
				}
			}
		}
		if (ids.Count == 0)
		{
			GD.PrintErr($"No cards of type {type} found.");
		}
		return ids;
    }
	public Array<int> GetIdsbyType(string type)
	{
		return GetIdsbyType(new Array<string>() { type });
	}
	public Array<int> GetIdsbyAttribute(string attribute)
	{
		var ids = new Array<int>();
		if (_cardRoot.TryGetValue("cards", out var array))
		{
			foreach (Dictionary item in (Array)array)
			{
				if (item.TryGetValue("attribute", out var attrResult) 
					&& (string)attrResult == attribute)
				{
					if (item.TryGetValue("id", out var itemId))
					{
						ids.Add((int)itemId);
					}
				}
			}
		}
		if (ids.Count == 0)
		{
			GD.PrintErr($"No cards with attribute {attribute} found.");
		}
		return ids;
	}
	public Array<int> GetIdsbyLevel(int level)
	{
		var ids = new Array<int>();
		if (_cardRoot.TryGetValue("cards", out var array))
		{
			foreach (Dictionary item in (Array)array)
			{
				if (item.TryGetValue("level", out var itemLevel) 
					&& (int)itemLevel == level)
				{
					if (item.TryGetValue("id", out var itemId))
					{
						ids.Add((int)itemId);
					}
				}
			}
		}
		if (ids.Count == 0)
		{
			GD.PrintErr($"No cards with level {level} found.");
		}
		return ids;
	}
	#endregion

}