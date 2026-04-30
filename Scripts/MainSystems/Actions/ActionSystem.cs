using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 行动系统。
/// 
/// 负责：
/// 1. 查询当前阶段可执行哪些行动。
/// 2. 检查行动条件。
/// 3. 生成行动结果。
/// 
/// 不负责：
/// 1. 直接扣体力。
/// 2. 直接修改课程成绩。
/// 3. 直接推进 DDL。
/// 4. 直接解锁成就。
/// </summary>
public sealed class ActionSystem : GameSystemBase
{
    private readonly Dictionary<string, ActionDefinition> _definitions = new();

    public ActionSystem(
        IEventBus eventBus,
        IConditionEvaluator conditionEvaluator,
        IEffectResolver effectResolver)
        : base(eventBus, conditionEvaluator, effectResolver)
    {
    }

    public void Register(ActionDefinition definition)
    {
        _definitions[definition.Id] = definition;
    }

    public IReadOnlyList<ActionDefinition> ListAvailableActions(GameContext context)
    {
        return _definitions.Values
            .Where(action => IsAvailableInCurrentStage(action, context))
            .Where(action => ConditionEvaluator.EvaluateAll(action.Conditions, context))
            .ToList();
    }

    public ActionResult Execute(string actionId, GameContext context)
    {
        var definition = _definitions[actionId];
        if (!ConditionEvaluator.EvaluateAll(definition.Conditions, context))
        {
            throw new InvalidOperationException($"Action '{actionId}' is not executable now.");
        }

        var result = new ActionResult
        {
            NextSceneId = definition.TargetSceneId
        };

        result.Effects.AddRange(definition.Effects);
        result.Events.Add(new DomainEvent(DomainEventType.ActionExecuted, context, definition.Id));

        return result;
    }

    private static bool IsAvailableInCurrentStage(ActionDefinition action, GameContext context)
    {
        var stageType = context.CurrentStage?.Type;
        return stageType == null || action.AvailableStages.Count == 0 || action.AvailableStages.Contains(stageType.Value);
    }
}