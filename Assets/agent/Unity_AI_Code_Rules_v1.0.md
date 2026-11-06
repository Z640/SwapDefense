[Unity 팀 프로젝트 공통 AI 코드 규칙 v1.0]

이 문서는 우리 팀 프로젝트에서 사람이나 AI가 코드를 작성할 때 공통으로 적용하는 규칙입니다.
팀원이 ChatGPT, Claude, Copilot 등 어떤 AI를 사용하더라도 아래 내용을 그대로 붙여넣고 사용합니다.

프로젝트 환경
- 엔진: Unity 6000.3.9f1
- 언어: C#
- 프로젝트 성격: 학생 수준의 팀 프로젝트
- 게임 방향: 덕코프와 유사한 PvE 탐색 / 전투 / 루팅 / 탈출 구조
- 목표: 과도한 설계보다 이해하기 쉽고 수정하기 쉬운 코드를 우선합니다.

[기본 작성 원칙]
- 기존 프로젝트 구조와 기존 코드가 있다면 그것을 우선합니다.
- 기존 기능을 이유 없이 전면 수정하거나 구조를 갈아엎지 않습니다.
- 학생 팀 프로젝트 수준에 맞게 단순하고 이해하기 쉽게 작성합니다.
- 필요 이상으로 복잡한 디자인 패턴, 추상화, 인터페이스를 남발하지 않습니다.
- 하나의 스크립트가 너무 많은 서로 다른 역할을 맡지 않도록 합니다.
- 다른 팀원이 코드를 읽었을 때 이름만 보고 역할을 이해할 수 있게 작성합니다.
- 테스트용 기능과 실제 게임 기능은 가능하면 분리합니다.

[이름 규칙]
- 클래스: PascalCase / 예: RoomSystem, PlayerHealth, EnemySpawner
- 함수: PascalCase, 가능하면 동사로 시작 / 예: ClearRoom(), TakeDamage(), SpawnEnemy(), OpenDoor()
- 프로퍼티: PascalCase / 예: CurrentHealth, IsCleared
- private 변수: camelCase / 예: currentHealth, moveSpeed, enemyCount
- 지역 변수: camelCase / 예: targetPosition, damageAmount
- 매개변수: camelCase / 예: damageAmount, targetPosition
- bool 변수: 의미에 맞게 is / has / can 사용 권장
  예: isDead, isMoving, isCleared, hasKey, canAttack
- enum 이름과 값: PascalCase
- aaa, temp1, test123, check, flag, Function1, DoThing 같은 의미 없는 이름은 사용하지 않습니다.

[Inspector 변수 규칙]
Unity Inspector에서 수정할 값은 기본적으로 다음 형식을 사용합니다.

[SerializeField] private float moveSpeed = 5f;
[SerializeField] private int maxHealth = 100;

Inspector에 보여주기 위한 목적으로 public 필드를 사용하지 않습니다.

가능하면 사용하지 않음:
public float moveSpeed;

외부에서 값을 읽어야 한다면 프로퍼티를 사용합니다.

[SerializeField] private int currentHealth;
public int CurrentHealth => currentHealth;

[다른 시스템과 연결하는 규칙]
다른 클래스의 내부 변수를 외부에서 직접 수정하지 않습니다.

좋지 않은 예:
player.currentHealth -= 10;

좋은 예:
player.TakeDamage(10);

좋지 않은 예:
room.isCleared = true;

좋은 예:
room.ClearRoom();

원칙:
- 다른 시스템의 내부 상태를 직접 바꾸지 않습니다.
- 해당 시스템이 제공하는 public 함수나 프로퍼티를 사용합니다.
- 시스템 간 의존성을 필요 이상으로 강하게 만들지 않습니다.

[스크립트 역할 규칙]
하나의 스크립트가 지나치게 많은 기능을 담당하지 않도록 합니다.

예:
PlayerMovement.cs
PlayerHealth.cs
PlayerAttack.cs
PlayerInventory.cs

분리 기준:
- 서로 관계없는 기능이 한 클래스에 계속 추가될 때
- 코드가 너무 길어져 찾기 어려울 때
- 다른 시스템에서도 독립적으로 사용해야 할 때

[파일 이름 규칙]
파일명과 클래스명은 반드시 동일하게 작성합니다.

예:
파일: RoomSystem.cs

public class RoomSystem : MonoBehaviour
{
}

[숫자와 설정값 규칙]
의미를 알기 어려운 숫자를 코드 곳곳에 직접 작성하지 않습니다.

좋지 않은 예:
if (currentHealth <= 20)

좋은 예:
[SerializeField] private int lowHealthThreshold = 20;

if (currentHealth <= lowHealthThreshold)

단, 0이나 1처럼 의미가 명확한 값까지 억지로 변수로 만들 필요는 없습니다.

[코드 형식 규칙]
중괄호는 다음 형식으로 작성합니다.

if (isDead)
{
    return;
}

함수도 동일합니다.

private void MovePlayer()
{
}

한 줄 if문이라도 가능하면 중괄호를 사용해 팀 스타일을 통일합니다.

[Inspector 정리 규칙]
Inspector 변수가 많으면 Header를 사용해 정리합니다.

[Header("Movement")]
[SerializeField] private float moveSpeed = 5f;

[Header("Combat")]
[SerializeField] private int attackDamage = 10;
[SerializeField] private float attackRange = 1.5f;

[Header("Debug")]
[SerializeField] private bool showDebugLog;

[주석 규칙]
코드만 읽어도 알 수 있는 내용을 그대로 설명하는 주석은 피합니다.

불필요한 예:
// 체력을 10 감소시킨다.
currentHealth -= 10;

필요한 예:
// 이미 클리어한 방은 재입장해도 다시 잠그지 않는다.
if (isCleared)
{
    return;
}

주석은 "무엇을 하는지"보다 "왜 이렇게 했는지"를 설명할 때 사용합니다.

[Debug.Log 규칙]
개발 중 Debug.Log는 사용해도 됩니다.
기능이 완성되면 필요 없는 로그는 정리합니다.

반복적으로 사용할 로그라면 켜고 끌 수 있게 작성할 수 있습니다.

[SerializeField] private bool showDebugLog;

private void Log(string message)
{
    if (showDebugLog)
    {
        Debug.Log(message);
    }
}

[테스트 코드 규칙]
테스트용 기능은 실제 시스템 코드와 가능하면 분리합니다.

예:
Scripts/
├── Room/
│   └── RoomSystem.cs
└── Test/
    └── TempPlayerController2D.cs

예를 들어 K키로 방 강제 클리어, 무적 테스트, 임시 이동 캐릭터, 강제 아이템 지급 같은 기능은
실제 시스템 안에 직접 넣지 않고 테스트 스크립트로 분리합니다.

[AI가 코드를 작성할 때 반드시 지킬 것]
1. 기존 코드가 제공되었다면 기존 구조와 네이밍을 먼저 분석합니다.
2. 기존 코드를 이유 없이 전면 수정하지 않습니다.
3. 필요한 부분만 최소한으로 수정합니다.
4. 새 스크립트가 필요한 경우 왜 필요한지 설명합니다.
5. Inspector에서 어떤 값을 설정해야 하는지 함께 설명합니다.
6. Unity 초보자도 적용할 수 있도록 적용 순서를 설명합니다.
7. 코드가 여러 파일이라면 파일명을 명확히 구분합니다.
8. 다른 담당자의 시스템을 직접 수정해야 한다면 임의로 수정하지 말고 연결 방법을 제안합니다.
9. 특정 시스템이 아직 구현되지 않았다면 임시 테스트 방법을 따로 제공합니다.
10. 존재하지 않는 API나 클래스를 실제 프로젝트에 있는 것처럼 임의로 가정하지 않습니다.
11. Unity 6000.3.9f1 기준으로 작성합니다.
12. 과도한 최적화나 복잡한 구조보다 현재 기능을 안정적으로 완성하는 것을 우선합니다.

[AI 답변 형식]
코드를 요청받았을 때 가능하면 다음 순서로 답변합니다.

1. 구현하려는 기능 설명
2. 필요한 스크립트 목록
3. 각 스크립트 코드
4. Unity에서 적용하는 순서
5. Inspector 설정값
6. 테스트 방법
7. 이후 실제 시스템과 연결할 위치

[가장 중요한 핵심 규칙]
- 클래스 / 함수 / 프로퍼티 → PascalCase
- private 변수 / 지역 변수 / 매개변수 → camelCase
- bool → is / has / can 형식 권장
- Inspector 변수 → [SerializeField] private
- 파일명 → 클래스명과 동일
- 다른 시스템 내부 변수 직접 수정 금지
- public 함수나 프로퍼티를 통해 연결
- 의미 없는 변수명 사용 금지
- 기존 프로젝트 구조 우선

[AI에게 전달할 마지막 지시]
앞으로 내가 Unity 관련 코드나 기능 구현을 요청하면 위 규칙을 항상 적용해서 답변해 주세요.
기존 코드가 있다면 위 규칙보다 기존 프로젝트의 동작을 깨지 않는 것을 우선하고,
규칙과 기존 코드가 충돌할 경우 어떤 부분이 충돌하는지 먼저 알려주세요.
