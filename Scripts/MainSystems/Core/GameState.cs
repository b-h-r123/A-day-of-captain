using Godot;
using System;
using System.Collections.Generic;



/// <summary>
/// 游戏状态类。
/// 这个类用于存放游戏完整状态，相当于存档本体。
/// 
/// 设计原则：
/// 1. GameState 只负责保存“当前世界是什么样子”，不负责推进流程。
/// 2. 状态修改应该尽量由 EffectResolver 统一执行。
/// 3. GameState 的属性 setter 只调用 SetField 标记脏数据，不直接 Save()。
/// 4. 一次行动或一次事件结算完成后，再调用 SaveIfDirty() 统一保存。
/// 
/// 注意：
/// 这里不写完整属性列表，只写一个 Player 示例。
/// 之后需要添加体力、健康、课程、NPC、任务等状态时，照着这个示例写即可。
/// </summary>
public sealed partial class GameState : AutoSaveModel
{
    // 示例：把玩家资料对象放进 GameState。
    private PlayerProfile _player = new PlayerProfile();
    private DayRound? _currentDay;
    private Stage? _currentStage;
    private DailyFocus _selectedFocus = DailyFocus.Balanced;
    /// <summary>
    /// 玩家资料。
    /// 
    /// 写法说明：
    /// 1. 私有字段用 _player 保存真实数据。
    /// 2. 公开属性 Player 用于 JSON 序列化和外部访问。
    /// 3. setter 中调用 SetField，不直接调用 Save。
    /// 4. 如果替换整个 Player 对象，会标记 GameState 为脏数据。
    /// 
    /// 注意：
    /// 如果只是修改 Player 内部属性，例如 Player.Name = "xxx"，
    /// 这个 setter 不会被调用，因此不会自动标记脏数据。
    /// 这种情况有两种处理方式：
    /// 1. 修改后手动调用 MarkChanged()。
    /// 2. 更推荐：通过 Effect 修改玩家资料，并由 EffectResolver 统一保存。
    /// </summary>
    public PlayerProfile Player
    {
        get => _player;
        set => SetField<PlayerProfile>(ref _player, value);
    }
    public DayRound? CurrentDay
    {
        get => _currentDay;
        set => SetField(ref _currentDay, value);
    }

    public Stage? CurrentStage
    {
        get => _currentStage;
        set => SetField(ref _currentStage, value);
    }
    
    public DailyFocus SelectedFocus
    {
        get => _selectedFocus;
        set => SetField(ref _selectedFocus, value);
    }

    /*
    // 添加其他属性时，照这个格式写：

    private SomeState _someState = new SomeState();

    public SomeState SomeState
    {
        get => _someState;
        set => SetField(ref _someState, value);
    }

    // 如果是集合，也可以这样写：

    private List<SomeItem> _items = new List<SomeItem>();

    public List<SomeItem> Items
    {
        get => _items;
        set => SetField(ref _items, value);
    }

    */
}
// TODO：以下数据暂存在这里，以便过编译，后期需要把这些定义换到专门定义这些属性的文件夹里
/// <summary>
/// 玩家资料示例类。
/// 
/// 这里只放示例字段，实际项目中可以继续扩展。
/// 如果你希望 PlayerProfile 内部属性变化也能自动通知 GameState，
/// 可以以后再做“可观察对象”或统一通过 Effect 修改。
/// 目前为了结构简单，推荐先由 EffectResolver 统一保存 GameState。
/// </summary>
public sealed class PlayerProfile
{
    public string Name { get; set; } = "unnamed";
    public int Age { get; set; } = 19;
    public string PreviousStationMaster { get; set; } = "nim";
}
/// <summary>
/// 每日回合数据。
/// 
/// 它保存当天日期、玩家选择的偏向，以及是否已经抽取心情。
/// 不建议在这里写复杂推进逻辑，推进逻辑属于 GameLoop。
/// </summary>
public sealed class DayRound
{
    public DateTime Date { get; }
    public DailyFocus Focus { get; }
    public bool MoodDrawn { get; set; }

    public DayRound(DateTime date, DailyFocus focus)
    {
        Date = date;
        Focus = focus;
    }
}

/// <summary>
/// 阶段数据。
/// 
/// forcedBySchedule 表示这个阶段是否被课表强制为上课。
/// sharedNightStamina 表示夜晚和深夜共用精力条。
/// </summary>
public sealed class Stage
{
    public StageType Type { get; }
    public bool ForcedBySchedule { get; set; }
    public bool SharedNightStamina { get; set; }

    public Stage(StageType type)
    {
        Type = type;
        SharedNightStamina = type == StageType.Evening || type == StageType.LateNight;
    }
}



// TODO:按照示例添加GameState类中的属性，可以在本文件中,GameState的外部声明类或者变量;
