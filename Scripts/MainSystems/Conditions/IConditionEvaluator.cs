using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 条件求值器接口。
/// 
/// 把条件求值集中到这里，便于以后加入：
/// - 调试日志
/// - 条件失败原因
/// - 复杂 AND/OR/NOT 组合
/// - 可视化剧情编辑器
/// </summary>
public interface IConditionEvaluator
{
    bool Evaluate(Condition condition, GameContext context);
    bool EvaluateAll(IEnumerable<Condition> conditions, GameContext context);
}


//TODO:完善下面的条件求值器
/// <summary>
/// 默认条件求值器。
/// </summary>
public sealed class DefaultConditionEvaluator : IConditionEvaluator
{
    public bool Evaluate(Condition condition, GameContext context)
    {
        return condition.Evaluate(context);
    }

    public bool EvaluateAll(IEnumerable<Condition> conditions, GameContext context)
    {
        return conditions.All(condition => Evaluate(condition, context));
    }
}