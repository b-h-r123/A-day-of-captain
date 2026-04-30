using Godot;
using System;
using System.Collections.Generic;

public sealed partial class GameManager
{
    private readonly GameLoop _gameLoop;

    public GameMode Mode { get; private set; }

    public GameManager(GameLoop gameLoop, GameMode mode = GameMode.DetailedControl)
    {
        _gameLoop = gameLoop;
        Mode = mode;
    }
	
    /// 初始化游戏。
    /// 这里只做生命周期编排，具体数据初始化由 GameStateFactory 或各系统完成。
    public void InitializeGame()
    {
        _gameLoop.Initialize();
    }
	
    /// 开始一天。
    /// 具体逻辑例如抽心情、分配体力、触发 DayStarted，全部交给 GameLoop。
    public void StartDay()
    {
        _gameLoop.StartDay();
    }
	
    /// 结束一天。
    /// 这里不直接做结局判定；结局系统可以监听 DayEnded 或由 GameLoop 调用统一检查点。
    public void EndDay()
    {
        _gameLoop.EndDay();
    }
	
    /// 快速推进一周。
    /// 快速推进仍然应该复用同一套事件、条件、效果机制，只是 UI 选择更少。
    public void SkipWeek()
    {
        for (var i = 0; i < 7; i++)
        {
            _gameLoop.StartDay();
            _gameLoop.ResolveAutoDay();
            _gameLoop.EndDay();
        }
    }
}