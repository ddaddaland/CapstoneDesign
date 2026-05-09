using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public Slider progressSlider; // 방금 만든 슬라이더 UI를 연결할 칸

    public Transform car;         // 내 자동차
    public float startY = 0f;     // 게임 시작 시 자동차의 Y 위치 (보통 0)
    public float endY = 1000f;    // 목적지의 Y 위치 (예: 1000만큼 가면 클리어)

    void Start()
    {
        // 만약 시작 위치가 0이 아니라면, 게임 시작 시 차의 현재 위치를 출발점으로 잡습니다.
        startY = car.position.y;
    }

    void Update()
    {
        // 1. 차가 뒤로 가거나(그럴 일은 없겠지만), 목적지를 넘어가 버리는 것을 방지하기 위해 위치 제한
        float currentY = Mathf.Clamp(car.position.y, startY, endY);

        // 2. 수학 계산
        float totalDistance = endY - startY;      // 총 가야 할 거리
        float movedDistance = currentY - startY;  // 현재까지 온 거리

        // 3. 0.0 ~ 1.0 사이의 비율 구하기 (예: 절반 왔으면 0.5)
        float progressValue = movedDistance / totalDistance;

        // 4. 슬라이더에 값 적용!
        progressSlider.value = progressValue;
    }
}
