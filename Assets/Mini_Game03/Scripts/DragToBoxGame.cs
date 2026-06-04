using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class DragToBoxGame : MonoBehaviour
{
    public RectTransform gameArea;
    public GameObject goalBoxPrefab;
    public GameObject starPrefab;
    public TMP_Text instructionText;
    public TMP_Text timerText;

    [Header("타이머")]
    public float timeLimit = 5f;

    private GameObject box;
    private GameObject star;
    private bool running = false;
    private bool IsCleared = false;
    private float timeLeft;

    void Start()
    {
        if (goalBoxPrefab == null || starPrefab == null || gameArea == null) return;

        float halfW = gameArea.rect.width * 0.5f;
        float halfH = gameArea.rect.height * 0.5f;

        box = Instantiate(goalBoxPrefab, gameArea);
        star = Instantiate(starPrefab, gameArea);

        box.GetComponent<RectTransform>().anchoredPosition = new Vector2(halfW - 150f, 0f);
        star.GetComponent<RectTransform>().anchoredPosition = new Vector2(
            Random.Range(-(halfW - 60f), -(halfW * 0.3f)),
            Random.Range(-(halfH - 100f), halfH - 100f));

        var drag = star.AddComponent<SimpleDragHandler>();
        drag.dragArea = gameArea;
        drag.onDrop = CheckClear;

        timeLeft = timeLimit;
        running = true;
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

    void CheckClear()
    {
        if (!running || star == null || box == null) return;

        var starPos = star.GetComponent<RectTransform>().anchoredPosition;
        var boxPos = box.GetComponent<RectTransform>().anchoredPosition;

        if (Mathf.Abs(starPos.x - boxPos.x) < 80f && Mathf.Abs(starPos.y - boxPos.y) < 80f && IsCleared == false)
        {
            running = false;
            IsCleared = true;
            Destroy(star);
            Debug.Log("게임 성공! (클리어)");
            GameManager.instance.GameClear();
        }
    }
}

// 통합된 드래그 핸들러
public class SimpleDragHandler : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public RectTransform dragArea;
    public System.Action onDrop;

    public void OnDrag(PointerEventData eventData)
    {
        if (dragArea == null) return;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(dragArea, eventData.position, eventData.pressEventCamera, out var localPoint);
        GetComponent<RectTransform>().anchoredPosition = localPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        onDrop?.Invoke();
    }
}