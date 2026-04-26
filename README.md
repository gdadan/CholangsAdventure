# 촐랑이의 모험

충북게임 아카데미 프로그래밍 과정에서 만든 **첫 게임**입니다.

> **🏆 2023 충북게임 아카데미 성과발표회 대상**

<br/>

## 📝 프로젝트 소개

| 항목 | 내용 |
|------|------|
| **개발 기간** | 2023.08 ~ 2023.09 (약 10일) |
| **개발 환경** | C#, Unity |
| **개발 장르** | 2D 플랫포머 |
| **개발 인원** | 1명 (전체 단독 개발) |

**한 줄 설명** : 몬스터와 장애물을 피해 결승점에 도달해야 하는 2D 플랫포머 게임입니다.

<br/>

## 📷 인게임 화면

<img src="https://github.com/user-attachments/assets/f44996be-40d7-4a19-8ef0-be9880fc3bc4" width="45%" />
<img src="https://github.com/user-attachments/assets/86536b0f-65d8-4542-8a96-cf391a9bb855" width="45%" />
<img src="https://github.com/user-attachments/assets/6f746c23-3eb4-471e-9de6-cd84c4206f12" width="45%" />
<img src="https://github.com/user-attachments/assets/9542642f-6c7f-407f-b1c5-28f2e1047aff" width="45%" />

<br/>

## 🎯 첫 게임에서 시도한 것

| 영역 | 시도 |
|------|------|
| **적 AI** | enum + switch로 3종 몬스터 패턴 분기 |
| **물리 기반 점프** | Rigidbody2D + Raycast로 착지 판정 |
| **무적 시간** | 코루틴 + 레이어 변경으로 피격 후 무적 처리 |
| **점수 영구 저장** | `PlayerPrefs`로 최고 점수 기록 |
| **싱글톤 매니저** | `GameManager.instance`로 전역 게임 상태 관리 |
| **벽 끼임 버그 해결** | 좌우 Raycast로 벽에 붙어 멈추는 현상 직접 처리 |

<br/>

---

## 🛠 1. 적 AI — enum 기반 패턴 분기

### 고민했던 점
처음에는 몬스터마다 다 따로 만들려 했지만, **공통 동작이 있어 중복**이 발생했습니다.

### 해결법
`enum EnemyType` + `switch`로 한 스크립트에서 분기하고, **공통 이동 로직은 매개변수로 재사용**.

```csharp
public enum EnemyType { Mushroom, Chicken, Plant }

void Update()
{
    switch (enemyType)
    {
        case EnemyType.Mushroom: Mushroom(1f, 0.3f); break;
        case EnemyType.Chicken:  Chicken(); break;   // → Mushroom(4.5f, 0.4f)
        case EnemyType.Plant:    Plant(); break;
    }
}

// 발판 끝 감지 → 자동 회전
public void Mushroom(float speed, float layFront)
{
    rigid.velocity = new Vector2(nextMove * speed, rigid.velocity.y);

    Vector2 frontVec = new Vector2(rigid.position.x + nextMove * layFront, rigid.position.y);
    RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 2, LayerMask.GetMask("Platform"));

    if (rayHit.collider == null)  // 앞에 발판이 없으면
        Turn();                    // 반대 방향으로 회전
}
```

→ Chicken은 Mushroom 함수를 다른 인자로 호출하여 재사용. 코드 중복 없이 속도/감지 거리만 조정.

> 💡 **회고** : 지금 보면 [섀도우 헌터의 추상 클래스 기반 스킬 시스템](https://github.com/gdadan/ShadowHunterCode)의 **출발점**이었습니다. 그때는 enum 분기였지만 이후 추상 클래스로 확장하는 사고로 발전했습니다.

🔗 [Enemy.cs](./Assets/Scripts/Enemy.cs)

<br/>

---

## 🛠 2. 플레이어 — Raycast 기반 점프 판정과 벽 끼임 해결

### 고민했던 점
- 단순히 `Space` 키로 점프 힘만 주면 **공중 점프 가능 → 무한 점프 버그**
- 벽에 부딪힌 채로 점프하면 **벽에 붙어버리는 현상** 발생

### 해결법
Rigidbody2D 기반의 자연스러운 점프와 좌우 이동, 그리고 **벽에 닿았을 때 공중에 끼이지 않도록** 처리했습니다.

**(1) 점프 가능 여부 — 아래 방향 Raycast로 발판 감지**
```csharp
RaycastHit2D rayHit = Physics2D.Raycast(transform.position, Vector3.down, 1, LayerMask.GetMask("Platform"));
if (rayHit.collider != null && rayHit.distance < 0.8f)
{
    anim.SetBool("isJumping", false);
    isGrouded = true;
}
```

**(2) 가변 점프 높이 — 키 떼는 시점에 속도 절반으로**
```csharp
else if (Input.GetKeyUp(KeyCode.Space) && rigid.velocity.y > 0)
{
    rigid.velocity = rigid.velocity * 0.5f;  // 짧게 누르면 짧게 점프
}
```

**(3) 벽 끼임 해결 — 좌우 Raycast로 벽 감지 시 강제 하강**
```csharp
RaycastHit2D rayHitRight = Physics2D.Raycast(transform.position, Vector3.right, 0.5f, LayerMask.GetMask("Platform"));
RaycastHit2D rayHitLeft  = Physics2D.Raycast(transform.position, Vector3.left,  0.5f, LayerMask.GetMask("Platform"));

if (!isGrouded && (rayHitRight.distance < 0.01f || rayHitLeft.distance < 0.01f))
    rigid.velocity = Vector2.down;
```

→ 직접 마주친 버그를 **Raycast로 진단·해결한 첫 경험**.

🔗 [Player.cs](./Assets/Scripts/Player.cs)

<br/>

---


## 💡 회고 — 이 프로젝트에서 배운 것

- **"끝까지 만든다"의 의미** : 처음으로 게임을 완성해본 경험. 작은 버그 하나하나(벽 끼임, 무한 점프)를 직접 진단하고 해결하면서 **개발자의 책임감**을 배웠습니다.
- **다음 프로젝트로 이어진 사고** : 여기서 enum 분기로 처리한 적 패턴이, 이후 [섀도우 헌터](https://github.com/gdadan/ShadowHunterCode)에서는 **추상 클래스 + 인터페이스** 기반 스킬 시스템으로 발전했습니다. 같은 문제를 더 좋은 구조로 풀어내는 성장 과정이었습니다.
- **Unity 기본기 정립** : Rigidbody2D, Raycast, Animator, PlayerPrefs, 싱글톤, 코루틴, 레이어 — 이 게임에서 익힌 기본기가 이후 모든 프로젝트의 토대가 됐습니다.

<br/>

## 📁 코드 구조

```
Assets/Scripts/
├── Player.cs           # 플레이어 이동/점프/피격 (Raycast 활용)
├── Enemy.cs            # 적 AI (enum 패턴 분기)
├── GameManager.cs      # 싱글톤 게임 매니저 + 점수 저장
├── SoundManager.cs     # 사운드 매니저
├── Item.cs             # 코인/생명 아이템
├── MovingPlatform.cs   # 움직이는 플랫폼
├── RockHead.cs         # 떨어지는 장애물
├── ChangeScene.cs      # 씬 전환
└── ScrollingCloud.cs   # 배경 구름 스크롤
```
