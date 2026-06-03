using UnityEngine;

public class CatchFruitGame : MonoBehaviour
{
    public RectTransform gameArea;
    public GameObject basketPrefab;
    public GameObject fruitRedPrefab;
    public GameObject fruitYellowPrefab;

    private GameObject basket;
    private GameObject[] fruits = new GameObject[8];
    private int caught = 0;
    private float spawnTimer = 0.15f;
    private bool running = false;
    private bool IsCleared = false;

    void Start()
    {
        if (basketPrefab == null || gameArea == null) return;

        basket = Instantiate(basketPrefab, gameArea);
        float halfH = gameArea.rect.height * 0.5f;
        basket.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -(halfH - 30f));

        caught = 0;
        running = true;
    }

    void Update()
    {
        if (!running || basket == null) return;

        var basketRt = basket.GetComponent<RectTransform>();
        var basketPos = basketRt.anchoredPosition;
        float halfW = gameArea.rect.width * 0.5f;
        float halfH = gameArea.rect.height * 0.5f;

        if (Input.GetMouseButton(0))
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(gameArea, Input.mousePosition, null, out Vector2 localPoint);
            basketPos.x = Mathf.Clamp(localPoint.x, -(halfW - 40f), halfW - 40f);
            basketRt.anchoredPosition = basketPos;
        }

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

            pos.y -= 300f * Time.deltaTime;
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