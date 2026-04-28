using Godot;
using Godot.Collections;

enum CardState
{
    _base,
    _dragging
}
public partial class Card : Control
{
    #region 卡面静态数据
    [Export] private int _cardId;
    [Export] private int _cardLevel;
    [Export] private string _cardName;
    [Export] private string _cardDescription;
    [Export] private string _cardType;
    [Export] private string _cardAttribute;
    [Export] private NodePath _iconPath;
    [Export] private NodePath _nameLabelPath;
    [Export] private NodePath _levelIconPath;
    [Export] private NodePath _attrIconPath;

    private TextureRect _iconNode;
    private Label _nameLabelNode;
    private TextureRect _levelIcon;
    private TextureRect _attrIcon;
    #endregion
    [Export] private CardState _currentState = CardState._base;
    [Export] private Control _followTarget;
    private Vector2 _velocity = Vector2.Zero;
    private float _draggingDamping = 0.4f;
    private int _sensitivity = 500;
    public override void _Ready()
    {
        _iconNode = GetNodeOrNull<TextureRect>(_iconPath);
        _nameLabelNode = GetNodeOrNull<Label>(_nameLabelPath);
        _levelIcon = GetNodeOrNull<TextureRect>(_levelIconPath);
        _attrIcon = GetNodeOrNull<TextureRect>(_attrIconPath);

        Initialize(_cardId);
    }
    public override void _Process(double delta)
    {
        base._Process(delta);
        switch (_currentState)
        {
            case CardState._base:

                if(_followTarget != null)
                {
                    ElasticMove((float)delta, _followTarget.GlobalPosition, _sensitivity);
                }
                break;

            case CardState._dragging:

                Vector2 targetPosition = GetGlobalMousePosition() - GetSize() / 2;
                GlobalPosition = GlobalPosition.Lerp(targetPosition, _draggingDamping);
                break;
        }
    }
    public void Initialize(int id)
    {
        if (DataManager.Instance == null)
        {
            GD.PrintErr("请确认DataManager已配置为AutoLoad。");
            return;
        }

        Dictionary card = DataManager.Instance.GetCardbyId(id);
        if (card == null)
        {
            GD.PrintErr($"Initialize failed: card id {id} not found.");
            return;
        }

        _cardId = id;

        if (card.TryGetValue("level", out var level)) _cardLevel = (int)level;
        if (card.TryGetValue("name", out var name)) _cardName = name.ToString();
        if (card.TryGetValue("attribute", out var attr)) _cardAttribute = attr.ToString();
        if (card.TryGetValue("type", out var typeArr) && typeArr.VariantType == Variant.Type.Array)
        {
            Array arr = typeArr.AsGodotArray();
            if (arr.Count > 0) _cardType = arr[0].ToString();
        }

        string displayText;
        if (card.TryGetValue("name", out var name_key))
        {
            _cardName = name_key.ToString().Localized();
            displayText = _cardName;
        }
        else displayText = "[No name available]";

        if (_nameLabelNode != null)
        {
            _nameLabelNode.Text = displayText;
        }

        if (_iconNode != null && card.TryGetValue("image", out var imagePath))
        {
            Texture2D tex = ResourceLoader.Load<Texture2D>(imagePath.ToString());
            if (tex != null) _iconNode.Texture = tex;
            else GD.PrintErr($"Image load failed: {imagePath}");
        }
    }

    #region 基础拖拽方法区域
    // 弹簧函数（B站上找的，感觉效果不错）
    private void ElasticMove(float delta, Vector2 targetPosition, int sensitivity)
    {
        Vector2 direction = targetPosition - GlobalPosition - GetSize() / 2;
        Vector2 force = direction * sensitivity;
        float damping = 0.1f;

        _velocity += force * delta;
        _velocity *= 1 - damping;

        GlobalPosition += _velocity * delta;
    }
    // <-信号
    private void OnButtonDown()
    {
        _currentState = CardState._dragging;
        GD.Print("Dragging started");
    }
    // <-信号
    private void OnButtonUp()
    {
        _currentState = CardState._base;
        GD.Print("Dragging stopped");
    }
    #endregion
    
    // 设置跟随目标的接口
    public void SetFollowTarget(Control target)
    {
        _followTarget = target;
    }

    // 卡牌翻转的接口
    public void Swap(){}
    // 下面是卡牌效果触发的接口，后续会根据需要添加参数
    public virtual void On_Play(){}
    public virtual void On_Place(){}
}