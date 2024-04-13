using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField]
    float maxUpPos; //위쪽 최대 거리

    [SerializeField]
    float maxDownPos; //아래쪽 최대 거리

    [SerializeField]
    float speed; //발판 움직이는 속도

    float curYPos = 0f; //현재 y값

    Vector3 oriPos; //처음 위치

    private void Start()
    {
        oriPos = transform.position;
    }

    void Update()
    {
        //발판이 위아래로 일정하게 움직임
        curYPos += speed  * Time.deltaTime;

        if (curYPos > maxUpPos || curYPos < maxDownPos)
        {
            speed *= -1;
        }
        
        transform.position = new Vector3(oriPos.x, oriPos.y + curYPos, 0);
    }
}
