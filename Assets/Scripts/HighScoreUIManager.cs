using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


[DefaultExecutionOrder(1000)]
public class HighScoreUIManager : MonoBehaviour
{
    public TMP_Text[] scoreTexts = new TMP_Text[3];

    void Start()
    {
        for (int i= 0; i < MainManager.Instance.highScore.Length; i++)
        {
            updateText(scoreTexts[i], MainManager.Instance.highScore[i]);
        }
    }

    public void updateText(TMP_Text oneScore, MainManager.HighScoreEntry hse)
    {
        string spaces = "                  ";
        int wantedSpaces = spaces.Length - hse.scorer.Length;
        if (wantedSpaces < 2)
        {
            wantedSpaces = 2;
        }
        spaces = spaces.Remove(wantedSpaces - 1);
        oneScore.text = $"{hse.scorer}{spaces}{hse.points}";
    }


    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }
}
