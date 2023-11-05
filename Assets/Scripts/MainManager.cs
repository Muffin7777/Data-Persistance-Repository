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

    public class HighScoreEntry
    {
        public int position;
        public int points;
        public string scorer;
    }

    public HighScoreEntry[] highScore = new HighScoreEntry[3];
    private bool m_GameOver;


    private void Awake()
    {

        if (Instance != null && SceneManager.GetActiveScene().buildIndex == 1)
        {
            InitializeGameObjects();
            Instance.StartNew();
            Destroy(gameObject);
            return;
        }else if(Instance != null)
        {
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
        ScoreText.text = $"Player: {playerName}  Score : {m_Points}";
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
        ScoreText.text = $"Player: {playerName}  Score : {m_Points}";
    }

    void UpdateBestScoreText()
    {
        BestScoreText.text = $"Best Score : {highScore[0].scorer} : {highScore[0].points}";
    } 


    public void GameOver()
    {
        m_GameOver = true;
        GameOverText.SetActive(true);
        if (m_Points > highScore[2].points)
        {
            FindPositionAndUpdateHighScore();

            UpdateBestScoreText();
            SaveHighScore();

        }
    }

    public void FindPositionAndUpdateHighScore()
    {
        int arrayPos = 2;
        if (m_Points > highScore[1].points)
        {
            highScore[2].points = highScore[1].points;
            highScore[2].scorer = highScore[1].scorer;
            arrayPos = 1;
        }
        if (m_Points > highScore[0].points)
        {
            highScore[1].points = highScore[0].points;
            highScore[1].scorer = highScore[0].scorer;
            arrayPos = 0;
        }
        highScore[arrayPos].points = m_Points;
        highScore[arrayPos].scorer = playerName;
    }

    /*Persist data*/ 
    [System.Serializable]
    class SaveData
    {

        public int highScoreFirstPosition;
        public int highScoreFirstPoints;
        public string highScoreFirstScorer;

        public int highScoreSecondPosition;
        public int highScoreSecondPoints;
        public string highScoreSecondScorer;

        public int highScoreThirdPosition;
        public int highScoreThirdPoints;
        public string highScoreThirdScorer;

    }




    public void SaveHighScore()
    {
        SaveData data = new();

        data.highScoreFirstPoints = highScore[0].points;
        data.highScoreFirstScorer = highScore[0].scorer;
        data.highScoreFirstPosition = highScore[0].position;

        data.highScoreSecondPoints = highScore[1].points;
        data.highScoreSecondScorer = highScore[1].scorer;
        data.highScoreSecondPosition = highScore[1].position;

        data.highScoreThirdPoints = highScore[2].points;
        data.highScoreThirdScorer = highScore[2].scorer;
        data.highScoreThirdPosition = highScore[2].position;


        string json = JsonUtility.ToJson(data);

        File.WriteAllText(Application.persistentDataPath + "/savedHighScore.json", json);
    }

    public void LoadHighScore()
    {
        string path = Application.persistentDataPath + "/savedHighScore.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            highScore[0] = new HighScoreEntry();
            highScore[0].position = data.highScoreFirstPosition;
            highScore[0].points = data.highScoreFirstPoints;
            highScore[0].scorer = data.highScoreFirstScorer;

            highScore[1] = new HighScoreEntry();
            highScore[1].position = data.highScoreSecondPosition;
            highScore[1].points = data.highScoreSecondPoints;
            highScore[1].scorer = data.highScoreSecondScorer;

            highScore[2] = new HighScoreEntry();
            highScore[2].position = data.highScoreThirdPosition;
            highScore[2].points = data.highScoreThirdPoints;
            highScore[2].scorer = data.highScoreThirdScorer;
         
        }
        else
        {
            FillHighScoreWithDefaultValues();
        }
    }

    public void FillHighScoreWithDefaultValues()
    {
        for (int i = 0; i < 3; i++)
        {
            highScore[i] = new HighScoreEntry();
            highScore[i].position = i;
            highScore[i].points = 0;
            highScore[i].scorer = "none";
        }
    }


}
