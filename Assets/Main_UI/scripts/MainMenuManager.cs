using TMPro;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    void Start()
    {
        scoreText.text = "최고 기록 : " + GameManager.instance.highScore;
    }

    public void ClickStartButton()
    {
        GameManager.instance.LoadNextGame();
    }
}
