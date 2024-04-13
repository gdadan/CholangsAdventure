using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enemy;

public class Enemy : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    Rigidbody2D rigid;
    Animator anim;

    public GameObject plantBulletObj; //Plant �Ѿ�
    public GameObject player;

    [SerializeField]
    float curShotDelay; //���� �Ѿ� �ð�
    [SerializeField]
    float maxShotDelay; //�Ѿ� ������
    [SerializeField]
    int nextMove; //���� �ٲٱ�
    [SerializeField]
    int enemyScore; //���� ����

    public EnemyType enemyType;

    //���� Ÿ�� ����
    public enum EnemyType
    {
        Mushroom,
        Chicken,
        Plant
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        curShotDelay += Time.deltaTime;

        //����Ÿ�Ը��� ���� ������ ���� ����
        switch (enemyType)
        {
            case EnemyType.Mushroom:
                Mushroom(1f,0.3f);
                break;
            case EnemyType.Chicken:
                Chicken();
                break;
            case EnemyType.Plant:
                Plant();
                break;
        }
    }

    public void OnDamaged()
    {        
        //score++
        GameManager.instance.score += enemyScore;

        //���� �´� �ִϸ��̼� ����
        anim.SetBool("isHitting", true);

        //���� ����
        StartCoroutine(DeActive());
    }

    //���Ͱ� ���� ���� ���⿡�� �ݴ� ��������
    void Turn()
    {
        nextMove *= -1;
        spriteRenderer.flipX = nextMove == 1;
    }

    IEnumerator DeActive()
    {
        //Destroy Enemy
        yield return new WaitForSeconds(0.3f);
        gameObject.SetActive(false);
    }

    //���� Ÿ�� �� Mushroom ������
    public void Mushroom(float enemyMoveSpeed, float layFront)
    {
        //Move
        rigid.velocity = new Vector2(nextMove * enemyMoveSpeed, rigid.velocity.y);

        //Platform Check
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * layFront, rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));

        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 2, LayerMask.GetMask("Platform"));

        if (rayHit.collider == null)
        {
            //�÷����� ������ ���Ͱ� ���� ����� �ݴ�� ���ư�
            Turn();
        }
    }

    //���� Ÿ�� �� Chicken, Mushroom���� ������ ������
    void Chicken()
    {
        Mushroom(4.5f, 0.4f);
    }

    //���� Ÿ�� �� Plant �Ѿ� �߻�
    void Plant()
    {
        if (curShotDelay < maxShotDelay)
            return;

        //�Ĺ��� maxShotDelay���� �������� �Ѿ� ��
        GameObject plantBullet = Instantiate(plantBulletObj, transform.position, transform.rotation);
        Rigidbody2D rigid = plantBullet.GetComponent<Rigidbody2D>();

        rigid.AddForce(Vector2.left * 3, ForceMode2D.Impulse);

        curShotDelay = 0;
        //�Ĺ��� �� �Ѿ� �����ð� �Ŀ� �ı�
        Destroy(plantBullet, 6f);
    }
}
