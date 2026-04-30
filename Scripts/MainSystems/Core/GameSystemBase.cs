using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 游戏系统基类。
/// 
/// 抽象基类适合放通用依赖，例如 EventBus、EffectResolver、ConditionEvaluator。
/// 子类只关心自己的业务，不需要重复保存这些基础设施。
/// </summary>
public abstract class GameSystemBase : IGameSystem
{
    protected IEventBus EventBus { get; }
    protected IConditionEvaluator ConditionEvaluator { get; }
    protected IEffectResolver EffectResolver { get; }

    protected GameSystemBase(
        IEventBus eventBus,
        IConditionEvaluator conditionEvaluator,
        IEffectResolver effectResolver)
    {
        EventBus = eventBus;
        ConditionEvaluator = conditionEvaluator;
        EffectResolver = effectResolver;
    }

    public virtual void Initialize(GameState state)
    {
    }
}