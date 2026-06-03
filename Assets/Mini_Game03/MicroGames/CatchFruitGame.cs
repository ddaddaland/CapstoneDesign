using UnityEngine;
using UnityEngine.UI;

namespace MiniGame
{
    public class CatchFruitGame : MicroGameBase
    {
        public override string Instruction => "받아라!";

        [Header("Prefabs")]
        [Tooltip("마우스로 조종할 바구니 프리팹을 연결하세요.")]
        public GameObject basketPrefab;
        [Tooltip("위에서 떨어질 첫 번째 과일(예: 빨간 사과) 프리팹을 연결하세요.")]
        public GameObject fruitRedPrefab;
        [Tooltip("위에서 떨어질 두 번째 과일(예: 노란 바나나) 프리팹을 연결하세요.")]
        public GameObject fruitYellowPrefab;

        private GameObject basket;
        private readonly GameObject[] fruits = new GameObject[8];
        private int caught;
        private float spawnTimer;

        public override void Setup()
        {
            // 에러 방지용 체크
            if (basketPrefab == null || fruitRedPrefab == null || fruitYellowPrefab == null)
            {
                Debug.LogError("CatchFruitGame: 인스펙터에 할당되지 않은 프리팹이 있습니다! 모두 연결해 주세요.");
                return;
            }

            // 하드코딩된 경로 대신 인스펙터에 등록된 바구니 프리팹을 생성합니다.
            basket = Instantiate(basketPrefab, GameArea);

            // 바구니 시작 위치 설정 (화면 아래쪽)
            basket.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -(GameHalfH - 30f));
        }

        public override void StartGame()
        {
            base.StartGame();
            spawnTimer = 0.15f;
            caught = 0; // 시작할 때 잡은 과일 개수 초기화
        }

        private void Update()
        {
            if (!running || basket == null) return;

            // 마우스 위치에 따라 바구니 좌우 이동
            RectTransformUtility.ScreenPointToLocalPointInRectangle(GameArea, Input.mousePosition, null, out var localPoint);
            basket.GetComponent<RectTransform>().anchoredPosition = new Vector2(Mathf.Clamp(localPoint.x, -GameHalfW + 20f, GameHalfW - 20f), -(GameHalfH - 30f));

            spawnTimer -= Time.deltaTime;

            // 과일 생성 타이머
            if (spawnTimer <= 0f)
            {
                spawnTimer = 0.65f; // 0.65초마다 과일 생성
                SpawnFruit();
            }

            var basketPos = basket.GetComponent<RectTransform>().anchoredPosition;

            // 과일 이동 및 충돌 체크
            for (int i = 0; i < fruits.Length; i++)
            {
                if (fruits[i] == null) continue;
                var rt = fruits[i].GetComponent<RectTransform>();
                var pos = rt.anchoredPosition;

                // 과일이 아래로 떨어짐
                pos.y -= 300f * Time.deltaTime;
                rt.anchoredPosition = pos;

                // 과일과 바구니 사이의 거리가 가까워지면(가로 95, 세로 42 이하) 획득!
                if (Mathf.Abs(pos.x - basketPos.x) < 95f && Mathf.Abs(pos.y - basketPos.y) < 42f)
                {
                    Destroy(fruits[i]);
                    fruits[i] = null;
                    caught++;
                    if (caught >= 3) cleared = true; // 3개 잡으면 클리어!
                }
                // 바닥으로 완전히 떨어지면 그냥 파괴
                else if (pos.y < -(GameHalfH + 20f))
                {
                    Destroy(fruits[i]);
                    fruits[i] = null;
                }
            }
        }

        private void SpawnFruit()
        {
            for (int i = 0; i < fruits.Length; i++)
            {
                if (fruits[i] != null) continue;

                // 확률(50%)에 따라 빨간 과일 또는 노란 과일 프리팹을 선택합니다.
                GameObject selectedPrefab = Random.value > 0.5f ? fruitRedPrefab : fruitYellowPrefab;

                // 선택된 프리팹을 생성합니다.
                fruits[i] = Instantiate(selectedPrefab, GameArea);

                // 화면 위쪽에서 랜덤한 가로 위치에 스폰
                fruits[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(Random.Range(-(GameHalfW - 40f), GameHalfW - 40f), GameHalfH - 20f);
                break;
            }
        }

        public override void Cleanup()
        {
            if (basket != null) Destroy(basket);
            for (int i = 0; i < fruits.Length; i++)
            {
                if (fruits[i] != null) Destroy(fruits[i]);
            }
        }
    }
}