using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 系统注册表。
/// 
/// 它解决两个问题：
/// 1. GameLoop 不需要手写一堆字段保存所有系统。
/// 2. 系统之间不需要互相 new 或互相持有引用。
/// 
/// 真实项目里可以用依赖注入容器代替。
/// </summary>
public sealed class SystemRegistry
{
    private readonly Dictionary<Type, IGameSystem> _systems = new();

    public IEnumerable<IGameSystem> AllSystems => _systems.Values;

    public void Add<TSystem>(TSystem system) where TSystem : IGameSystem
    {
        _systems[typeof(TSystem)] = system;
    }

    public TSystem Get<TSystem>() where TSystem : IGameSystem
    {
        return (TSystem)_systems[typeof(TSystem)];
    }
}