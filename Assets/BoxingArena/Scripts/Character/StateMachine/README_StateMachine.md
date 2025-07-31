# StateMachine System cho Boxing Game

## Tổng quan

Project này sử dụng hệ thống StateMachine từ GamePremiumBase package để quản lý các trạng thái của boxer trong game boxing.

## Cấu trúc StateMachine

### Core Components

1. **StateMachine.Controller** - Quản lý việc chuyển đổi giữa các states
2. **StateMachine.State** - Đại diện cho một trạng thái cụ thể
3. **StateMachine.Transition** - Định nghĩa cách chuyển từ state này sang state khác
4. **StateMachine.Behaviour** - Logic xử lý trong mỗi state
5. **StateMachine.Event** - Trigger để kích hoạt transition

### Editable Components

1. **EditableStateMachineController** - MonoBehaviour để quản lý StateMachine
2. **StateSO** - ScriptableObject cho các state
3. **TransitionSO** - ScriptableObject cho các transition

## Cách sử dụng

### 1. Tạo States

```csharp
[CreateAssetMenu(fileName = "MyState", menuName = "BoxingArena/States/MyState")]
public class MyStateSO : StateSO
{
    protected override void StateEnable()
    {
        // Logic khi vào state
    }

    protected override void StateUpdate()
    {
        // Logic cập nhật mỗi frame
    }

    protected override void StateDisable()
    {
        // Logic khi thoát state
    }
}
```

### 2. Tạo Transitions

```csharp
[CreateAssetMenu(fileName = "MyTransition", menuName = "BoxingArena/Transitions/MyTransition")]
public class MyTransitionSO : TransitionSO
{
    public override void SetupTransition(object[] parameters)
    {
        // Setup transition logic
        transition = new State.Transition(
            new PredicateEvent(() => condition),
            targetState.State
        );
    }
}
```

### 3. Setup StateMachine

```csharp
public class MyStateMachineController : EditableStateMachineController
{
    [SerializeField] private List<StateSO> states;
    
    protected override void Awake()
    {
        base.Awake();
        // Setup states
        foreach (var state in states)
        {
            state.SetupState(parameters);
        }
    }
    
    public void StartStateMachine()
    {
        StartStateMachine();
    }
}
```

## States trong Boxing Game

### 1. BoxerIdleStateSO
- **Mục đích**: Trạng thái nghỉ ngơi của boxer
- **Input**: Space (Attack), B (Block), WASD (Move)
- **Animation**: Idle animation

### 2. BoxerAttackStateSO
- **Mục đích**: Thực hiện tấn công
- **Duration**: 1 giây
- **Damage**: Gây sát thương cho đối thủ
- **Animation**: Attack animation

### 3. BoxerBlockStateSO
- **Mục đích**: Phòng thủ
- **Damage Reduction**: 80% giảm sát thương
- **Input**: Giữ B để block
- **Animation**: Block animation

### 4. BoxerMoveStateSO
- **Mục đích**: Di chuyển boxer
- **Speed**: 3.5 units/second
- **Input**: WASD
- **Animation**: Walk/Run animation

### 5. BoxerKOStateSO
- **Mục đích**: Trạng thái bất tỉnh
- **Trigger**: Khi health <= 0
- **Duration**: 3 giây
- **Animation**: KO animation

## Transitions

### Types of Transitions

1. **Manual** - Chuyển đổi thủ công
2. **Timed** - Chuyển đổi theo thời gian
3. **InputBased** - Chuyển đổi theo input
4. **HealthBased** - Chuyển đổi theo health

### Example Transitions

```csharp
// Idle -> Attack (Space key)
var idleToAttack = new State.Transition(
    new PredicateEvent(() => Input.GetKeyDown(KeyCode.Space)),
    attackState.State
);

// Attack -> Idle (After attack duration)
var attackToIdle = new State.Transition(
    new PredicateEvent(() => attackTime >= attackDuration),
    idleState.State
);

// Any -> KO (Health <= 0)
var toKO = new State.Transition(
    new PredicateEvent(() => health <= 0),
    koState.State
);
```

## Events và Callbacks

### State Events

```csharp
// Subscribe to state events
state.StateStarted += (state) => Debug.Log("State started");
state.StateEnded += (state) => Debug.Log("State ended");
```

### Transition Events

```csharp
// Subscribe to transition events
transition.EventTriggered += (transition) => Debug.Log("Transition triggered");
```

## Best Practices

### 1. State Design
- Mỗi state chỉ nên có một trách nhiệm cụ thể
- Sử dụng StateEnable() để khởi tạo, StateDisable() để cleanup
- Tránh logic phức tạp trong StateUpdate()

### 2. Transition Design
- Transitions nên rõ ràng và có điều kiện cụ thể
- Sử dụng PredicateEvent cho điều kiện phức tạp
- Tránh circular transitions

### 3. Performance
- Cache references trong SetupState()
- Tránh tạo objects mới trong Update()
- Sử dụng object pooling cho effects

### 4. Debugging
- Sử dụng Debug.Log để track state changes
- Implement state visualization trong editor
- Add breakpoints trong transition conditions

## Ví dụ hoàn chỉnh

Xem file `BoxerStateMachineExample.cs` để có ví dụ hoàn chỉnh về cách implement StateMachine cho boxing game.

## Troubleshooting

### Common Issues

1. **State không chuyển đổi**: Kiểm tra transition conditions
2. **Animation không play**: Kiểm tra Animator references
3. **Performance issues**: Kiểm tra Update() logic
4. **Null references**: Kiểm tra SetupState() parameters

### Debug Tips

1. Enable debug logs trong states
2. Use Unity's State Machine Debugger
3. Add breakpoints trong transition conditions
4. Check Inspector values at runtime 