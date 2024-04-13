using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockHead : MonoBehaviour
{
    public string state; //RockHead ����

    [SerializeField]
    float moveSpeed; //�����̴� �ӵ�

    public bool isMoving; //�����̰� �ִ� �� üũ
    public bool isGrounded; //���� ���� �� üũ

    public float stoppingDistance; //������ ���ߴ� �Ÿ�

    Rigidbody2D rigid;

    Vector3 oriPos; //ó�� RockHead ��ġ

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
            //�����̰� ���� ���� ��, ���� ���������� �Ʒ��� �̵�
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
            //���� ������ ���� �̵� �� ���� ��ȭ
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
        //�ð��� ���� ���� �� ���� �̵��ϱ� ���� isGrounded ��ȭ
        yield return new WaitForSecondsRealtime(0.7f);
        isGrounded = true;
    }
}
