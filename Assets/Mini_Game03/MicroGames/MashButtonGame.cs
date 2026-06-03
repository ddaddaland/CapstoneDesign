using UnityEngine;

namespace MiniGame
{
    public class MashButtonGame : MicroGameBase
    {
        public override string Instruction => "눌러라!";

        [Header("Prefabs")]
        [Tooltip("게이지 바의 배경 프리팹을 연결하세요.")]
        public GameObject barBackgroundPrefab;
        [Tooltip("게이지 바의 채워지는 부분(Fill) 프리팹을 연결하세요.")]
        public GameObject barFillPrefab;
        [Tooltip("힌트 라벨(텍스트/이미지) 프리팹을 연결하세요.")]
        public GameObject hintPrefab;

        private GameObject barBackground;
        private GameObject barFill;
        private GameObject hint;
        private float progress;

        public override void Setup()
        {
            // 에러 방지용 체크
            if (barBackgroundPrefab == null || barFillPrefab == null || hintPrefab == null)
            {
                Debug.LogError("MashButtonGame: 인스펙터에 할당되지 않은 프리팹이 있습니다! 모두 연결해 주세요.");
                return;
            }

            // 하드코딩된 경로 대신 인스펙터에 등록된 프리팹들을 생성합니다.
            barBackground = Instantiate(barBackgroundPrefab, GameArea);
            barFill = Instantiate(barFillPrefab, GameArea);
            hint = Instantiate(hintPrefab, GameArea);

            // 생성된 UI들이 화면 중앙에 예쁘게 놓이도록 위치를 초기화해 줍니다.
            barBackground.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            barFill.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            hint.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, GameHalfH * 0.4f); // 힌트는 화면 위쪽에 배치
        }

        public override void StartGame()
        {
            base.StartGame();
            progress = 0f;

            // 게임 시작 시 게이지 바가 비어있는 상태로 보이도록 크기를 0으로 초기화
            if (barFill != null)
            {
                barFill.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 48f);
            }
        }

        private void Update()
        {
            if (!running || barFill == null) return;

            // 스페이스바나 마우스 좌클릭 시 게이지 증가
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                progress += 0.08f;

            // 시간이 지나면 게이지가 조금씩 줄어듦
            progress = Mathf.Max(0f, progress - Time.deltaTime * 0.04f);

            // progress 값(0~1)에 따라 게이지 바(Fill)의 가로 길이를 늘려줍니다.
            // GameArea 너비에 비례해서 최대 너비가 결정됩니다.
            float maxBarWidth = GameHalfW * 2f - 80f;
            barFill.GetComponent<RectTransform>().sizeDelta = new Vector2(maxBarWidth * Mathf.Clamp01(progress), 48f);

            // 게이지가 100% 다 차면 클리어!
            if (progress >= 1f) cleared = true;
        }

        public override void Cleanup()
        {
            if (barBackground != null) Destroy(barBackground);
            if (barFill != null) Destroy(barFill);
            if (hint != null) Destroy(hint);
        }
    }
}