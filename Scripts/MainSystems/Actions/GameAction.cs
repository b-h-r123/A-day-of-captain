using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 行动抽象类。
/// 
/// 如果某些行动需要复杂算法，可以继承这个抽象类。
/// 但大多数普通行动建议使用 ActionDefinition 数据化配置。
/// </summary>
public abstract class GameAction
{
    public string Id { get; }
    public string Name { get; }

    protected GameAction(string id, string name)
    {
        Id = id;
        Name = name;
    }

    public abstract bool CanExecute(GameContext context);
    public abstract ActionResult Execute(GameContext context);
}