using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private int[] gameScenes;
    private int index = 1;
    private int currentStage = 0;
    public int highScore = 0;
    private int totalScenes;
    private float gameSpeed = 1f;
    private static GameManager m_instance;
    public TextMeshProUGUI stageText;
    public static GameManager instance
    {
        get
        {
            if (m_instance == null)
                m_instance = FindObjectOfType<GameManager>();
            return m_instance;
        }

    }
    void Awake()
    {
        totalScenes = SceneManager.sceneCountInBuildSettings;
        if (instance != this) { 
            Destroy(gameObject); 
            return; 
        }
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        stageText.gameObject.SetActive(false);
        gameScenes = new int[totalScenes];
        for(int i = 0; i < totalScenes; i++)
        {
            gameScenes[i] = i;
        }
        shuffleGames();
    }
    private void InitGame()
    {
        index = 1;
        currentStage = 0;
        gameSpeed = 1f;
    }
    public void shuffleGames()
    {
        for(int i = 1; i < totalScenes; i++)
        {
            int temp = gameScenes[i];
            int randomIndex = Random.Range(1, totalScenes);
            gameScenes[i] = gameScenes[randomIndex];
            gameScenes[randomIndex] = temp;
        }
    }
    public void LoadNextGame()
    {
        StartCoroutine(LoadNextScene());
    }
    public void GameClear()
    {
        StartCoroutine(ClearAndLoadNextScene());
    }
    public void GameOver()
    {
        StartCoroutine(OverAndLoadMainScene());
    }
    IEnumerator LoadNextScene()
    {
        if(index >= totalScenes)
        {
            index = 1;
            shuffleGames();
            gameSpeed += 0.15f;
        }
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(gameScenes[index++]);
        Time.timeScale = 0f;
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        stageText.text = "STAGE" + (++currentStage);
        stageText.gameObject.SetActive(true);

        yield return new WaitForSecondsRealtime(1.2f);
        stageText.text = "START";
        yield return new WaitForSecondsRealtime(0.7f);
        stageText.gameObject.SetActive(false);
        Time.timeScale = gameSpeed;
        
    }
    IEnumerator ClearAndLoadNextScene()
    {
        if (highScore < currentStage)
        {
            highScore = currentStage;
        }
        Time.timeScale = 0f;
        stageText.text = "CLEAR!!";
        stageText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(1.0f);
        stageText.gameObject.SetActive(false);
        StartCoroutine(LoadNextScene());
    }
    IEnumerator OverAndLoadMainScene()
    {
        Time.timeScale = 0f;
        stageText.text = "Game Over...";
        stageText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(2.0f);
        stageText.gameObject.SetActive(false);
        InitGame();
        Time.timeScale = gameSpeed;
        SceneManager.LoadSceneAsync(0);
    }
}
