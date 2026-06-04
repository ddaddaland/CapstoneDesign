using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TapTargetGame : MonoBehaviour
{
    public RectTransform gameArea;
    public GameObject targetPrefab;
    public TMP_Text instructionText;
    public TMP_Text countText;
    public TMP_Text timerText;

    [Header("타이머")]
    public float timeLimit = 5f;

    private GameObject currentTarget;
    private int hitCount = 0;
    private const int RequiredHits = 3;
    private bool running = false;
    private bool IsCleared = false;
    private float timeLeft;

    void Start()
    {
        hitCount = 0;
        timeLeft = timeLimit;
        running = true;
        SpawnTarget();
    }

    void Update()
    {
        if (!running) return;

        timeLeft -= Time.deltaTime;
        if (timerText != null)
            timerText.text = $"남은 시간: {Mathf.Max(0f, timeLeft):F1}";

        if (timeLeft <= 0f)
        {
            running = false;
            GameManager.instance.GameOver();
        }
    }

    void SpawnTarget()
    {
        if (currentTarget != null) Destroy(currentTarget);
        if (targetPrefab == null || gameArea == null) return;

        float halfW = gameArea.rect.width * 0.5f;
        float halfH = gameArea.rect.height * 0.5f;

        currentTarget = Instantiate(targetPrefab, gameArea);
        currentTarget.GetComponent<RectTransform>().anchoredPosition = new Vector2(
            Random.Range(-(halfW - 80f), halfW - 80f),
            Random.Range(-(halfH - 80f), halfH - 80f));

        var button = currentTarget.GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnTapTarget);
    }

    void OnTapTarget()
    {
        if (!running) return;

        hitCount++;

        if (countText != null)
            countText.text = $"남은 타겟 {hitCount} / {RequiredHits}";

        if (hitCount >= RequiredHits && IsCleared == false)
        {
            running = false;
            IsCleared = true;
            Debug.Log("게임 성공! (클리어)");
            if (currentTarget != null) Destroy(currentTarget);
            GameManager.instance.GameClear();
        }
        SpawnTarget();
    }
}