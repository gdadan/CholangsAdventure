using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockHead : MonoBehaviour
{
    public string state; //RockHead 상태

    [SerializeField]
    float moveSpeed; //움직이는 속도

    public bool isMoving; //움직이고 있는 지 체크
    public bool isGrounded; //땅에 닿은 지 체크

    public float stoppingDistance; //움직임 멈추는 거리

    Rigidbody2D rigid;

    Vector3 oriPos; //처음 RockHead 위치

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        oriPos = transform.position;
        isMoving = false;
        isGrounded = false;
    }
    private void Update()
    {
        switch (state)
        {
            case ("RockHeadMoving"):
                Move();
                break;
            case ("RockHeadIdle"):
                Debug.Log("A");
                isMoving = false;
                rigid.velocity = Vector3.zero;
                break;
        }
    }
    
    public void Move()
    {
        float distance = Vector2.Distance(transform.position, oriPos);

        if (!isMoving)
        {
            //움직이고 있지 않을 때, 땅에 닿을때까지 아래로 이동
            transform.Translate(Vector2.down * moveSpeed * Time.deltaTime);
            if (distance > stoppingDistance)
            {
                isMoving = true;
                SoundManager.instance.PlaySFX(8);
                StartCoroutine(WaitTime());
            }

        }

        if (isGrounded)
        {
            //땅에 닿으면 위로 이동 후 상태 변화
            transform.Translate(Vector2.up * moveSpeed * Time.deltaTime);
            if (distance < 0.05f)
            {
                isGrounded = false;
                state = "RockHeadIdle";
            }
        }
    }

    IEnumerator WaitTime()
    {
        //시간이 조금 지난 후 위로 이동하기 위한 isGrounded 변화
        yield return new WaitForSecondsRealtime(0.7f);
        isGrounded = true;
    }
}
