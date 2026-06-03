using UnityEngine;
using UnityEngine.UI;

namespace MiniGame
{
    public class TapTargetGame : MicroGameBase
    {
        public override string Instruction => "탭!";

        [Header("Prefabs")]
        [Tooltip("생성될 타겟(버튼) 프리팹을 여기에 연결하세요.")]
        public GameObject targetPrefab; // 유니티 에디터에서 직접 할당할 프리팹!

        private GameObject currentTarget;
        private int hitCount;
        private const int RequiredHits = 3;

        public override void StartGame()
        {
            base.StartGame();
            hitCount = 0; // 시작할 때 타격 횟수 초기화
            SpawnTarget();
        }

        private void SpawnTarget()
        {
            // 기존에 화면에 있던 타겟이 있다면 지워줍니다.
            if (currentTarget != null) Destroy(currentTarget);

            // 안전장치: 프리팹이 안 들어있으면 에러 띄우기
            if (targetPrefab == null)
            {
                Debug.LogError("TapTargetGame: targetPrefab이 할당되지 않았습니다! 인스펙터 창을 확인해주세요.");
                return;
            }

            // 스크립트에 연결된 프리팹을 GameArea 안에 생성(Instantiate)합니다.
            currentTarget = Instantiate(targetPrefab, GameArea);

            // 위치를 GameArea 크기에 맞게 랜덤으로 지정해줍니다.
            currentTarget.GetComponent<RectTransform>().anchoredPosition = new Vector2(
                Random.Range(-(GameHalfW - 80f), GameHalfW - 80f),
                Random.Range(-(GameHalfH - 80f), GameHalfH - 80f));

            // 버튼 컴포넌트를 찾아서 클릭 이벤트를 연결해줍니다.
            var button = currentTarget.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnTapTarget);
        }

        private void OnTapTarget()
        {
            if (!running) return;

            hitCount++;
            if (hitCount >= RequiredHits)
            {
                cleared = true;
                if (currentTarget != null) Destroy(currentTarget); // 클리어 시 마지막 타겟 지우기
                return;
            }

            // 3번 채우지 못했다면 새로운 타겟 생성
            SpawnTarget();
        }

        public override void Cleanup()
        {
            // 게임이 끝날 때 남은 타겟 지우기
            if (currentTarget != null) Destroy(currentTarget);
        }
    }
}