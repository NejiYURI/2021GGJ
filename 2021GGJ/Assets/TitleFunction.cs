using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleFunction : MonoBehaviour
{
    public GameObject TutotialPanel;
    public void PlayGame()
    {
        SceneManager.LoadScene("PlayScene");
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void TutorialDialog()
    {
        TutotialPanel.SetActive(!TutotialPanel.activeInHierarchy);
    }
}
