using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Godot;

/// <summary>
/// 持久化模型基类。
/// 继承此类的子类将获得以下能力：
/// 1. 调用 Load() 可从文件加载数据，并覆盖当前实例的公开属性。
/// 2. 调用 Save() 可将当前实例的公开属性序列化到文件。
/// 3. 子类使用 SetField 辅助方法，可在属性被修改时自动标记为“脏数据”。
/// 4. 调用 SaveIfDirty() 可在对象发生过变化时保存，避免每次属性修改都立刻写文件。
/// 
/// 注意：
/// 这个类不建议在构造函数中自动 Load()。
/// 原因是基类构造函数执行时，子类字段初始化还没有完全结束，过早加载可能被子类默认值覆盖。
/// 推荐做法是：先 new GameState()，再手动调用 Load()。
/// </summary>
public abstract class AutoSaveModel
{
    // 存储文件的路径，子类可以在构造函数中指定，也可以使用默认规则。
    private readonly string _filePath;

    // 标记当前对象是否发生过变化。
    // SetField 修改属性后只会把它设为 true，不会立刻保存。
    private bool _isDirty;

    // 标记当前是否正在加载。
    // Load() 内部会通过反射设置属性，如果不加这个标记，SetField 可能把“加载过程”误判成“玩家修改”。
    private bool _isLoading;

    /// <summary>
    /// 当前对象是否发生过未保存的变化。
    /// </summary>
    [JsonIgnore]
    public bool IsDirty => _isDirty;

    /// <summary>
    /// 当前对象对应的存档路径。
    /// 这个属性只用于调试和日志，不参与 JSON 存档。
    /// </summary>
    [JsonIgnore]
    public string FilePath => _filePath;

    /// <param name="filePath">持久化文件的完整路径。如果传 null，则自动根据子类类型名生成。</param>
    protected AutoSaveModel(string? filePath = null)
    {
        // 如果没有指定路径，则放到当前程序目录下的 "Saves" 文件夹中，文件名=类名.json。
        if (string.IsNullOrEmpty(filePath))
        {
            string typeName = GetType().Name;
            string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Saves");
            Directory.CreateDirectory(dir);
            filePath = Path.Combine(dir, $"{typeName}.json");
        }

        _filePath = filePath;
    }

    /// <summary>
    /// 将当前对象保存到文件（全量序列化）。
    /// 保存成功后会清除脏标记。
    /// </summary>
    public void Save()
    {
        try
        {
            string? dir = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }

            string json = JsonSerializer.Serialize(
                this,
                GetType(),
                CreateJsonOptions());

            File.WriteAllText(_filePath, json);
            _isDirty = false;
        }
        catch (Exception ex)
        {
            // 使用日志记录。
            Console.WriteLine($"[AutoSaveModel] 保存失败: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// 如果当前对象发生过变化，则保存到文件。
    /// 这个方法放在 EffectResolver.ApplyAll() 之后调用，实现“一次结算保存一次”。
    /// 如果每改一次就保存一次，会造成重复写文件，也更容易产生半结算存档。
    /// </summary>
    /// <returns>如果真的执行了保存，返回 true；如果没有变化，返回 false。</returns>
    public bool SaveIfDirty()
    {
        if (!_isDirty)
        {
            return false;
        }

        Save();
        return true;
    }

    /// <summary>
    /// 从文件加载数据覆盖当前对象（全量反序列化）。
    /// 加载成功后会清除脏标记。
    /// </summary>
    public void Load()
    {
        try
        {
            if (!File.Exists(_filePath))
            {
                // 文件不存在时保持当前对象默认值。
                return;
            }

            _isLoading = true;

            string json = File.ReadAllText(_filePath);

            // 反序列化为当前运行时类型。
            // 这里使用 GetType() 是为了让 GameState 这种子类可以正确恢复自己的公开属性。
            object? loaded = JsonSerializer.Deserialize(json, GetType(), CreateJsonOptions());
            if (loaded == null)
            {
                return;
            }

            // 将 loaded 的公开属性值复制到 this。
            // 只复制可读可写属性；只读属性、索引器、私有字段不参与。
            foreach (var prop in GetType().GetProperties())
            {
                if (!prop.CanRead || !prop.CanWrite)
                {
                    continue;
                }

                if (prop.GetIndexParameters().Length > 0)
                {
                    continue;
                }

                object? value = prop.GetValue(loaded);
                prop.SetValue(this, value);
            }

            _isDirty = false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AutoSaveModel] 加载失败: {ex.Message}");
            // 加载失败不抛出异常，使用当前默认值继续。
        }
        finally
        {
            _isLoading = false;
        }
    }

    /// <summary>
    /// 辅助方法：用于子类的属性 setter 中，实现“修改后标记脏数据”。
    /// 
    /// 这个方法仍然有用。
    /// 它主要负责“替换 GameState 顶层属性”时自动标记脏数据。
    /// 
    /// 示例：
    /// private PlayerProfile _player = new PlayerProfile();
    /// public PlayerProfile Player
    /// {
    ///     get => _player;
    ///     set => SetField(ref _player, value);
    /// }
    /// 
    /// 当执行 state.Player = new PlayerProfile() 时，SetField 会生效。
    /// 但当执行 state.Player.Name = "xxx" 时，Player 的 setter 不会被调用。
    /// 所以后者应该统一写进 Mutate()。
    /// </summary>
    /// <typeparam name="T">字段类型。</typeparam>
    /// <param name="field">引用传递的私有字段。</param>
    /// <param name="value">传入的新值。</param>
    /// <param name="propertyName">属性名（自动获取，无需手动填写）。</param>
    /// <returns>如果真的有变化，返回 true；如果新旧值相同，返回 false。</returns>
    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;

        // Load() 过程中设置属性，不应该让对象变脏。
        if (!_isLoading)
        {
            MarkDirty();
        }

        return true;
    }

    /// <summary>
    /// 手动标记当前对象发生过变化。
    /// 
    /// 使用场景：
    /// 1. 修改了 List 内部元素，例如 Courses.Add(course)，不会触发属性 setter。
    /// 2. 修改了嵌套对象内部属性，例如 Player.Name = "xxx"，不会触发 GameState.Player 的 setter。
    /// 3. 某些系统批量修改状态后，希望显式告诉存档系统“需要保存”。
    /// </summary>
    protected void MarkDirty()
    {
        _isDirty = true;
    }

    /// <summary>
    /// 公开的脏标记方法。
    /// 当外部系统确实修改了集合内部或嵌套对象内部时，可以调用它。
    /// 
    /// 注意：
    /// 现在更推荐使用 Mutate() 包裹状态修改。
    /// MarkChanged() 主要保留给旧代码、临时调试代码，或无法方便包裹的特殊情况。
    /// </summary>
    [Obsolete("优先使用 Mutate(...) 包裹状态修改；MarkChanged() 仅保留给旧代码或特殊情况。")]
    public void MarkChanged()
    {
        MarkDirty();
    }

    /// <summary>
    /// 统一修改入口。
    /// 
    /// 使用场景：
    /// 1. 修改顶层属性，例如 state.CurrentScene = scene。
    /// 2. 修改嵌套对象，例如 state.Attributes.Stamina.Current -= 10。
    /// 3. 修改集合内部，例如 state.Tasks.Add(task)。
    /// 
    /// 这样 Effect 不需要关心“这次修改会不会触发属性 setter”。
    /// 只要状态变化写在 Mutate() 里，执行完成后就会统一标记为脏数据。
    /// 
    /// 推荐规则：
    /// 1. 凡是 Effect 里要修改 GameState，都优先使用 context.State.Mutate(...)。
    /// 2. 不管修改的是顶层属性、嵌套对象、还是集合内部，都放进 Mutate()。
    /// 3. Mutate() 只负责标记脏数据，不负责立即保存。
    /// 4. 保存统一由 EffectResolver.ApplyAll() 末尾的 SaveIfDirty() 完成。
    /// 
    /// 示例一：修改顶层属性。
    /// context.State.Mutate(() =>
    /// {
    ///     context.State.CurrentScene = new Scene("mall", "商场", "Life");
    /// });
    /// 
    /// 示例二：修改嵌套对象。
    /// context.State.Mutate(() =>
    /// {
    ///     context.State.Attributes.Stamina.Current -= 10;
    /// });
    /// 
    /// 示例三：修改集合内部。
    /// context.State.Mutate(() =>
    /// {
    ///     context.State.Tasks.Add(task);
    /// });
    /// 
    /// 注意：
    /// Mutate() 不会判断 mutation 里面是否真的改了值。
    /// 只要执行了 mutation，就会认为状态可能发生变化，并标记为脏数据。
    /// 如果需要避免无意义保存，请在调用 Mutate() 前先做条件判断。
    /// </summary>
    /// <param name="mutation">需要执行的状态修改逻辑。</param>
    public void Mutate(Action mutation)
    {
        if (mutation == null)
        {
            throw new ArgumentNullException(nameof(mutation));
        }

        mutation();
        MarkDirty();
    }

    /// <summary>
    /// 创建 JSON 序列化选项。
    /// 子类如果需要特殊转换器，可以重写这个方法。
    /// </summary>
    protected virtual JsonSerializerOptions CreateJsonOptions()
    {
        return new JsonSerializerOptions
        {
            WriteIndented = true,
            IncludeFields = false
        };
    }
}