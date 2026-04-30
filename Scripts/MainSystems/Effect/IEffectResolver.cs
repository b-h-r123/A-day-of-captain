using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 效果解析器接口。
/// 
/// 统一修改状态的好处：
/// 1. 所有数值变化有统一入口，便于记录日志。
/// 2. 以后做撤销、回放、存档差异会更简单。
/// 3. 可以在这里处理倍率、心情修正、健康惩罚等全局规则。
/// </summary>
public interface IEffectResolver
{
    void Apply(Effect effect, GameContext context);
    void ApplyAll(IEnumerable<Effect> effects, GameContext context);
}

//TODO:做这个效果解析器

/// <summary>
/// 默认效果解析器。
/// 
/// 示例中直接调用 Effect.Apply。
/// Apply() 只应用单个效果，不负责保存。
/// ApplyAll() 代表“一次结算边界”，会在所有效果应用完成后统一 SaveIfDirty()。
/// </summary>
public sealed class DefaultEffectResolver : IEffectResolver
{
    public void Apply(Effect effect, GameContext context)
    {
        effect.Apply(context);
    }

    public void ApplyAll(IEnumerable<Effect> effects, GameContext context)
    {
        foreach (var effect in effects)
        {
            Apply(effect, context);
        }

        context.State.SaveIfDirty();
    }
}