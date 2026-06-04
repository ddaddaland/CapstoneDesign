using UnityEngine;
using TMPro;

public class CatchFruitGame : MonoBehaviour
{
    public RectTransform gameArea;
    public GameObject basketPrefab;
    public GameObject fruitRedPrefab;
    public GameObject fruitYellowPrefab;
    public TMP_Text countText;
    public TMP_Text timerText;

    [Header("타이머")]
    public float timeLimit = 5f;

    private GameObject basket;
    private GameObject[] fruits = new GameObject[8];
    private int caught = 0;
    private float spawnTimer = 0.15f;
    private bool running = false;
    private bool IsCleared = false;
    private float timeLeft;

    void Start()
    {
        if (basketPrefab == null || gameArea == null) return;

        basket = Instantiate(basketPrefab, gameArea);
        float halfH = gameArea.rect.height * 0.5f;
        basket.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -(halfH - 30f));

        caught = 0;
        timeLeft = timeLimit;
        running = true;
    }

    void Update()
    {
        if (!running || basket == null) return;

        timeLeft -= Time.deltaTime;
        if (timerText != null)
            timerText.text = $"남은 시간: {Mathf.Max(0f, timeLeft):F1}";

        if (timeLeft <= 0f)
        {
            running = false;
            GameManager.instance.GameOver();
            return;
        }

        var basketRt = basket.GetComponent<RectTransform>();
        var basketPos = basketRt.anchoredPosition;
        float halfW = gameArea.rect.width * 0.5f;
        float halfH = gameArea.rect.height * 0.5f;

    
        RectTransformUtility.ScreenPointToLocalPointInRectangle(gameArea, Input.mousePosition, null, out Vector2 localPoint);
        basketPos.x = Mathf.Clamp(localPoint.x, -(halfW - 40f), halfW - 40f);
        basketRt.anchoredPosition = basketPos;
        

        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            spawnTimer = 0.65f;
            SpawnFruit(halfW, halfH);
        }

        for (int i = 0; i < fruits.Length; i++)
        {
            if (fruits[i] == null) continue;
            var rt = fruits[i].GetComponent<RectTransform>();
            var pos = rt.anchoredPosition;

            pos.y -= 600f * Time.deltaTime;
            rt.anchoredPosition = pos;

            if (Mathf.Abs(pos.x - basketPos.x) < 95f && Mathf.Abs(pos.y - basketPos.y) < 42f)
            {
                Destroy(fruits[i]);
                fruits[i] = null;
                caught++;
                if (caught >= 3 && IsCleared == false)
                {
                    running = false;
                    IsCleared = true;
                    Debug.Log("게임 성공! (클리어)");
                    GameManager.instance.GameClear();
                }
            }
            else if (pos.y < -(halfH + 20f))
            {
                Destroy(fruits[i]);
                fruits[i] = null;
            }
        }
    }

    void SpawnFruit(float halfW, float halfH)
    {
        for (int i = 0; i < fruits.Length; i++)
        {
            if (fruits[i] == null)
            {
                GameObject selectedPrefab = Random.value > 0.5f ? fruitRedPrefab : fruitYellowPrefab;
                fruits[i] = Instantiate(selectedPrefab, gameArea);
                fruits[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(Random.Range(-(halfW - 40f), halfW - 40f), halfH - 20f);
                break;
            }
        }
    }
}