using Godot;
using System;
using System.Collections.Generic;

//提供当前世界状态的读取入口
//把当前的状态包起来，让系统统一去访问
public sealed class GameContext
{
    public GameState State { get; }
    
    public Stage? CurrentStage => State.CurrentStage;

    public GameContext(GameState state)
    {
        State = state;
    }
    
    public static GameContext FromState(GameState state)
    {
        return new GameContext(state);
    }
}
