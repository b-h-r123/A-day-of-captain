using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 一个最小可用的同步事件总线。
/// 
/// 示例里用同步调用，方便理解。
/// 如果后续需要动画、网络、异步 UI，可以替换成异步队列。
/// </summary>
public sealed class SimpleEventBus : IEventBus
{
    private readonly Dictionary<DomainEventType, List<Action<DomainEvent>>> _handlers = new();

    public void Subscribe(DomainEventType type, Action<DomainEvent> handler)
    {
        if (!_handlers.TryGetValue(type, out var handlers))
        {
            handlers = new List<Action<DomainEvent>>();
            _handlers[type] = handlers;
        }

        handlers.Add(handler);
    }

    public void Publish(DomainEvent domainEvent)
    {
        if (!_handlers.TryGetValue(domainEvent.Type, out var handlers))
        {
            return;
        }

        foreach (var handler in handlers)
        {
            handler(domainEvent);
        }
    }
}