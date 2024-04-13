using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public int coinScore; //���� ����
    
    public Sprite openChest; //���� ���� �̹���

    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void AddScore()
    {
        SoundManager.instance.PlaySFX(3);
        //Coin Score ++
        GameManager.instance.score += coinScore;
        gameObject.SetActive(false);
    }

    public void AddLife()
    {
        SoundManager.instance.PlaySFX(4);
        //���++ , ���ڰ� ���� ���·� �̹��� �ٲ�� ���̻� ��ȣ�ۿ�x
        GameManager.instance.health += 1;
        spriteRenderer.sprite = openChest;
        gameObject.layer = 15;
    }
}
