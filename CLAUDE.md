# Seed — CLAUDE.md

## 프로젝트 개요

- **유형**: Unity 공통 코드 베이스 (여러 프로젝트에서 재사용)
- **엔진**: Unity 6000.0.51f1 (Unity 6)
- **렌더 파이프라인**: URP 17.0.4
- **외부 의존**: 없음 (UniTask / DOTween / Cinemachine 등 미사용)

이 레포는 단일 게임이 아닌 **모듈 라이브러리**다. 각 모듈은 `Assets/Seed/<Module>/` 아래에 asmdef 단위로 격리되어 있으며, 사용처에서는 필요한 모듈만 골라 참조한다.

### 현재 모듈

| 모듈 | asmdef | 네임스페이스 | 외부 의존 | 설명 |
|------|--------|------------|-----------|------|
| Pooling | `Seed.Pooling` | `Seed.Pooling` | — | `IPoolable<TKey>`, `PoolOps`, `BasePool<TKey,TItem>` |
| StateMachine | `Seed.StateMachine` | `Seed.StateMachine` | — | 제네릭 FSM (`IState`, `BaseState<TContext>`, `StateMachine<TContext>`) |
| Camera | `Seed.Camera` | `Seed.CameraSystem` | — | `CameraHelper.MainCamera`, `IsOnScreen`, `PlaceAtWorldPoint` |
| Input | `Seed.Input` | `Seed.Input` | — | `JoystickController` (모바일 가상 조이스틱) |
| Audio | `Seed.Audio` | `Seed.Audio` | `Seed.Camera` | `SoundLibrarySO<TId>`, `SoundUtil<TId>`, `SoundLibraryBootstrap<TId>` |
| Interaction | `Seed.Interaction` | `Seed.Interaction` | — | `IInteractable<TActor>`, `ActorInteractor<TActor>` |

### 향후 계획

- **Import / Export 툴**: 사용처 프로젝트에서 모듈을 골라 받아오거나, 이 레포로 모듈 단위 변경을 역으로 보낼 수 있는 Editor 툴 개발 예정. 모듈 경계와 의존 그래프가 깔끔할수록 툴 설계가 수월해지므로, 모듈 추가 시 외부 의존과 인터-모듈 의존을 의식해서 작성한다.
- 외부 패키지 의존이 필요한 코드(예: DOTween 기반 Tween, UniTask 기반 Scenario, Cinemachine 기반 CameraFocus)는 별도 모듈로 분리하여 추가할 수 있다.

---

## 모듈 / 네임스페이스 규칙

### asmdef
- 한 모듈 = 하나의 asmdef = `Assets/Seed/<Module>/Seed.<Module>.asmdef`
- asmdef 이름은 **항상** `Seed.<Module>` (예: `Seed.Pooling`, `Seed.Camera`)

### 네임스페이스
- 기본은 asmdef 이름과 동일: `Seed.<Module>`
- **`UnityEngine.<Module>`과 충돌하는 경우** 네임스페이스에 `System` 접미사를 붙인다:
  - `Seed.Camera` (asmdef) → `namespace Seed.CameraSystem` (UnityEngine.Camera 충돌 회피)
  - 동일 패턴: `Seed.Input` → 충돌 시 `Seed.InputSystem` 등
- asmdef 파일의 `rootNamespace` 필드를 충돌 회피 네임스페이스로 명시하여 자동 생성 파일도 따라가게 한다.

### 모듈 간 의존
- 가능하면 모듈 간 의존 없이 작성. 의존이 필요하면 asmdef `references`에 명시 (예: `Seed.Audio` → `Seed.Camera`).
- 외부 패키지 의존은 도입 전 검토 — 지금까지의 원칙은 "외부 의존 0".

---

## 코딩 컨벤션 (C#)

| 대상 | 규칙 | 예시 |
|------|------|------|
| 클래스 / 인터페이스 | PascalCase | `BasePool`, `IInteractable` |
| 프로퍼티 | PascalCase | `public int Count { get; private set; }` |
| 파라미터 / 로컬 변수 | camelCase | `float tickRate`, `int count` |
| private 필드 | `_camelCase` | `private float _speed;` |
| private static readonly | PascalCase | `private static readonly Collider[] OverlapBuffer;` |
| SerializeField | private, 언더스코어 없음 | `[SerializeField] private float speed;` |
| 상수 | SCREAMING_SNAKE_CASE | `const int MAX_STACK = 30;` |
| 이벤트 | `On` + PascalCase | `public event Action OnMoneyChanged;` |
| 코루틴 메서드 | `Coroutine` 접미사 | `IEnumerator MoveCoroutine()` |
| ScriptableObject 클래스 | `SO` 접미사 | `SoundLibrarySO`, `ToolDataSO` |
| 인터페이스 | `I` 접두사 + PascalCase | `IInteractable`, `IPoolable` |

### 추가 규칙
- `#region`은 가독성에 도움이 될 때 한해 사용 — 단, 파일을 작게 유지하는 것이 우선
- `Awake`: 레퍼런스 초기화 / `Start`: 로직 초기화 용도 분리
- `Update`에서 직접 물리 연산 금지 → `FixedUpdate` 또는 코루틴 사용
- null 체크는 `is null` / `is not null` 사용 (`== null` 지양)
  - 단, `UnityEngine.Object`의 destroyed 상태 체크가 필요한 경우는 `== null` 패턴이 맞다 (예: `_currentInteractable is Object obj && obj == null`)

---

## 클래스 네이밍 룰

라이브러리 자체 코드뿐 아니라, 이 레포의 모듈을 가져다 쓰는 사용처 프로젝트에서도 따르는 것을 권장.

### 일반

| 카테고리 | 규칙 | 예시 |
|----------|------|------|
| 매니저 (Singleton) | `Manager` 접미사 | `CurrencyManager`, `GameManager` |
| FSM 상태 | `State` 접미사 | `IdleState`, `MoveState` |
| ScriptableObject | `SO` 접미사 | `SoundLibrarySO`, `ToolDataSO` |
| 인터페이스 | `I` 접두사 | `IInteractable`, `IPoolable` |
| 추상 기반 클래스 | `Base` 접두사 | `BasePool`, `BaseState` |
| 데이터 컨테이너 (pure C#) | `Data` 접미사 | `UpgradeData` |
| Helper / Util (static) | `Helper` 또는 `Util` 접미사 | `CameraHelper`, `SoundUtil` |

### UI

| 카테고리 | 규칙 | 예시 |
|----------|------|------|
| 화면 단위 (MVC View) | `View` 접미사 | `HudView`, `ResultView` |
| 버튼 | `Button` 접미사 | `RetryButton` |
| 텍스트 라벨 | `Label` 접미사 | `MoneyLabel` |
| 게이지 / 바 | `Bar` 접미사 | `ProgressBar` |
| 아이콘 | `Icon` 접미사 | `CurrencyIcon` |
| 리스트 셀 | `Item` 접미사 | `RewardItem` |
| 팝업 (모달) | `Popup` 접미사 | `ConfirmPopup` |
| 월드 부착 UI | `Indicator` 접미사 | `ProgressIndicator` |
| 잠깐 표시 후 사라지는 UI | `Toast` 접미사 | `RewardToast` |

---

## 커밋 룰

### 형식
```
<type>: <subject>

<body (선택)>
```

### Type
| Type | 설명 |
|------|------|
| feat | 새 기능 추가 |
| fix | 버그 수정 |
| refactor | 리팩토링 (기능 변경 없음) |
| chore | 패키지/빌드/설정 등 |
| docs | 문서 변경 |
| style | 포맷팅 (코드 동작 변경 없음) |
| test | 테스트 추가/수정 |
| perf | 성능 개선 |

### 규칙
- subject는 50자 이내, 한국어 OK
- 명령형 어미 (`~을 추가했다` → `~ 추가`)
- 끝에 마침표 X
- body는 **무엇**이 아닌 **왜**를 설명
- 한 커밋에는 하나의 논리적 변경만 — 패키지 설치와 기능 추가는 분리
- 실제 커밋 실행 전 반드시 사용자 컨펌을 받는다

---

## 폴더 구조

```
Assets/
  Seed/
    Pooling/
      Seed.Pooling.asmdef
      IPoolable.cs
      PoolOps.cs
      BasePool.cs
    StateMachine/
      Seed.StateMachine.asmdef
      IState.cs
      BaseState.cs
      StateMachine.cs
    Camera/
      Seed.Camera.asmdef        # rootNamespace: Seed.CameraSystem
      CameraHelper.cs
    Input/
      Seed.Input.asmdef
      JoystickController.cs
    Audio/
      Seed.Audio.asmdef         # references: Seed.Camera
      SoundLibrarySO.cs
      SoundUtil.cs
      SoundLibraryBootstrap.cs
    Interaction/
      Seed.Interaction.asmdef
      IInteractable.cs
      ActorInteractor.cs
```

새 모듈은 `Assets/Seed/<Module>/`에 동일한 패턴으로 추가한다.
