using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 领域事件。
/// 
/// 领域事件表示“世界里已经发生的事实”，例如：
/// - 玩家执行了一个行动
/// - 课程成绩变化
/// - NPC 好感度变化
/// - 任务到期
/// 
/// 注意：
/// 事件不是命令。事件描述已经发生的事，不负责要求别人做什么。
/// 订阅者可以根据事件决定是否响应。
/// </summary>
public sealed class DomainEvent
{
    public DomainEventType Type { get; }
    public GameContext Context { get; }
    public string? SourceId { get; }
    public object? Payload { get; }

    public DomainEvent(
        DomainEventType type,
        GameContext context,
        string? sourceId = null,
        object? payload = null)
    {
        Type = type;
        Context = context;
        SourceId = sourceId;
        Payload = payload;
    }
}