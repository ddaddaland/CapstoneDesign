using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace MiniGame
{
    /// <summary>
    /// 미니게임 씬 단독 실행용. GameManager 없이 Canvas·UI·게임 루프를 직접 구동합니다.
    /// Main 씬에서 Additive 로드될 때는 GameManager가 있으므로 비활성화됩니다.
    /// </summary>
    [DefaultExecutionOrder(-100)]
    public class MicroGameSceneHost : MonoBehaviour
    {
        [SerializeField] private float roundDuration = 5f;
        [SerializeField] private float introDuration = 0.28f;
        [SerializeField] private float resultDuration = 0.9f;

        private MicroGameBase microGame;
        private RectTransform gameAreaRect;
        private TextMeshProUGUI instructionText;
        private TextMeshProUGUI timerText;
        private TextMeshProUGUI resultText;
        private Image flashOverlay;

        private RectTransform instructionRect;
        private RectTransform resultRect;
        private Vector3 gameAreaBaseScale;
        private Vector3 instructionBaseScale;
        private Vector3 resultBaseScale;
        private Color timerBaseColor;
        private Color resultBaseColor;
        private Color instructionBaseColor;
        private bool lastRoundSuccess;

        private void Awake()
        {
            if (FindFirstObjectByType<GameManager>() != null)
            {
                enabled = false;
                return;
            }

            microGame = GetComponent<MicroGameBase>();
            if (microGame == null)
                microGame = GetComponentInChildren<MicroGameBase>(true);

            if (microGame == null)
            {
                Debug.LogError("MicroGameSceneHost: MicroGameBase 컴포넌트를 찾을 수 없습니다.");
                enabled = false;
                return;
            }

            EnsureEventSystem();

            var canvasPrefab = Resources.Load<GameObject>("Prefabs/UI/Canvas");
            if (canvasPrefab == null)
            {
                Debug.LogError("MicroGameSceneHost: Resources/Prefabs/UI/Canvas 를 찾을 수 없습니다.");
                enabled = false;
                return;
            }

            var canvasInstance = Instantiate(canvasPrefab);
            canvasInstance.transform.localScale = Vector3.one;
            BindUi(canvasInstance.transform);

            if (gameAreaRect == null)
            {
                Debug.LogError("MicroGameSceneHost: GameArea 를 찾을 수 없습니다.");
                enabled = false;
                return;
            }

            microGame.SetGameArea(gameAreaRect);
            CacheUiReferences();
        }

        private void Start()
        {
            if (!enabled || microGame == null)
                return;

            StartCoroutine(GameLoop());
        }

        private void BindUi(Transform canvasRoot)
        {
            instructionText = FindChildComponent<TextMeshProUGUI>(canvasRoot, "InstructionText");
            timerText       = FindChildComponent<TextMeshProUGUI>(canvasRoot, "TimerText");
            resultText      = FindChildComponent<TextMeshProUGUI>(canvasRoot, "ResultText");

            var scoreText = FindChildComponent<TextMeshProUGUI>(canvasRoot, "ScoreText");
            var livesText = FindChildComponent<TextMeshProUGUI>(canvasRoot, "LivesText");
            if (scoreText != null) scoreText.gameObject.SetActive(false);
            if (livesText != null) livesText.gameObject.SetActive(false);

            var gameAreaTransform = FindChildTransform(canvasRoot, "GameArea");
            if (gameAreaTransform != null)
                gameAreaRect = gameAreaTransform.GetComponent<RectTransform>();

            var flashPrefab = Resources.Load<GameObject>("Prefabs/UI/FlashOverlay");
            if (flashPrefab != null)
            {
                var flashInstance = Instantiate(flashPrefab, canvasRoot);
                flashOverlay = flashInstance.GetComponent<Image>();
            }
        }

        private void CacheUiReferences()
        {
            instructionRect = instructionText != null ? instructionText.rectTransform : null;
            resultRect      = resultText      != null ? resultText.rectTransform      : null;
            gameAreaBaseScale = gameAreaRect.localScale;

            if (instructionRect != null) instructionBaseScale = instructionRect.localScale;
            if (resultRect      != null) resultBaseScale      = resultRect.localScale;
            if (timerText       != null) timerBaseColor       = timerText.color;
            if (resultText      != null) resultBaseColor      = resultText.color;
            if (instructionText != null) instructionBaseColor = instructionText.color;
        }

        private IEnumerator GameLoop()
        {
            yield return new WaitForSeconds(0.4f);

            while (true)
            {
                yield return StartCoroutine(PrepareRound());
                yield return StartCoroutine(PlayRound());
                yield return StartCoroutine(ShowRoundResult(lastRoundSuccess));
            }
        }

        private IEnumerator PrepareRound()
        {
            microGame.Cleanup();
            microGame.Setup();

            if (instructionText != null)
            {
                instructionText.gameObject.SetActive(true);
                instructionText.text  = microGame.Instruction;
                instructionText.color = instructionBaseColor;
            }

            if (resultText != null) resultText.text = string.Empty;
            if (timerText  != null) timerText.text  = string.Empty;

            yield return StartCoroutine(PlayInstructionZoom());

            if (instructionText != null)
                instructionText.gameObject.SetActive(false);
        }

        private IEnumerator PlayRound()
        {
            float timer = roundDuration;
            lastRoundSuccess = false;
            microGame.StartGame();

            while (timer > 0f)
            {
                timer -= Time.deltaTime;
                float clampedTimer = Mathf.Max(0f, timer);

                if (timerText != null)
                {
                    float warningLerp = 1f - Mathf.Clamp01(clampedTimer / 1.5f);
                    timerText.text  = Mathf.CeilToInt(clampedTimer).ToString();
                    timerText.color = Color.Lerp(timerBaseColor, new Color(1f, 0.3f, 0.25f), warningLerp);
                    timerText.rectTransform.localScale = Vector3.one * (1f + warningLerp * 0.28f);
                }

                if (microGame.IsCleared()) { lastRoundSuccess = true; break; }
                if (microGame.IsFailed())  { lastRoundSuccess = false; break; }

                yield return null;
            }

            microGame.EndGame();

            if (timerText != null)
            {
                timerText.text  = string.Empty;
                timerText.color = timerBaseColor;
                timerText.rectTransform.localScale = Vector3.one;
            }
        }

        private IEnumerator ShowRoundResult(bool success)
        {
            if (resultText != null)
            {
                resultText.gameObject.SetActive(true);
                resultText.text  = success ? "성공!" : "실패!";
                resultText.color = success ? new Color(0.18f, 1f, 0.38f) : new Color(1f, 0.26f, 0.26f);
            }

            yield return StartCoroutine(AnimateRoundResult(success));

            if (resultText != null)
            {
                resultText.text  = string.Empty;
                resultText.color = resultBaseColor;
            }

            microGame.Cleanup();

            if (instructionText != null)
            {
                instructionText.gameObject.SetActive(true);
                instructionText.color = Color.white;
                instructionText.text  = success ? "성공! 클릭해서 다시 하기" : "실패! 클릭해서 다시 하기";
            }

            yield return new WaitForSeconds(0.35f);
            while (!Input.GetMouseButtonDown(0))
                yield return null;

            if (instructionText != null)
                instructionText.gameObject.SetActive(false);
        }

        private IEnumerator PlayInstructionZoom()
        {
            float t = 0f;
            Vector3 startGameScale = gameAreaBaseScale * 1.24f;
            Vector3 startTextScale = instructionBaseScale * 1.9f;
            Color startColor = new Color(1f, 0.95f, 0.35f);

            while (t < introDuration)
            {
                t += Time.deltaTime;
                float progress = Mathf.Clamp01(t / introDuration);
                float eased    = EaseOutBack(progress);

                gameAreaRect.localScale = Vector3.Lerp(startGameScale, gameAreaBaseScale, eased);
                if (instructionRect != null)
                    instructionRect.localScale = Vector3.Lerp(startTextScale, instructionBaseScale, eased);
                if (instructionText != null)
                    instructionText.color = Color.Lerp(startColor, instructionBaseColor, progress);

                SetFlashColor(new Color(1f, 0.95f, 0.65f, Mathf.Lerp(0.24f, 0f, progress)));
                yield return null;
            }

            SetFlashColor(Color.clear);
        }

        private IEnumerator AnimateRoundResult(bool success)
        {
            if (resultText == null) yield break;

            float t = 0f;
            while (t < resultDuration)
            {
                t += Time.deltaTime;
                float progress = Mathf.Clamp01(t / resultDuration);
                float punch    = Mathf.Sin(progress * Mathf.PI);

                if (resultRect != null)
                {
                    float appear = EaseOutBack(Mathf.Clamp01(progress / 0.3f));
                    resultRect.localScale = resultBaseScale * Mathf.Lerp(0.2f, 1f + (success ? 0.45f : 0.25f) * punch, appear);
                }

                SetFlashColor(new Color(success ? 0.3f : 1f, success ? 1f : 0.2f, success ? 0.45f : 0.2f, (success ? 0.2f : 0.28f) * punch * (1f - progress)));
                yield return null;
            }

            SetFlashColor(Color.clear);
        }

        private void SetFlashColor(Color color)
        {
            if (flashOverlay != null)
                flashOverlay.color = color;
        }

        private static void EnsureEventSystem()
        {
            if (FindFirstObjectByType<EventSystem>() != null)
                return;

            var eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<StandaloneInputModule>();
        }

        internal static Transform FindChildTransform(Transform root, string childName)
        {
            foreach (var transform in root.GetComponentsInChildren<Transform>(true))
            {
                if (transform.name == childName)
                    return transform;
            }

            return null;
        }

        internal static T FindChildComponent<T>(Transform root, string childName) where T : Component
        {
            var child = FindChildTransform(root, childName);
            return child != null ? child.GetComponent<T>() : null;
        }

        private static float EaseOutBack(float x)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1f;
            float p = x - 1f;
            return 1f + c3 * p * p * p + c1 * p * p;
        }
    }
}
