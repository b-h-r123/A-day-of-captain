/// <summary>
    /// 游戏运行模式。
    /// 详细操控用于逐日、逐阶段选择行动；快速跳周用于按照玩家预设偏向批量推进 7 天。
    /// </summary>
    public enum GameMode
    {
        DetailedControl,
        FastForwardWeek
    }

    /// <summary>
    /// 玩家每天开始时选择的体力分配倾向。
    /// 这个枚举不直接修改体力，而是作为输入交给 ResourceSystem 或 EffectResolver 处理。
    /// </summary>
    public enum DailyFocus
    {
        Daytime,
        Night,
        Balanced
    }

    /// <summary>
    /// 一天内的阶段。
    /// 文档中“一天是一个回合”，上午、下午、夜晚等就是阶段。
    /// </summary>
    public enum StageType
    {
        Morning,
        Forenoon,
        Afternoon,
        Evening,
        LateNight
    }

    /// <summary>
    /// 主行动类型。
    /// 主行动用于决定阶段入口，例如“上课”进入教室，“生活”进入泛用生活场景。
    /// </summary>
    public enum MainActionType
    {
        AttendClass,
        Life,
        Study,
        Work,
        SleepIn
    }

    /// <summary>
    /// 健康阶段。
    /// 不建议让 Health 直接去改 Stamina 和 Mood；更好的做法是发布状态变化，
    /// 再由 ResourceSystem 统一计算最终上限和倍率。
    /// </summary>
    public enum HealthState
    {
        Healthy,
        Weakened,
        Sick,
        Critical
    }

    /// <summary>
    /// 每日心情状态。
    /// 心情影响事件概率、经验获取和部分扣除倍率。
    /// </summary>
    public enum MoodState
    {
        Good,
        Normal,
        Bad
    }

    /// <summary>
    /// 课程类型。
    /// 课程类型可用于计算总学业分、触发不同事件池、影响奖学金判定。
    /// </summary>
    public enum CourseCategory
    {
        General,
        Major
    }

    /// <summary>
    /// 上课时的课堂表现。
    /// 这是“上课”主行动下的典型子行动结果输入。
    /// </summary>
    public enum ClassPerformance
    {
        Focused,
        Slacking,
        DoingProject
    }

    /// <summary>
    /// 事件触发方式。
    /// 主动事件通常来自玩家选择；被动事件可能固定、随机或满足条件后发生。
    /// </summary>
    public enum EventTriggerMode
    {
        Active,
        Fixed,
        Random,
        Conditional
    }

    /// <summary>
    /// 事件类型。
    /// 分类主要用于事件池筛选、概率修正和 UI 展示。
    /// </summary>
    public enum EventCategory
    {
        Daily,
        Relationship,
        Organization,
        Opportunity,
        Crisis
    }

    /// <summary>
    /// NPC 关系阶段。
    /// 关系阶段由 RelationshipSystem 根据好感度阈值解锁，不建议由 PhoneSystem 直接修改。
    /// </summary>
    public enum RelationshipLevel
    {
        Acquaintance,
        Friend,
        CloseFriend,
        Lover
    }

    /// <summary>
    /// 经济收入类型。
    /// 经济系统只处理钱的增减，不直接决定事件或卡牌。
    /// </summary>
    public enum IncomeType
    {
        Scholarship,
        ProjectBonus,
        PartTimeJob,
        Outsourcing
    }

    /// <summary>
    /// 消费类型。
    /// 购买卡牌、事件支出等都可以建模为消费，但最终效果仍建议走 EffectResolver。
    /// </summary>
    public enum ExpenseType
    {
        Meal,
        Item,
        Card,
        EventCost
    }

    /// <summary>
    /// 领域事件类型。
    /// 用枚举只是为了示例清晰；正式项目也可以用字符串 ID 或派生事件类。
    /// </summary>
    public enum DomainEventType
    {
        DayStarted,
        DayEnded,
        StageStarted,
        StageEnded,
        ActionExecuted,
        ResourceChanged,
        CourseAttended,
        TaskProgressed,
        TaskExpired,
        NpcFavorChanged,
        RelationshipUnlocked,
        MoneyChanged,
        CardAcquired,
        EndingTriggered,
        AchievementUnlocked
    }