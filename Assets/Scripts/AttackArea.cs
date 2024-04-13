using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//���� �� RockHead �����̴� ���� ��ũ��Ʈ
public class AttackArea : MonoBehaviour
{
    public RockHead rockHead;
  
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 10)
        {
            //�÷��̾ ������ ������ RockHead ������
            if (!rockHead.isMoving)
                rockHead.state = "RockHeadMoving";
        }
    }
}
