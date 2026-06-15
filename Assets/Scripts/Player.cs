using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float maxSpeed; //플레이어 최고 속도
    public float jumpPower; //플레이어 점프

    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriteRenderer;

    public GameObject oriParent;

    bool isGrouded = true;

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        //플레이어 점프
        if (Input.GetKeyDown(KeyCode.Space) && !anim.GetBool("isJumping"))
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("isJumping", true);

            SoundManager.instance.PlaySFX(5);
        }
        else if (Input.GetKeyUp(KeyCode.Space) && rigid.velocity.y > 0)
        {
            //스페이스 버튼에서 손을 떼는 순간 && 속도의 y 값이 양수라면(위로 상승 중)
            //현재 속도를 절반으로 변경
            rigid.velocity = rigid.velocity * 0.5f;
            isGrouded = false;
        }

        //조작키를 떼면 속도 멈추기
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
        }

        //플레이어 보는 방향 전환
        if (Input.GetButton("Horizontal"))
        {
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }

        //애니매이션 전환
        if (Mathf.Abs(rigid.velocity.x) < 0.2f)
        {
            anim.SetBool("isRunning", false);
        }
        else
        {
            anim.SetBool("isRunning", true);
        }
    }

    private void FixedUpdate()
    {
        //플레이어 움직임
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        //플레이어 최고속도 제한
        if (rigid.velocity.x > maxSpeed)
        {
            //Right Max Speed
            rigid.velocity = new Vector2 (maxSpeed, rigid.velocity.y);
        }
        else if (rigid.velocity.x < maxSpeed * (-1))
        {
            //Left Max Speed
            rigid.velocity = new Vector2 (maxSpeed * (-1), rigid.velocity.y);
        }

        
        //점프에서 점프안하는 모션으로 애니메이션 전환
        if ((rigid.velocity.y <= 0))
        {
            Debug.DrawRay(transform.position, Vector3.down, new Color(0, 1, 0));

            RaycastHit2D rayHit = Physics2D.Raycast(transform.position, Vector3.down, 1, LayerMask.GetMask("Platform"));

            if (rayHit.collider != null)
            {                
                if (rayHit.distance < 0.8f)
                {
                    anim.SetBool("isJumping", false);
                    isGrouded = true;
                }
            }
        }

        StopJumping();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //적 위에서 밟으면 공격, 다른 곳에 닿으면 데미지
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)
            {
                OnAttack(collision.transform);
            }
            else
            {
                OnDamaged(collision.transform.position);

            }
        }

        //스파이크에 닿으면 데미지
        if (collision.gameObject.CompareTag("Spike"))
        {
            OnDamaged(collision.transform.position);
        }

        //총알에 닿으면 데미지를 받고 총알 삭제
        if (collision.gameObject.CompareTag("EnemyBullet"))
        {
            OnDamaged(collision.transform.position);
            Destroy(collision.gameObject);
        }

        //움직이는 플랫폼에 닿으면 그 플랫폼 자식으로
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            transform.SetParent(collision.transform);
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = 0;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            transform.SetParent(oriParent.transform);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //코인에 닿을 시 Score++
        if (collision.gameObject.CompareTag("Coin"))
        {
            collision.gameObject.GetComponent<Item>().AddScore();         
        }

        //상자에 닿으면 목숨 +1
        if (collision.gameObject.CompareTag("BonusLife"))
        {
            collision.gameObject.GetComponent<Item>().AddLife();
        }

        //결승점에 닿으면 다음 스테이지로
        if (collision.gameObject.CompareTag("EndPoint"))
        {
            GameManager.instance.NextStage();
        }
    }

    //플레이어 공격 시
    void OnAttack(Transform enemy)
    {
        SoundManager.instance.PlaySFX(6);

        //Reaction Force
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);

        //Enemy Die
        Enemy enemyLogic = enemy.GetComponent<Enemy>();
        enemyLogic.OnDamaged();
    }

    //플레이어가 데미지 받을 시
    void OnDamaged(Vector2 targetPos)
    {
        //Health --
        GameManager.instance.HealthDown();

        //Change Layer
        gameObject.layer = 11;

        //Reaction Force
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc, 1) * 7, ForceMode2D.Impulse);

        //View Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
       
        //Animation
        anim.SetTrigger("doDamaged");
        StartCoroutine(OffDamage());
    }

    //잠시동안 플레이어 무적
    IEnumerator OffDamage()
    {
        yield return new WaitForSeconds(1f);
        gameObject.layer = 10;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    //플레이어 사망 시
    public void OnDie()
    {
        Time.timeScale = 0;
        gameObject.layer = 11;
        spriteRenderer.color = new Color(1, 1, 1, 0);
    }


    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }

    //벽에 플레이어 붙는 현상 체크
    void StopJumping()
    {
        RaycastHit2D rayHitRight = Physics2D.Raycast(transform.position, Vector3.right, 0.5f, LayerMask.GetMask("Platform"));
        RaycastHit2D rayHitLeft = Physics2D.Raycast(transform.position, Vector3.left, 0.5f, LayerMask.GetMask("Platform"));
        if (!isGrouded)
        {
            if (rayHitRight.collider != null || rayHitLeft.collider != null)
            {
                if (rayHitRight.distance < 0.01f || rayHitLeft.distance < 0.01f)
                {
                    rigid.velocity = Vector2.down;
                }
            }
        }
    }
}
