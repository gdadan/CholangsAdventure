using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int score; //�������� ���ھ�
    public int health; //�÷��̾� ���
    public int stageIndex;

    public Text scoreUI; //�÷��� ȭ�鿡�� �������� ���ھ�UI
    public Text lifeUI; //�÷��� ȭ�鿡�� �������� ���UI
    public Text stageUI; //�÷��� ȭ�鿡�� �������� ��������UI
    public Text recordScore; //�˾�â �ְ� ��� ���ھ�
    public Text currentScore; //�˾�â ���� ��� ���ھ�

    public GameObject restartPopUp; //���� ����� �˾�â
    public GameObject quitPopUp; //�������� �˾�â
    public GameObject[] stages; //������ ��������
    public GameObject startPoint; //���� ����

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
        
        //�÷��̾� ó�� ���� ��ġ ����
        player.transform.position = startPoint.transform.position;

        stageUI.text = "Stage " + (stageIndex + 1);
    }
    private void Update()
    {
        //�������� ���ھ�UI, �÷��̾� ��� UI
        scoreUI.text = score.ToString();
        lifeUI.text = ("X " + health).ToString();

        //���� ���� �˾�â Ȱ��ȭ
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
        //�÷��̾� ���� üũ
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
        //�÷��̾� Start�������� �ٽ� ��������
        player.transform.position = startPoint.transform.position;
        player.VelocityZero();
    }

    public void NextStage()
    {
        //�������� Ŭ���� �� ���� ����������, ��� Ŭ���� �� ���� ����� �˾�â ���
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
            btnText.text = "���� ����!";
            btnText.color = new Color(0, 170, 255);
            restartPopUp.SetActive(true);
            RecordScore();
        }
    }

    public void HealthDown()
    {
        //�÷��̾ ���� �� ü��--, ����� ���� �� ����� �˾�â
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

            //����۹�ư UI Ȱ��ȭ
            restartPopUp.SetActive(true);
            RecordScore();
        }
    }

    //���� �����
    public void OnClickRestartBtn()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(1);

        SoundManager.instance.bgmAudioSource.Stop();
        SoundManager.instance.BGMStart();
    }

    //���� ȭ������ ������
    public void OnClickTitleBtn()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);

        SoundManager.instance.bgmAudioSource.Stop();
        SoundManager.instance.BGMStart();
    }
    
    //���� ������ ��ư Ŭ�� �� ���� ����
    public void OnClickQuitBtn()
    {
        Application.Quit();
    }

    //�˾�â Close
    public void ClosePopup()
    {
        quitPopUp.SetActive(false);
        isquitOpen = !isquitOpen;
        Time.timeScale = 1;
    }

    //�ְ��� ����
    public void RecordScore()
    {
        int highScore = PlayerPrefs.GetInt("HighScore");

        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
        }
        recordScore.text = "�ְ� ���� : " + highScore.ToString();
        currentScore.text = "���� : " + score.ToString();
    }
}
