using Godot;
using System;
using System.Collections.Generic;

/// 所有游戏系统的统一接口。
/// 例如 ActionSystem、EventSystem、TaskSystem、RelationshipSystem 都可以实现它。
/// 这样 GameLoop 可以用统一方式初始化、刷新、关闭系统。
public interface IGameSystem
{
    void Initialize(GameState state);
}