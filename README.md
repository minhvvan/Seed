# Seed

Unity 프로젝트 베이스 코드 모음. `Assets/Seed/` 아래에 모듈별로 asmdef 분리되어 있어 필요한 모듈만 골라 쓸 수 있다.

## 모듈 구성

| 모듈 | asmdef | 외부 의존 | 설명 |
|------|--------|-----------|------|
| Pooling | `Seed.Pooling` | — | `IPoolable<TKey>`, `PoolOps`, `BasePool<TKey,TItem>` |
| StateMachine | `Seed.StateMachine` | — | 제네릭 FSM (`IState`, `BaseState<TContext>`, `StateMachine<TContext>`) |
| Camera | `Seed.Camera` | — | `CameraHelper.MainCamera`, `IsOnScreen`, `PlaceAtWorldPoint` |
| Input | `Seed.Input` | — | `JoystickController` (모바일 가상 조이스틱) |
| Audio | `Seed.Audio` | `Seed.Camera` | `SoundLibrarySO<TId>`, `SoundUtil<TId>`, `SoundLibraryBootstrap<TId>` |
| Interaction | `Seed.Interaction` | — | `IInteractable<TActor>`, `ActorInteractor<TActor>` |

전부 외부 패키지 의존 없음 (UniTask / DOTween / Cinemachine 불필요).

## 사용 예시

### Pooling
```csharp
public enum ItemType { Mineral, Money }

public class StackItem : MonoBehaviour, IPoolable<ItemType>
{
    [SerializeField] private ItemType type;
    public ItemType PoolKey => type;
}

public class ItemPool : BasePool<ItemType, StackItem> { }
```

### StateMachine
```csharp
public class IdleState : BaseState<PlayerController>
{
    public IdleState(PlayerController c, StateMachine<PlayerController> sm) : base(c, sm) { }
    public override void Tick() { /* ... */ }
}

_stateMachine = new StateMachine<PlayerController>();
_stateMachine.ChangeState(new IdleState(this, _stateMachine));
```

### Audio
```csharp
public enum SfxId { None, Hit, Coin }

[CreateAssetMenu(menuName = "Game/Sfx Library")]
public class SfxLibrarySO : SoundLibrarySO<SfxId> { }

public class SfxBootstrap : SoundLibraryBootstrap<SfxId> { }

SoundUtil<SfxId>.Play(SfxId.Hit, audioSource);
```

### Interaction
```csharp
public interface IPlayerInteractable : IInteractable<Player> { }

public class PlayerInteractor : ActorInteractor<Player> { }
```

## 익스포트하지 않은 항목 (외부 의존 필요)

원본 PayToJail에는 다음 모듈도 있으나 외부 패키지 의존 때문에 제외:

- **Tween** (DOTween) — `ItemTween.Jump`
- **Scenario** (UniTask) — `ScenarioManager`, `ScenarioStep` 비동기 시나리오 러너
- **CameraFocus** (Cinemachine + UniTask) — `CameraFocusManager` 포커스/리턴
- **BaseToast** (Cinemachine + DOTween) — 카메라 블렌딩 중 자동 숨김 토스트

필요 시 패키지 추가 후 별도 모듈로 추가.
