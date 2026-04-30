using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 条件抽象类。
/// 
/// 条件只负责回答一个问题：“当前上下文是否满足条件？”
/// 它不修改状态，也不触发事件。
/// 
/// 事件、对话分支、成就、结局、任务判定都可以复用这套条件系统。
/// </summary>
public abstract class Condition
{
    public string Description { get; }

    protected Condition(string description)
    {
        Description = description;
    }

    public abstract bool Evaluate(GameContext context);
}