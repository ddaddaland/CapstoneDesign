using UnityEngine;

namespace MiniGame
{
    public class DragToBoxGame : MicroGameBase
    {
        public override string Instruction => "넣어라!";

        [Header("Prefabs")]
        [Tooltip("목표 지점이 될 상자 프리팹을 연결하세요.")]
        public GameObject goalBoxPrefab;
        [Tooltip("마우스로 직접 드래그할 대상(별) 프리팹을 연결하세요.")]
        public GameObject starPrefab;

        private GameObject box;
        private GameObject star;

        public override void Setup()
        {
            // 에러 방지용 체크
            if (goalBoxPrefab == null || starPrefab == null)
            {
                Debug.LogError("DragToBoxGame: 인스펙터에 할당되지 않은 프리팹이 있습니다! 모두 연결해 주세요.");
                return;
            }

            // 하드코딩된 경로 대신 인스펙터에 등록된 프리팹을 생성합니다.
            box = Instantiate(goalBoxPrefab, GameArea);
            star = Instantiate(starPrefab, GameArea);

            // 상자(Box)는 오른쪽에 고정으로 배치합니다.
            box.GetComponent<RectTransform>().anchoredPosition = new Vector2(GameHalfW - 150f, 0f);

            // 별(Star)은 왼쪽 구역에서 랜덤한 위치에 나타납니다.
            star.GetComponent<RectTransform>().anchoredPosition = new Vector2(Random.Range(-(GameHalfW - 60f), -(GameHalfW * 0.3f)), Random.Range(-(GameHalfH - 100f), GameHalfH - 100f));

            // 생성된 별 오브젝트에 드래그 기능을 담당하는 DragHandler를 붙여줍니다.
            var drag = star.AddComponent<DragHandler>();
            drag.dragArea = GameArea;
            drag.onDrop = CheckClear; // 드래그를 놓았을 때 CheckClear 함수를 실행하도록 연결
        }

        private void CheckClear()
        {
            if (!running || star == null || box == null) return;

            var starPos = star.GetComponent<RectTransform>().anchoredPosition;
            var boxPos = box.GetComponent<RectTransform>().anchoredPosition;

            // 별의 중심과 상자의 중심 거리가 가까워지면(가로/세로 오차 80 이하) 클리어!
            if (Mathf.Abs(starPos.x - boxPos.x) < 80f && Mathf.Abs(starPos.y - boxPos.y) < 80f)
            {
                cleared = true;
            }
        }

        public override void Cleanup()
        {
            if (box != null) Destroy(box);
            if (star != null) Destroy(star);
        }
    }
}