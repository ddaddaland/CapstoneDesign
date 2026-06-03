using UnityEngine;

public class DodgeGame : MonoBehaviour
{
    public RectTransform gameArea;
    public GameObject playerPrefab;
    public GameObject obstaclePrefab;

    private GameObject player;
    private GameObject[] obstacles = new GameObject[10];
    private float spawnTimer = 0.2f;
    private bool running = false;

    private float surviveTimer = 0f;

    void Start()
    {
        if (playerPrefab == null || gameArea == null) return;

        player = Instantiate(playerPrefab, gameArea);
        float halfH = gameArea.rect.height * 0.5f;
        player.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -(halfH - 60f));

        // 게임 시작 시 타이머 0으로 초기화
        surviveTimer = 0f;
        running = true;
    }

    void Update()
    {
        if (!running || player == null) return;

        surviveTimer += Time.deltaTime;

        if (surviveTimer >= 5f)
        {
            running = false;
            Debug.Log("게임 성공! (5초 생존 클리어)");
            return;
        }

        MovePlayer();
        UpdateObstacles();
    }

    void MovePlayer()
    {
        float axis = 0f;
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) axis -= 1f;
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) axis += 1f;

        var rt = player.GetComponent<RectTransform>();
        var pos = rt.anchoredPosition;
        float halfW = gameArea.rect.width * 0.5f;

        if (Input.GetMouseButton(0))
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(gameArea, Input.mousePosition, null, out Vector2 localPoint);
            pos.x = Mathf.Clamp(localPoint.x, -(halfW - 40f), halfW - 40f);
        }
        else
        {
            pos.x = Mathf.Clamp(pos.x + axis * 600f * Time.deltaTime, -(halfW - 40f), halfW - 40f);
        }
        rt.anchoredPosition = pos;
    }

    void UpdateObstacles()
    {
        spawnTimer -= Time.deltaTime;
        float halfW = gameArea.rect.width * 0.5f;
        float halfH = gameArea.rect.height * 0.5f;

        if (spawnTimer <= 0f)
        {
            spawnTimer = 0.45f;
            for (int i = 0; i < obstacles.Length; i++)
            {
                if (obstacles[i] == null)
                {
                    obstacles[i] = Instantiate(obstaclePrefab, gameArea);
                    obstacles[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(Random.Range(-(halfW - 20f), halfW - 20f), halfH - 20f);
                    break;
                }
            }
        }

        var playerPos = player.GetComponent<RectTransform>().anchoredPosition;

        for (int i = 0; i < obstacles.Length; i++)
        {
            if (obstacles[i] == null) continue;
            var rt = obstacles[i].GetComponent<RectTransform>();
            var pos = rt.anchoredPosition;

            pos.y -= 380f * Time.deltaTime;
            rt.anchoredPosition = pos;

            if (Vector2.Distance(pos, playerPos) < 62f)
            {
                running = false;
                Debug.Log("게임 실패! (장애물 충돌)");
            }

            if (pos.y < -(halfH + 20f))
            {
                Destroy(obstacles[i]);
                obstacles[i] = null;
            }
        }
    }
}