using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public string[] miniGames;
    public int currentIndex = 0;
    private string currentGame;
    void Awake()
    {
        if(instance != null) { 
            Destroy(gameObject); 
            return; 
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartLoad()
    {
        StartCoroutine(StartGame());
    }
    public void NextLoad()
    {
        //StartCoroutine(NextGame());
    }
    IEnumerator StartGame()
    {
        yield return SceneManager.LoadSceneAsync(miniGames[currentIndex]); ;
        SceneManager.LoadSceneAsync("Mini_Game_UI", LoadSceneMode.Additive);
    }
    //IEnumerator NextGame(string current, string next)
    //{

    //}
}
