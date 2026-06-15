using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingCloud : MonoBehaviour
{
    [SerializeField]
    float speed; //구름 이동 속도

    void Update()
    {
        //구름을 왼쪽으로 계속 이동
        transform.Translate(Vector3.left * speed * Time.deltaTime);
    }
}
