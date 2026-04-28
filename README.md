# A day of captain

## Card.cs 功能说明

位置：assets/scripts/cards/Card.cs

### 类：Card

- 继承自 Godot 的 Control，用于卡牌 UI 的展示与拖拽交互。

### 主要函数

- _Ready()
  - 获取并缓存节点引用（图标、名称、等级、属性）。
  - 调用 Initialize(_cardId) 初始化卡牌显示。

- _Process(double delta)
  - 基于当前状态更新位置。
  - 基础状态下跟随目标节点（弹性移动）。
  - 拖拽状态下跟随鼠标位置（阻尼插值）。

- Initialize(int id)
  - 从 DataManager 中读取卡牌数据并写入本地字段。
  - 设置卡牌名称文本与图标贴图。
  - 当 DataManager 或卡牌数据缺失时输出错误日志。

- ElasticMove(float delta, Vector2 targetPosition, int sensitivity)
  - 弹簧式位移计算，用于跟随目标位置。

- OnButtonDown()
  - 切换为拖拽状态，并输出调试日志。

- OnButtonUp()
  - 切换为基础状态，并输出调试日志。

- SetFollowTarget(Control target)
  - 设置跟随目标，用于在基础状态下的弹性跟随。

- Swap()
  - 卡牌翻转的接口（目前为空实现）。

- On_Play()
  - 触发“打出”效果的接口（可在子类中重写）。

- On_Place()
  - 触发“放置”效果的接口（可在子类中重写）。

## Manager 接口说明

### DataManager

位置：assets/scripts/manager/DataManager.cs

- `DataManager.Instance`
  - 单例引用，需在 Godot AutoLoad 中配置 DataManager。
  - 使用方法：在脚本中直接访问 `DataManager.Instance`。

- `LoadJsonFile(string filePath)`
  - 读取并解析 JSON 文件，返回根字典。
  - 使用方法：`var dict = DataManager.LoadJsonFile("res://path/file.json");`

- `GetCardbyId(int id)`
  - 根据卡牌 id 获取对应的卡牌字典，未找到返回 null。
  - 使用方法：`var card = DataManager.Instance.GetCardbyId(1);`

- `GetEffectsbyId(int id)`
  - 根据卡牌 id 获取效果数组，未找到返回空数组。
  - 使用方法：`var effects = DataManager.Instance.GetEffectsbyId(1);`

- `GetIdsbyType(Array<string> type)` / `GetIdsbyType(string type)`
  - 根据卡牌类型筛选并返回 id 列表。
  - 使用方法：`var ids = DataManager.Instance.GetIdsbyType("attack");`

- `GetIdsbyAttribute(string attribute)`
  - 根据卡牌属性筛选并返回 id 列表。
  - 使用方法：`var ids = DataManager.Instance.GetIdsbyAttribute("fire");`

- `GetIdsbyLevel(int level)`
  - 根据卡牌等级筛选并返回 id 列表。
  - 使用方法：`var ids = DataManager.Instance.GetIdsbyLevel(2);`

### TextManager

位置：assets/scripts/manager/TextManager.cs

- `TextManager.Instance`
  - 单例引用，需在 Godot AutoLoad 中配置 TextManager。
  - 使用方法：在脚本中直接访问 `TextManager.Instance`。

- `BuildEffectDescriptionbyId(int id)`
  - 将卡牌效果列表拼接为描述字符串。
  - 使用方法：`var desc = TextManager.Instance.BuildEffectDescriptionbyId(1);`

## Extension 接口说明

### GodotCollectionExtensions

位置：assets/scripts/extensions/GodotCollectionExtensions.cs

- `GetString(this Dictionary dict, string key)` / `GetString(this Dictionary dict, string key, string defaultValue)`
  - 从字典中读取字符串值。
  - 使用方法：`var name = dict.GetString("name", "unknown");`

- `GetArray(this Array array)` / `GetArray(this Variant varValue)`
  - 将 Godot Array 或 Variant 转为 object[]。
  - 使用方法：`var args = effects.GetArray();`

- `GetSplicedArray(...)`（多重重载）
  - 将数组与数组拼接为新的 object[]。
  - 使用方法：`var args = a.GetSplicedArray(b);`

### LocalizationExtension

位置：assets/scripts/extensions/LocalizationExtension.cs

- `Localized(this string key)`
  - 将 key 转为本地化字符串。
  - 使用方法：`var text = "card_name".Localized();`

- `Localized(this object[] value)`
  - 将 object[] 内的字符串进行本地化处理。
  - 使用方法：`var args = values.Localized();`

- `Localized(this Variant varValue)`
  - 将 Variant 中的字符串或数组进行本地化处理。
  - 使用方法：`var args = variant.Localized();`
