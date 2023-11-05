using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class MainManager : MonoBehaviour
{
    public static MainManager Instance;

    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public Text BestScoreText;
    public GameObject GameOverText;


    public string playerName;
    private bool m_Started;
    private int m_Points;
    private int highest_Points;
    private string highest_Scorer;

    private bool m_GameOver;


    private void Awake()
    {
        if (Instance != null)
        {
            InitializeGameObjects();
            Instance.StartNew();
            Destroy(gameObject);
            return;
        }
        Instance = this;
        m_GameOver = false;
        m_Started = false;
        DontDestroyOnLoad(gameObject);
    }

    /*Initialse gameObjects*/
    private void InitializeGameObjects()
    {
        if (Instance.BrickPrefab == null)
        {
            Instance.BrickPrefab = this.BrickPrefab;
        }
        if (Instance.Ball == null)
        {
            Instance.Ball = this.Ball;
        }
        if (Instance.ScoreText == null)
        {
            Instance.ScoreText = this.ScoreText;
        }
        if (Instance.BestScoreText == null)
        {
            Instance.BestScoreText = this.BestScoreText;
        }
        if (Instance.GameOverText == null)
        {
            Instance.GameOverText = this.GameOverText;
        }
       
    }

    // Start is called before the first frame update
    void StartNew()
    {
        m_GameOver = false;
        m_Started = false;
        m_Points = 0;
        UpdateBestScoreText();
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);

        int[] pointCountArray = new[] { 1, 1, 2, 2, 5, 5 };
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
    }


    private void Update()
    {

        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                Debug.Log("StartKeyDown");
                Debug.Log(m_Started);
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(1);
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SceneManager.LoadScene(0);
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
    }

    void UpdateBestScoreText()
    {
        BestScoreText.text = $"Best Score : {highest_Scorer} : {highest_Points}";
    } 


    public void GameOver()
    {
        m_GameOver = true;
        GameOverText.SetActive(true);
        if (m_Points > highest_Points)
        {
            highest_Points = m_Points;
            highest_Scorer = playerName;
            UpdateBestScoreText();
            SaveHighScore();

        }
    }



    /*Persist data*/ 
    [System.Serializable]
    class SaveData
    {
        public int highestScore;
        public string highestScorer;
    }


    public void SaveHighScore()
    {
        SaveData data = new SaveData();
        data.highestScore = highest_Points;
        data.highestScorer = highest_Scorer;

        string json = JsonUtility.ToJson(data);

        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    public void LoadHighScore()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            highest_Scorer = data.highestScorer;
            highest_Points = data.highestScore;
        }
        else
        {
            highest_Scorer = "none";
            highest_Points = 0;
        }
    }
}
