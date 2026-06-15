using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public int coinScore; //코인 점수
    
    public Sprite openChest; //상자 열린 이미지

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
        //목숨++ , 상자가 열린 상태로 이미지 바뀌고 더이상 상호작용x
        GameManager.instance.health += 1;
        spriteRenderer.sprite = openChest;
        gameObject.layer = 15;
    }
}
