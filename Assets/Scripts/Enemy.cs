using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enemy;

public class Enemy : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    Rigidbody2D rigid;
    Animator anim;

    public GameObject plantBulletObj; //Plant 총알
    public GameObject player;

    [SerializeField]
    float curShotDelay; //현재 총알 시간
    [SerializeField]
    float maxShotDelay; //총알 딜레이
    [SerializeField]
    int nextMove; //방향 바꾸기
    [SerializeField]
    int enemyScore; //몬스터 점수

    public EnemyType enemyType;

    //몬스터 타입 구분
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

        //몬스터타입마다 몬스터 각자의 패턴 실행
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

        //몬스터 맞는 애니메이션 실행
        anim.SetBool("isHitting", true);

        //몬스터 삭제
        StartCoroutine(DeActive());
    }

    //몬스터가 원래 가는 방향에서 반대 방향으로
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

    //몬스터 타입 중 Mushroom 움직임
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
            //플랫폼이 없으면 몬스터가 현재 방향과 반대로 돌아감
            Turn();
        }
    }

    //몬스터 타입 중 Chicken, Mushroom보다 빠르게 움직임
    void Chicken()
    {
        Mushroom(4.5f, 0.4f);
    }

    //몬스터 타입 중 Plant 총알 발사
    void Plant()
    {
        if (curShotDelay < maxShotDelay)
            return;

        //식물이 maxShotDelay마다 왼쪽으로 총알 쏨
        GameObject plantBullet = Instantiate(plantBulletObj, transform.position, transform.rotation);
        Rigidbody2D rigid = plantBullet.GetComponent<Rigidbody2D>();

        rigid.AddForce(Vector2.left * 3, ForceMode2D.Impulse);

        curShotDelay = 0;
        //식물이 쏜 총알 일정시간 후에 파괴
        Destroy(plantBullet, 6f);
    }
}
