using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int score; //스테이지 스코어
    public int health; //플레이어 목숨
    public int stageIndex;

    public Text scoreUI; //플레이 화면에서 보여지는 스코어UI
    public Text lifeUI; //플레이 화면에서 보여지는 목숨UI
    public Text stageUI; //플레이 화면에서 보여지는 스테이지UI
    public Text recordScore; //팝업창 최고 기록 스코어
    public Text currentScore; //팝업창 현재 기록 스코어

    public GameObject restartPopUp; //게임 재시작 팝업창
    public GameObject quitPopUp; //게임종료 팝업창
    public GameObject[] stages; //각각의 스테이지
    public GameObject startPoint; //시작 지점

    public Player player;

    bool isquitOpen;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        //플레이어 처음 시작 위치 조정
        player.transform.position = startPoint.transform.position;

        stageUI.text = "Stage " + (stageIndex + 1);
    }
    private void Update()
    {
        //스테이지 스코어UI, 플레이어 목숨 UI
        scoreUI.text = score.ToString();
        lifeUI.text = ("X " + health).ToString();

        //게임 종료 팝업창 활성화
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleSetting();
        }
    }

    void ToggleSetting()
    {
        isquitOpen = !isquitOpen;
        quitPopUp.SetActive(isquitOpen);

        if (isquitOpen)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //플레이어 낙사 체크
        if (collision.gameObject.CompareTag("Player"))
        {
            //Health Down
            HealthDown();

            if (health > 0)
            {
                //Player Reposition
                PlayerReposition();
            }
        }
    }

    public void PlayerReposition()
    {
        //플레이어 Start지점으로 다시 돌려보냄
        player.transform.position = startPoint.transform.position;
        player.VelocityZero();
    }

    public void NextStage()
    {
        //스테이지 클리어 시 다음 스테이지로, 모두 클리어 시 게임 재시작 팝업창 띄움
        if (stageIndex < stages.Length - 1)
        {
            SoundManager.instance.PlaySFX(2);

            stages[stageIndex].SetActive(false);
            stageIndex++;
            stages[stageIndex].SetActive(true);
            PlayerReposition();

            stageUI.text = "Stage " + (stageIndex + 1);
        }
        else
        {
            SoundManager.instance.PlaySFX(1);

            //Player Contol Lock
            Time.timeScale = 0;
            
            //Restart Button UI
            Text btnText = restartPopUp.GetComponentInChildren<Text>();
            btnText.text = "게임 성공!";
            btnText.color = new Color(0, 170, 255);
            restartPopUp.SetActive(true);
            RecordScore();
        }
    }

    public void HealthDown()
    {
        //플레이어가 맞을 시 체력--, 목숨이 없을 시 재시작 팝업창
        if (health > 1)
        {
            SoundManager.instance.PlaySFX(7);

            health--;
        }
        else
        {
            health = 0;
            SoundManager.instance.PlaySFX(0);

            //Plyer Die Effect
            player.OnDie();

            //재시작버튼 UI 활성화
            restartPopUp.SetActive(true);
            RecordScore();
        }
    }

    //게임 재시작
    public void OnClickRestartBtn()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(1);

        SoundManager.instance.bgmAudioSource.Stop();
        SoundManager.instance.BGMStart();
    }

    //메인 화면으로 나가기
    public void OnClickTitleBtn()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);

        SoundManager.instance.bgmAudioSource.Stop();
        SoundManager.instance.BGMStart();
    }
    
    //게임 나가기 버튼 클릭 시 게임 종료
    public void OnClickQuitBtn()
    {
        Application.Quit();
    }

    //팝업창 Close
    public void ClosePopup()
    {
        quitPopUp.SetActive(false);
        isquitOpen = !isquitOpen;
        Time.timeScale = 1;
    }

    //최고기록 저장
    public void RecordScore()
    {
        int highScore = PlayerPrefs.GetInt("HighScore");

        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
        }
        recordScore.text = "최고 점수 : " + highScore.ToString();
        currentScore.text = "점수 : " + score.ToString();
    }
}
