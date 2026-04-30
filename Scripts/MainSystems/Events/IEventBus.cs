using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 事件总线接口。
/// 
/// 事件总线让系统之间通过“发布/订阅”通信。
/// 例如 ActionSystem 发布 ActionExecuted 后：
/// - TaskSystem 可以推进 DDL。
/// - AchievementSystem 可以检查成就。
/// - EventSystem 可以尝试触发随机事件。
/// 
/// ActionSystem 不需要知道这些系统存在。
/// </summary>
public interface IEventBus
{
    void Subscribe(DomainEventType type, Action<DomainEvent> handler);
    void Publish(DomainEvent domainEvent);
}