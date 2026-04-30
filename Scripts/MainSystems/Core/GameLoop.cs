using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 游戏循环控制器。
/// 
/// 这是“一天是一个回合”的核心编排层：
/// - 回合开始：抽心情、分配体力、发布 DayStarted。
/// - 阶段开始：加载可选主行动、发布 StageStarted。
/// - 行动执行：调用 ActionSystem，拿到 ActionResult。
/// - 结果结算：把 ActionResult 交给 EffectResolver 和 EventBus。
/// - 回合结束：发布 DayEnded，给任务/结局/成就系统检查机会。
/// 
/// 注意：
/// GameLoop 可以调用各系统接口，但各系统之间不应该互相硬调用。
/// </summary>
public sealed class GameLoop
{
    private readonly GameState _state;
    private readonly SystemRegistry _systems;
    private readonly IEventBus _eventBus;
    private readonly IEffectResolver _effectResolver;

    public GameLoop(
        GameState state,
        SystemRegistry systems,
        IEventBus eventBus,
        IEffectResolver effectResolver)
    {
        _state = state;
        _systems = systems;
        _eventBus = eventBus;
        _effectResolver = effectResolver;
    }

    public void Initialize()
    {
        foreach (var system in _systems.AllSystems)
        {
            system.Initialize(_state);
        }
    }

    public void StartDay()
    {
        _state.CurrentDay = new DayRound(DateTime.Today, _state.SelectedFocus);
        _eventBus.Publish(new DomainEvent(DomainEventType.DayStarted, GameContext.FromState(_state)));
    }

    public void EnterStage(StageType stageType)
    {
        _state.CurrentStage = new Stage(stageType);
        _eventBus.Publish(new DomainEvent(DomainEventType.StageStarted, GameContext.FromState(_state)));
    }

    public void ExecuteAction(string actionId)
    {
        var actionSystem = _systems.Get<ActionSystem>();
        var result = actionSystem.Execute(actionId, GameContext.FromState(_state));

        // 行动系统只返回“这次行动造成了什么成本、效果、事件”，不直接改 GameState。
        _effectResolver.ApplyAll(result.Effects, GameContext.FromState(_state));

        foreach (var domainEvent in result.Events)
        {
            _eventBus.Publish(domainEvent);
        }
    }

    public void ResolveAutoDay()
    {
        // 示例留空。
        // 快速跳过时，可以由 AI/规则系统为每个阶段自动选择行动。
    }

    public void EndDay()
    {
        _eventBus.Publish(new DomainEvent(DomainEventType.DayEnded, GameContext.FromState(_state)));
    }
}