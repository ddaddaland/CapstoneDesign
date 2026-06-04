using UnityEngine;
using TMPro;

public class MashButtonGame : MonoBehaviour
{
    public RectTransform gameArea;
    public GameObject barBackgroundPrefab;
    public GameObject barFillPrefab;
    public TMP_Text instructionText;
    public TMP_Text timerText;

    [Header("타이머")]
    public float timeLimit = 5f;

    private GameObject barBackground;
    private GameObject barFill;
    private float progress;
    private bool running = false;
    private bool IsCleared = false;
    private float timeLeft;

    void Start()
    {
        if (barBackgroundPrefab == null || barFillPrefab == null || gameArea == null) return;

        barBackground = Instantiate(barBackgroundPrefab, gameArea);
        barFill = Instantiate(barFillPrefab, gameArea);

        barBackground.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        barFill.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        barFill.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 48f);

        progress = 0f;
        timeLeft = timeLimit;
        running = true;
    }

    void Update()
    {
        if (!running || barFill == null) return;

        timeLeft -= Time.deltaTime;
        if (timerText != null)
            timerText.text = $"남은 시간: {Mathf.Max(0f, timeLeft):F1}";

        if (timeLeft <= 0f)
        {
            running = false;
            GameManager.instance.GameOver();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            progress += 0.08f;

        progress = Mathf.Max(0f, progress - Time.deltaTime * 0.04f);

        float maxBarWidth = gameArea.rect.width - 80f;
        barFill.GetComponent<RectTransform>().sizeDelta = new Vector2(maxBarWidth * Mathf.Clamp01(progress), 48f);

        if (progress >= 1f && IsCleared == false)
        {
            running = false;
            IsCleared = true;
            Debug.Log("게임 성공! (클리어)");
            GameManager.instance.GameClear();
        }
    }
}