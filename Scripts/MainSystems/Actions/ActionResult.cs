using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 行动结果。
/// 
/// 行动执行后不直接修改世界，而是返回：
/// - Effects：需要应用的状态变化。
/// - Events：需要发布的领域事件。
/// - NextSceneId：可能切换到的新场景。
/// </summary>
public sealed class ActionResult
{
    public List<Effect> Effects { get; } = new();
    public List<DomainEvent> Events { get; } = new();
    public string? NextSceneId { get; set; }
}