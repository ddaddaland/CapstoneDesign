using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MemoryColorGame : MonoBehaviour
{
    public RectTransform gameArea;
    public GameObject previewPrefab;
    public GameObject choicePrefab;
    public TMP_Text instructionText;
    public TMP_Text timerText;

    [Header("타이머")]
    public float timeLimit = 3f;

    private Color[] palette = {
        new Color(1f, 0.3f, 0.3f), new Color(0.25f, 1f, 0.35f),
        new Color(0.25f, 0.55f, 1f), new Color(1f, 0.88f, 0.15f)
    };

    private Color targetColor;
    private GameObject preview;
    private GameObject[] choices = new GameObject[4];
    private bool running = false;
    private bool IsCleared = false;
    private bool IsOver = false;
    private float timeLeft;
    private bool timerActive = false;

    void Start()
    {
        if (timerText != null) timerText.text = "";

        targetColor = palette[Random.Range(0, palette.Length)];
        preview = Instantiate(previewPrefab, gameArea);
        preview.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 60f);
        preview.GetComponent<Image>().color = targetColor;

        StartCoroutine(ShowChoices());
    }

    void Update()
    {
        if (!timerActive || !running) return;

        timeLeft -= Time.deltaTime;
        if (timerText != null)
            timerText.text = $"남은 시간: {Mathf.Max(0f, timeLeft):F1}";

        if (timeLeft <= 0f)
        {
            timerActive = false;
            running = false;
            if (IsOver == false)
            {
                IsOver = true;
                GameManager.instance.GameOver();
            }
        }
    }

    IEnumerator ShowChoices()
    {
        yield return new WaitForSeconds(0.9f);
        if (preview != null) Destroy(preview);

        timeLeft = timeLimit;
        timerActive = true;

        running = true;
        int[] order = { 0, 1, 2, 3 };
        for (int i = 0; i < order.Length; i++)
        {
            int j = Random.Range(i, order.Length);
            int temp = order[i]; order[i] = order[j]; order[j] = temp;
        }

        float halfW = gameArea.rect.width * 0.5f;
        float halfH = gameArea.rect.height * 0.5f;
        float spacing = (halfW * 2f - 400f) / 3f;
        float startX = -(spacing * 1.5f);
        float btnY = -(halfH - 120f);

        for (int i = 0; i < 4; i++)
        {
            Color color = palette[order[i]];
            var go = Instantiate(choicePrefab, gameArea);
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(startX + i * spacing, btnY);
            go.GetComponent<Image>().color = color;
            choices[i] = go;

            var button = go.GetComponent<Button>();
            button.onClick.AddListener(() => OnChoice(color));
        }
    }

    void OnChoice(Color selected)
    {
        if (!running) return;

        timerActive = false;
        running = false;
        if (selected == targetColor && IsCleared == false)
        {
            IsCleared = true;
            Debug.Log("게임 성공! (클리어)");
            GameManager.instance.GameClear();
        }
        else if (IsOver == false)
        {
            IsOver = true;
            Debug.Log("게임 실패!");
            GameManager.instance.GameOver();
        }
    }
}