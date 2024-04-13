using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float maxSpeed; //�÷��̾� �ְ� �ӵ�
    public float jumpPower; //�÷��̾� ����

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
        //�÷��̾� ����
        if (Input.GetKeyDown(KeyCode.Space) && !anim.GetBool("isJumping"))
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("isJumping", true);

            SoundManager.instance.PlaySFX(5);
        }
        else if (Input.GetKeyUp(KeyCode.Space) && rigid.velocity.y > 0)
        {
            //�����̽� ��ư���� ���� ���� ���� && �ӵ��� y ���� ������(���� ��� ��)
            //���� �ӵ��� �������� ����
            rigid.velocity = rigid.velocity * 0.5f;
            isGrouded = false;
        }

        //����Ű�� ���� �ӵ� ���߱�
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
        }

        //�÷��̾� ���� ���� ��ȯ
        if (Input.GetButton("Horizontal"))
        {
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }

        //�ִϸ��̼� ��ȯ
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
        //�÷��̾� ������
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        //�÷��̾� �ְ�ӵ� ����
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

        
        //�������� �������ϴ� ������� �ִϸ��̼� ��ȯ
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
        //�� ������ ������ ����, �ٸ� ���� ������ ������
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

        //������ũ�� ������ ������
        if (collision.gameObject.CompareTag("Spike"))
        {
            OnDamaged(collision.transform.position);
        }

        //�Ѿ˿� ������ �������� �ް� �Ѿ� ����
        if (collision.gameObject.CompareTag("EnemyBullet"))
        {
            OnDamaged(collision.transform.position);
            Destroy(collision.gameObject);
        }

        //�����̴� �÷����� ������ �� �÷��� �ڽ�����
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
        //���ο� ���� �� Score++
        if (collision.gameObject.CompareTag("Coin"))
        {
            collision.gameObject.GetComponent<Item>().AddScore();         
        }

        //���ڿ� ������ ��� +1
        if (collision.gameObject.CompareTag("BonusLife"))
        {
            collision.gameObject.GetComponent<Item>().AddLife();
        }

        //������� ������ ���� ����������
        if (collision.gameObject.CompareTag("EndPoint"))
        {
            GameManager.instance.NextStage();
        }
    }

    //�÷��̾� ���� ��
    void OnAttack(Transform enemy)
    {
        SoundManager.instance.PlaySFX(6);

        //Reaction Force
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);

        //Enemy Die
        Enemy enemyLogic = enemy.GetComponent<Enemy>();
        enemyLogic.OnDamaged();
    }

    //�÷��̾ ������ ���� ��
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

    //��õ��� �÷��̾� ����
    IEnumerator OffDamage()
    {
        yield return new WaitForSeconds(1f);
        gameObject.layer = 10;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    //�÷��̾� ��� ��
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

    //���� �÷��̾� �ٴ� ���� üũ
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
