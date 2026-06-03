using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace MiniGame
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        public TextMeshProUGUI instructionText;
        public TextMeshProUGUI timerText;
        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI livesText;
        public TextMeshProUGUI resultText;
        public GameObject gameArea;
        public Image flashOverlay;

        public float microGameDuration = 5f;
        public int startingLives = 3;

        [Header("Game Feel")]
        public float speedIncreasePerClear = 0.05f;
        public float maxTimeScale = 3.0f;
        public float introZoomScale = 1.24f;
        public float introDuration = 0.28f;
        public float resultDuration = 0.9f;

        // ── [수정] 프리팹 경로 대신 씬 이름 목록으로 변경 ──
        private readonly List<string> microGameSceneNames = new List<string>
        {
            "TapTargetGame",
            "DodgeGame",
            "CatchFruitGame",
            "MemoryColorGame",
            "MashButtonGame",
            "DragToBoxGame"
        };

        private MicroGameBase currentGame;
        private string loadedSceneName;   // 현재 로드된 미니게임 씬 이름
        private bool standaloneMode;
        private MicroGameBase standaloneMicroGame;
        private int score;
        private int lives;
        private int roundsCleared;
        private float timer;
        private float currentTimeScale = 1f;
        private bool currentSuccess;

        private RectTransform gameAreaRect;
        private RectTransform instructionRect;
        private RectTransform resultRect;
        private Vector2 gameAreaBasePos;
        private Vector3 gameAreaBaseScale;
        private Quaternion gameAreaBaseRotation;
        private Vector3 instructionBaseScale;
        private Vector3 resultBaseScale;
        private Color timerBaseColor;
        private Color resultBaseColor;
        private Color instructionBaseColor;

        public void BindUiFromCanvas(Transform canvasRoot)
        {
            instructionText = MicroGameSceneHost.FindChildComponent<TextMeshProUGUI>(canvasRoot, "InstructionText");
            timerText       = MicroGameSceneHost.FindChildComponent<TextMeshProUGUI>(canvasRoot, "TimerText");
            scoreText       = MicroGameSceneHost.FindChildComponent<TextMeshProUGUI>(canvasRoot, "ScoreText");
            livesText       = MicroGameSceneHost.FindChildComponent<TextMeshProUGUI>(canvasRoot, "LivesText");
            resultText      = MicroGameSceneHost.FindChildComponent<TextMeshProUGUI>(canvasRoot, "ResultText");

            var gameAreaTransform = MicroGameSceneHost.FindChildTransform(canvasRoot, "GameArea");
            if (gameAreaTransform != null)
                gameArea = gameAreaTransform.gameObject;

            var flashPrefab = Resources.Load<GameObject>("Prefabs/UI/FlashOverlay");
            if (flashPrefab != null && canvasRoot != null)
            {
                var flashInstance = Instantiate(flashPrefab, canvasRoot);
                flashOverlay = flashInstance.GetComponent<Image>();
            }
        }

        public void EnableStandaloneMode(MicroGameBase microGame)
        {
            standaloneMode       = true;
            standaloneMicroGame  = microGame;
        }

        private void Awake()
        {
            Instance = this;
            Time.timeScale = 1f;
        }

        private void Start()
        {
            if (gameArea == null)
            {
                Debug.LogError("GameManager: gameArea가 연결되지 않았습니다. Main 씬 UI 또는 미니게임 씬 부트스트랩을 확인하세요.");
                return;
            }

            lives = startingLives;
            CacheUiReferences();
            ResetPresentationState();
            UpdateHud();
            StartCoroutine(GameLoop());
        }

        private void OnDestroy()
        {
            Time.timeScale = 1f;
            if (Instance == this) Instance = null;
        }

        private void CacheUiReferences()
        {
            gameAreaRect    = gameArea        != null ? gameArea.GetComponent<RectTransform>() : null;
            instructionRect = instructionText != null ? instructionText.rectTransform          : null;
            resultRect      = resultText      != null ? resultText.rectTransform               : null;

            if (gameAreaRect    != null) { gameAreaBasePos = gameAreaRect.anchoredPosition; gameAreaBaseScale = gameAreaRect.localScale; gameAreaBaseRotation = gameAreaRect.localRotation; }
            if (instructionRect != null) { instructionBaseScale = instructionRect.localScale; }
            if (resultRect      != null) { resultBaseScale = resultRect.localScale; }
            if (timerText       != null) { timerBaseColor = timerText.color; }
            if (resultText      != null) { resultBaseColor = resultText.color; }
            if (instructionText != null) { instructionBaseColor = instructionText.color; }
        }

        // ────────────────────────────────────────────────────
        // 씬 로드 / 언로드
        // ────────────────────────────────────────────────────

        /// <summary>미니게임 씬을 Additive로 로드하고 MicroGameBase를 찾아 currentGame에 할당합니다.</summary>
        private IEnumerator LoadMicroGameScene(string sceneName)
        {
            var op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            yield return op;

            loadedSceneName = sceneName;

            // 로드된 씬의 루트 오브젝트에서 MicroGameBase 컴포넌트를 찾습니다.
            var scene = SceneManager.GetSceneByName(sceneName);
            foreach (var root in scene.GetRootGameObjects())
            {
                currentGame = root.GetComponentInChildren<MicroGameBase>(true);
                if (currentGame != null) break;
            }

            if (currentGame == null)
                Debug.LogError($"GameManager: '{sceneName}' 씬에서 MicroGameBase 컴포넌트를 찾을 수 없습니다!");
        }

        /// <summary>현재 로드된 미니게임 씬을 언로드합니다.</summary>
        private IEnumerator UnloadMicroGameScene()
        {
            if (string.IsNullOrEmpty(loadedSceneName)) yield break;

            yield return SceneManager.UnloadSceneAsync(loadedSceneName);
            loadedSceneName = null;
            currentGame     = null;
        }

        // ────────────────────────────────────────────────────
        // 게임 루프
        // ────────────────────────────────────────────────────

        private IEnumerator GameLoop()
        {
            yield return new WaitForSeconds(0.4f);
            while (lives > 0)
            {
                yield return StartCoroutine(PrepareRound());
                yield return StartCoroutine(PlayRound());
                yield return StartCoroutine(ShowRoundResult());
            }
            yield return StartCoroutine(ShowGameOver());
        }

        private IEnumerator PrepareRound()
        {
            ResetPresentationState();

            if (standaloneMode)
            {
                currentGame = standaloneMicroGame;
            }
            else
            {
                string sceneName = microGameSceneNames[Random.Range(0, microGameSceneNames.Count)];
                yield return StartCoroutine(LoadMicroGameScene(sceneName));
            }

            if (currentGame == null) yield break;

            currentGame.Setup();

            instructionText.gameObject.SetActive(true);
            instructionText.text  = currentGame.Instruction;
            instructionText.color = instructionBaseColor;
            resultText.text       = string.Empty;
            timerText.text        = string.Empty;

            yield return StartCoroutine(PlayInstructionZoom());

            instructionText.gameObject.SetActive(false);
            ResetPresentationState();
        }

        private IEnumerator PlayRound()
        {
            timer          = microGameDuration;
            currentSuccess = false;
            currentGame.StartGame();

            while (timer > 0f)
            {
                timer -= Time.deltaTime;
                float clampedTimer  = Mathf.Max(0f, timer);
                float warningLerp   = 1f - Mathf.Clamp01(clampedTimer / 1.5f);

                timerText.text  = Mathf.CeilToInt(clampedTimer).ToString();
                timerText.color = Color.Lerp(timerBaseColor, new Color(1f, 0.3f, 0.25f), warningLerp);
                timerText.rectTransform.localScale = Vector3.one * (1f + warningLerp * 0.28f + Mathf.Sin(Time.time * 24f) * warningLerp * 0.06f);

                if (currentGame.IsCleared()) { currentSuccess = true;  break; }
                if (currentGame.IsFailed())  { currentSuccess = false; break; }

                yield return null;
            }

            currentGame.EndGame();
            timerText.text  = string.Empty;
            timerText.color = timerBaseColor;
            timerText.rectTransform.localScale = Vector3.one;
        }

        private IEnumerator ShowRoundResult()
        {
            if (currentSuccess)
            {
                score++;
                roundsCleared++;
                IncreaseSpeed();
                resultText.text  = "성공!";
                resultText.color = new Color(0.18f, 1f, 0.38f);
            }
            else
            {
                lives--;
                resultText.text  = "실패!";
                resultText.color = new Color(1f, 0.26f, 0.26f);
            }

            UpdateHud();
            yield return StartCoroutine(AnimateRoundResult(currentSuccess));

            resultText.text  = string.Empty;
            resultText.color = resultBaseColor;
            ResetPresentationState();

            currentGame.Cleanup();
            if (!standaloneMode)
                yield return StartCoroutine(UnloadMicroGameScene());
        }

        private IEnumerator ShowGameOver()
        {
            Time.timeScale    = 1f;
            currentTimeScale  = 1f;
            instructionText.gameObject.SetActive(true);
            instructionText.color = Color.white;
            instructionText.text  = $"Game Over!\n점수: {score}\n클릭해서 다시 시작";
            resultText.text       = string.Empty;
            timerText.text        = string.Empty;

            yield return new WaitForSeconds(0.35f);
            while (!Input.GetMouseButtonDown(0)) yield return null;

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        // ────────────────────────────────────────────────────
        // 연출
        // ────────────────────────────────────────────────────

        private IEnumerator PlayInstructionZoom()
        {
            ResetPresentationState();
            float   startAngle      = Random.Range(-7f, 7f);
            Vector3 startGameScale  = gameAreaBaseScale * introZoomScale;
            Vector3 startTextScale  = instructionBaseScale * 1.9f;
            Color   startColor      = new Color(1f, 0.95f, 0.35f);
            float   t = 0f;

            while (t < introDuration)
            {
                t += Time.deltaTime;
                float progress = Mathf.Clamp01(t / introDuration);
                float eased    = EaseOutBack(progress);

                if (gameAreaRect    != null) { gameAreaRect.localScale    = Vector3.Lerp(startGameScale, gameAreaBaseScale, eased); gameAreaRect.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(startAngle, 0f, eased)); }
                if (instructionRect != null) { instructionRect.localScale = Vector3.Lerp(startTextScale, instructionBaseScale, eased); }

                instructionText.color = Color.Lerp(startColor, instructionBaseColor, progress);
                SetFlashColor(new Color(1f, 0.95f, 0.65f, Mathf.Lerp(0.24f, 0f, progress)));
                yield return null;
            }

            float settle = 0f;
            while (settle < 0.16f)
            {
                settle += Time.deltaTime;
                if (instructionRect != null) instructionRect.localScale = instructionBaseScale * (1f + Mathf.Sin(settle * 34f) * 0.02f);
                yield return null;
            }

            SetFlashColor(Color.clear);
        }

        private IEnumerator AnimateRoundResult(bool success)
        {
            if (resultText == null) yield break;

            ResetPresentationState();
            resultText.gameObject.SetActive(true);
            float t = 0f;
            Color flashColor = success ? new Color(0.3f, 1f, 0.45f, 0f) : new Color(1f, 0.2f, 0.2f, 0f);

            while (t < resultDuration)
            {
                t += Time.deltaTime;
                float progress      = Mathf.Clamp01(t / resultDuration);
                float fade          = 1f - progress;
                float punch         = Mathf.Sin(progress * Mathf.PI);
                float shakeStrength = success ? 10f : 22f;
                float shake         = Mathf.Sin(progress * 42f) * shakeStrength * fade;

                if (gameAreaRect != null)
                {
                    gameAreaRect.anchoredPosition = gameAreaBasePos + new Vector2(shake, success ? 0f : Mathf.Cos(progress * 34f) * 6f * fade);
                    gameAreaRect.localScale       = gameAreaBaseScale * (1f + (success ? 0.12f : -0.05f) * punch);
                    gameAreaRect.localRotation    = Quaternion.Euler(0f, 0f, success ? Mathf.Sin(progress * 18f) * 2.5f * fade : Mathf.Sin(progress * 30f) * 8f * fade);
                }

                if (resultRect != null)
                {
                    float appear    = EaseOutBack(Mathf.Clamp01(progress / 0.3f));
                    float overshoot = success ? 0.45f : 0.25f;
                    resultRect.localScale    = resultBaseScale * Mathf.Lerp(0.2f, 1f + overshoot * punch, appear);
                    resultRect.localRotation = Quaternion.Euler(0f, 0f, success ? Mathf.Sin(progress * 16f) * 4f * fade : Mathf.Sin(progress * 32f) * 9f * fade);
                }

                SetFlashColor(new Color(flashColor.r, flashColor.g, flashColor.b, (success ? 0.2f : 0.28f) * punch * fade));
                yield return null;
            }

            SetFlashColor(Color.clear);
            ResetPresentationState();
        }

        private void IncreaseSpeed()
        {
            currentTimeScale = Mathf.Min(maxTimeScale, currentTimeScale + speedIncreasePerClear);
            Time.timeScale   = currentTimeScale;
        }

        private void ResetPresentationState()
        {
            if (gameAreaRect    != null) { gameAreaRect.anchoredPosition = gameAreaBasePos; gameAreaRect.localScale = gameAreaBaseScale; gameAreaRect.localRotation = gameAreaBaseRotation; }
            if (instructionRect != null) { instructionRect.localScale = instructionBaseScale; instructionRect.localRotation = Quaternion.identity; }
            if (resultRect      != null) { resultRect.localScale = resultBaseScale;           resultRect.localRotation      = Quaternion.identity; }
            if (instructionText != null) { instructionText.color = instructionBaseColor; }
            if (timerText       != null) { timerText.color = timerBaseColor; timerText.rectTransform.localScale = Vector3.one; }
            SetFlashColor(Color.clear);
        }

        private void SetFlashColor(Color color)
        {
            if (flashOverlay != null) flashOverlay.color = color;
        }

        private void UpdateHud()
        {
            scoreText.text = $"점수: {score}";
            livesText.text = $"목숨: {lives}";
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
