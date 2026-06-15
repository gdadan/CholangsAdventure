using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//몬스터 중 RockHead 움직이는 관련 스크립트
public class AttackArea : MonoBehaviour
{
    public RockHead rockHead;
  
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 10)
        {
            //플레이어가 범위에 들어오면 RockHead 움직임
            if (!rockHead.isMoving)
                rockHead.state = "RockHeadMoving";
        }
    }
}
