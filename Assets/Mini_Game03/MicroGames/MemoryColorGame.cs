using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MiniGame
{
    public class MemoryColorGame : MicroGameBase
    {
        public override string Instruction => "기억해라!";

        [Header("Prefabs")]
        [Tooltip("처음에 잠시 보여줄 큰 색상 프리팹을 연결하세요.")]
        public GameObject previewPrefab;
        [Tooltip("나중에 나타날 4개의 선택지(버튼) 프리팹을 연결하세요.")]
        public GameObject choicePrefab;

        private readonly Color[] palette =
        {
            new Color(1f, 0.3f, 0.3f),   // 빨강
            new Color(0.25f, 1f, 0.35f), // 초록
            new Color(0.25f, 0.55f, 1f), // 파랑
            new Color(1f, 0.88f, 0.15f)  // 노랑
        };

        private Color targetColor;
        private GameObject preview;
        private readonly GameObject[] choices = new GameObject[4];

        public override void Setup()
        {
            targetColor = palette[Random.Range(0, palette.Length)];

            // 에러 방지용 체크
            if (previewPrefab == null)
            {
                Debug.LogError("MemoryColorGame: previewPrefab이 할당되지 않았습니다!");
                return;
            }

            // 하드코딩된 경로 대신 인스펙터에 등록된 프리팹을 생성합니다.
            preview = Instantiate(previewPrefab, GameArea);

            // 화면 중앙에 예쁘게 나오도록 위치 초기화
            preview.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, GameHalfH * 0.15f);
            preview.GetComponent<Image>().color = targetColor;
        }

        public override void StartGame()
        {
            base.StartGame();
            StartCoroutine(ShowChoices());
        }

        private IEnumerator ShowChoices()
        {
            yield return new WaitForSeconds(0.9f); // 0.9초 대기
            if (preview != null) Destroy(preview); // 프리뷰 삭제

            // 에러 방지용 체크
            if (choicePrefab == null)
            {
                Debug.LogError("MemoryColorGame: choicePrefab이 할당되지 않았습니다!");
                yield break;
            }

            // 선택지 순서 섞기
            int[] order = { 0, 1, 2, 3 };
            for (int i = 0; i < order.Length; i++)
            {
                int j = Random.Range(i, order.Length);
                int temp = order[i];
                order[i] = order[j];
                order[j] = temp;
            }

            // 4개의 버튼 생성
            float spacing = (GameHalfW * 2f - 80f) / 3f; // 버튼 간격을 GameArea 너비에 맞게 계산
            float startX  = -(spacing * 1.5f);            // 왼쪽 첫 버튼 X 위치
            float btnY    = -(GameHalfH - 120f);          // 버튼 Y 위치 (화면 아래쪽)

            for (int i = 0; i < 4; i++)
            {
                var color = palette[order[i]];
                var go = Instantiate(choicePrefab, GameArea);
                go.GetComponent<RectTransform>().anchoredPosition = new Vector2(startX + i * spacing, btnY);
                go.GetComponent<Image>().color = color;
                choices[i] = go;

                var button = go.GetComponent<Button>();
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => OnChoice(color));
            }
        }

        private void OnChoice(Color selected)
        {
            if (!running) return;

            // 선택한 색상이 정답 색상과 같으면 클리어
            if (selected == targetColor) cleared = true;
            else failed = true;
        }

        public override void Cleanup()
        {
            if (preview != null) Destroy(preview);
            for (int i = 0; i < choices.Length; i++)
            {
                if (choices[i] != null) Destroy(choices[i]);
            }
        }
    }
}