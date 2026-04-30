using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 效果抽象类。
/// 
/// 效果表示“要对世界产生的修改”，例如：
/// - 扣除体力
/// - 增加课程分
/// - 增加 NPC 好感
/// - 添加一个 DDL 任务
/// - 切换场景
/// 
/// 重要边界：
/// 行动、事件、对话不要直接修改 GameState，而是产出 Effect。
/// EffectResolver 统一应用 Effect。
/// </summary>
public abstract class Effect
{
    public string Description { get; }

    protected Effect(string description)
    {
        Description = description;
    }

    public abstract void Apply(GameContext context);
}