using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingCloud : MonoBehaviour
{
    [SerializeField]
    float speed; //���� �̵� �ӵ�

    void Update()
    {
        //������ �������� ��� �̵�
        transform.Translate(Vector3.left * speed * Time.deltaTime);
    }
}
