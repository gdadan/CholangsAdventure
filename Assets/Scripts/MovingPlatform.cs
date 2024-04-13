using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField]
    float maxUpPos; //���� �ִ� �Ÿ�

    [SerializeField]
    float maxDownPos; //�Ʒ��� �ִ� �Ÿ�

    [SerializeField]
    float speed; //���� �����̴� �ӵ�

    float curYPos = 0f; //���� y��

    Vector3 oriPos; //ó�� ��ġ

    private void Start()
    {
        oriPos = transform.position;
    }

    void Update()
    {
        //������ ���Ʒ��� �����ϰ� ������
        curYPos += speed  * Time.deltaTime;

        if (curYPos > maxUpPos || curYPos < maxDownPos)
        {
            speed *= -1;
        }
        
        transform.position = new Vector3(oriPos.x, oriPos.y + curYPos, 0);
    }
}
