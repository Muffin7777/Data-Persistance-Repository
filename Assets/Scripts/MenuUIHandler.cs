using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

[DefaultExecutionOrder(1000)]
public class MenuUIHandler : MonoBehaviour
{
    public TMP_InputField nameInput;
    public TMP_Text nameWarning;
    public TMP_Text sizeWarning;

    public void NewLetter()
    {
        MainManager.Instance.playerName = nameInput.text;
        nameWarning.gameObject.SetActive(false);
        sizeWarning.gameObject.SetActive(false);
    }

    private void Start()
    {
        if (MainManager.Instance.playerName != null && MainManager.Instance.playerName.Length > 0)
        {
            nameInput.text = MainManager.Instance.playerName;
        }
        LoadHighScore();
    }

    public void StartNew()
    {
        if (MainManager.Instance.playerName != null && MainManager.Instance.playerName.Length > 0 && MainManager.Instance.playerName.Length < 13) {
            SceneManager.LoadScene(1);
        }
        else if (MainManager.Instance.playerName.Length >= 13)
        {
            sizeWarning.gameObject.SetActive(true);
        }
        else
        {
            nameWarning.gameObject.SetActive(true);
        }
    }


    public void goToHighScoreMenu(){
        SceneManager.LoadScene(2);
    }


    public void Exit()
    {
        SaveHighScore();

#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit(); // original code to quit Unity player
#endif
    }

  

    public void LoadHighScore()
    {
        MainManager.Instance.LoadHighScore();
    }

    public void SaveHighScore()
    {
        MainManager.Instance.SaveHighScore();
    }
}
