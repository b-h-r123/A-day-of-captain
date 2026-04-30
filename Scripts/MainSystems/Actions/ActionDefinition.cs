using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 行动定义。
/// 
/// 把行动做成“数据定义”，而不是每个行动都写一个巨大类。
/// 例如：
/// - id: life.shopping
/// - name: 购物
/// - staminaCost: 20
/// - targetSceneId: mall
/// - effects: 扣钱、增加心情、可能获得道具
/// 
/// ActionSystem 解释这些定义，执行后返回 ActionResult。
/// </summary>
public sealed class ActionDefinition
{
    public string Id { get; }
    public string Name { get; }
    public MainActionType? MainType { get; }
    public IReadOnlyList<StageType> AvailableStages { get; }
    public int StaminaCost { get; }
    public string? TargetSceneId { get; }
    public IReadOnlyList<Condition> Conditions { get; }
    public IReadOnlyList<Effect> Effects { get; }

    public ActionDefinition(
        string id,
        string name,
        MainActionType? mainType,
        IEnumerable<StageType> availableStages,
        int staminaCost,
        string? targetSceneId,
        IEnumerable<Condition> conditions,
        IEnumerable<Effect> effects)
    {
        Id = id;
        Name = name;
        MainType = mainType;
        AvailableStages = availableStages.ToList();
        StaminaCost = staminaCost;
        TargetSceneId = targetSceneId;
        Conditions = conditions.ToList();
        Effects = effects.ToList();
    }
}