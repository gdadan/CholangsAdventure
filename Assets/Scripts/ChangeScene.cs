using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public GameObject quitPopUp; //�������� �˾�â

    bool isquitOpen;

    void Update()
    {
        StartGame();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleSetting();
        }
    }

    void StartGame()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(1);
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

    //���� ����
    public void OnClickQuitBtn()
    {
        Application.Quit();
    }

    //�˾�â Close
    public void ClosePopup()
    {
        Time.timeScale = 1;
        quitPopUp.SetActive(false);
        isquitOpen = !isquitOpen;
    }
}
