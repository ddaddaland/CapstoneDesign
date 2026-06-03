using UnityEngine;

namespace MiniGame
{
    public class DodgeGame : MicroGameBase
    {
        public override string Instruction => "피해라!";

        [Header("Prefabs")]
        [Tooltip("사용자가 직접 조종할 플레이어 프리팹을 연결하세요.")]
        public GameObject playerPrefab;
        [Tooltip("위에서 아래로 떨어지는 장애물 프리팹을 연결하세요.")]
        public GameObject obstaclePrefab;

        private GameObject player;
        private readonly GameObject[] obstacles = new GameObject[10];
        private float spawnTimer;
        private float surviveTimer;

        public override void Setup()
        {
            // 에러 방지용 체크
            if (playerPrefab == null || obstaclePrefab == null)
            {
                Debug.LogError("DodgeGame: 인스펙터에 할당되지 않은 프리팹이 있습니다! 모두 연결해 주세요.");
                return;
            }

            // 하드코딩된 경로 대신 인스펙터에 등록된 플레이어 프리팹을 생성합니다.
            player = Instantiate(playerPrefab, GameArea);

            // 플레이어의 시작 위치를 화면 아래쪽 중앙으로 예쁘게 잡아줍니다.
            player.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -(GameHalfH - 60f));
        }

        public override void StartGame()
        {
            base.StartGame();
            spawnTimer = 0.2f;
            surviveTimer = 0f;
        }

        private void Update()
        {
            if (!running || player == null) return;
            surviveTimer += Time.deltaTime;
            MovePlayer();
            UpdateObstacles();

            // 4.7초 동안 살아남으면 클리어!
            if (surviveTimer >= 4.7f) cleared = true;
        }

        private void MovePlayer()
        {
            var playerRt = player.GetComponent<RectTransform>();
            float axis = 0f;

            // 키보드 조작
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) axis -= 1f;
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) axis += 1f;

            // 마우스/터치 조작
            if (Input.GetMouseButton(0))
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(GameArea, Input.mousePosition, null, out var localPoint);
                playerRt.anchoredPosition = new Vector2(Mathf.Clamp(localPoint.x, -(GameHalfW - 20f), GameHalfW - 20f), -(GameHalfH - 60f));
            }
            else
            {
                var pos = playerRt.anchoredPosition;
                pos.x = Mathf.Clamp(pos.x + axis * 500f * Time.deltaTime, -(GameHalfW - 20f), GameHalfW - 20f);
                playerRt.anchoredPosition = pos;
            }
        }

        private void UpdateObstacles()
        {
            if (obstaclePrefab == null) return; // 안전 장치

            spawnTimer -= Time.deltaTime;

            // 장애물 생성 타이머
            if (spawnTimer <= 0f)
            {
                spawnTimer = 0.45f; // 0.45초마다 하나씩 생성
                for (int i = 0; i < obstacles.Length; i++)
                {
                    if (obstacles[i] != null) continue;

                    // 하드코딩된 경로 대신 obstaclePrefab을 생성합니다.
                    obstacles[i] = Instantiate(obstaclePrefab, GameArea);

                    // 화면 위쪽에서 가로축(X) 랜덤 위치에 생성
                    obstacles[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(Random.Range(-(GameHalfW - 20f), GameHalfW - 20f), GameHalfH - 20f);
                    break;
                }
            }

            var playerPos = player.GetComponent<RectTransform>().anchoredPosition;

            // 장애물 이동 및 충돌 체크
            for (int i = 0; i < obstacles.Length; i++)
            {
                if (obstacles[i] == null) continue;
                var rt = obstacles[i].GetComponent<RectTransform>();
                var pos = rt.anchoredPosition;

                // 장애물이 아래로 떨어짐
                pos.y -= 380f * Time.deltaTime;
                rt.anchoredPosition = pos;

                // 플레이어와 장애물 사이의 거리가 62 이하로 가까워지면(충돌) 게임 오버!
                if (Vector2.Distance(pos, playerPos) < 62f)
                {
                    failed = true;
                }

                // 장애물이 화면 밖으로 완전히 벗어나면 삭제
                if (pos.y < -(GameHalfH + 20f))
                {
                    Destroy(obstacles[i]);
                    obstacles[i] = null;
                }
            }
        }

        public override void Cleanup()
        {
            if (player != null) Destroy(player);
            for (int i = 0; i < obstacles.Length; i++)
            {
                if (obstacles[i] != null) Destroy(obstacles[i]);
            }
        }
    }
}