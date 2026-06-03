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

        private readonly List<string> microGamePrefabPaths = new List<string>();
        private MicroGameBase currentGame;
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

        private void Awake()
        {
            Instance = this;
            Time.timeScale = 1f;
        }

        private void Start()
        {
            lives = startingLives;
            CacheUiReferences();

            microGamePrefabPaths.Add("Prefabs/MicroGames/TapTargetGame");
            microGamePrefabPaths.Add("Prefabs/MicroGames/DodgeGame");
            microGamePrefabPaths.Add("Prefabs/MicroGames/CatchFruitGame");
            microGamePrefabPaths.Add("Prefabs/MicroGames/MemoryColorGame");
            microGamePrefabPaths.Add("Prefabs/MicroGames/MashButtonGame");
            microGamePrefabPaths.Add("Prefabs/MicroGames/DragToBoxGame");

            ResetPresentationState();
            UpdateHud();
            StartCoroutine(GameLoop());
        }

        private void OnDestroy()
        {
            Time.timeScale = 1f;
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void CacheUiReferences()
        {
            gameAreaRect = gameArea != null ? gameArea.GetComponent<RectTransform>() : null;
            instructionRect = instructionText != null ? instructionText.rectTransform : null;
            resultRect = resultText != null ? resultText.rectTransform : null;

            if (gameAreaRect != null)
            {
                gameAreaBasePos = gameAreaRect.anchoredPosition;
                gameAreaBaseScale = gameAreaRect.localScale;
                gameAreaBaseRotation = gameAreaRect.localRotation;
            }

            if (instructionRect != null)
            {
                instructionBaseScale = instructionRect.localScale;
            }

            if (resultRect != null)
            {
                resultBaseScale = resultRect.localScale;
            }

            if (timerText != null)
            {
                timerBaseColor = timerText.color;
            }

            if (resultText != null)
            {
                resultBaseColor = resultText.color;
            }

            if (instructionText != null)
            {
                instructionBaseColor = instructionText.color;
            }
        }

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
            currentGame = SpawnMicroGame(microGamePrefabPaths[Random.Range(0, microGamePrefabPaths.Count)]);
            currentGame.Setup();

            instructionText.gameObject.SetActive(true);
            instructionText.text = currentGame.Instruction;
            instructionText.color = instructionBaseColor;
            resultText.text = string.Empty;
            timerText.text = string.Empty;

            yield return StartCoroutine(PlayInstructionZoom());

            instructionText.gameObject.SetActive(false);
            ResetPresentationState();
        }

        private IEnumerator PlayRound()
        {
            timer = microGameDuration;
            currentSuccess = false;
            currentGame.StartGame();

            while (timer > 0f)
            {
                timer -= Time.deltaTime;
                float clampedTimer = Mathf.Max(0f, timer);
                timerText.text = Mathf.CeilToInt(clampedTimer).ToString();
                float warningLerp = 1f - Mathf.Clamp01(clampedTimer / 1.5f);
                timerText.color = Color.Lerp(timerBaseColor, new Color(1f, 0.3f, 0.25f), warningLerp);
                timerText.rectTransform.localScale = Vector3.one * (1f + warningLerp * 0.28f + Mathf.Sin(Time.time * 24f) * warningLerp * 0.06f);

                if (currentGame.IsCleared())
                {
                    currentSuccess = true;
                    break;
                }

                if (currentGame.IsFailed())
                {
                    currentSuccess = false;
                    break;
                }

                yield return null;
            }

            currentGame.EndGame();
            timerText.text = string.Empty;
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
                resultText.text = $"성공!";
                resultText.color = new Color(0.18f, 1f, 0.38f);
            }
            else
            {
                lives--;
                resultText.text = "실패!";
                resultText.color = new Color(1f, 0.26f, 0.26f);
            }

            UpdateHud();
            yield return StartCoroutine(AnimateRoundResult(currentSuccess));

            resultText.text = string.Empty;
            resultText.color = resultBaseColor;
            ResetPresentationState();

            currentGame.Cleanup();
            Destroy(currentGame.gameObject);
            currentGame = null;
        }

        private IEnumerator ShowGameOver()
        {
            Time.timeScale = 1f;
            currentTimeScale = 1f;
            instructionText.gameObject.SetActive(true);
            instructionText.color = Color.white;
            float highestSpeed = Mathf.Min(maxTimeScale, 1f + roundsCleared * speedIncreasePerClear);
            instructionText.text = $"Game Over!\n점수: {score}\n클릭해서 다시 시작";
            resultText.text = string.Empty;
            timerText.text = string.Empty;
            yield return new WaitForSeconds(0.35f);
            while (!Input.GetMouseButtonDown(0))
            {
                yield return null;
            }
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private IEnumerator PlayInstructionZoom()
        {
            ResetPresentationState();
            float startAngle = Random.Range(-7f, 7f);
            Vector3 startGameScale = gameAreaBaseScale * introZoomScale;
            Vector3 startTextScale = instructionBaseScale * 1.9f;
            Color startInstructionColor = new Color(1f, 0.95f, 0.35f);
            float t = 0f;

            while (t < introDuration)
            {
                t += Time.deltaTime;
                float progress = Mathf.Clamp01(t / introDuration);
                float eased = EaseOutBack(progress);
                if (gameAreaRect != null)
                {
                    gameAreaRect.localScale = Vector3.Lerp(startGameScale, gameAreaBaseScale, eased);
                    gameAreaRect.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(startAngle, 0f, eased));
                }

                if (instructionRect != null)
                {
                    instructionRect.localScale = Vector3.Lerp(startTextScale, instructionBaseScale, eased);
                }

                instructionText.color = Color.Lerp(startInstructionColor, instructionBaseColor, progress);
                SetFlashColor(new Color(1f, 0.95f, 0.65f, Mathf.Lerp(0.24f, 0f, progress)));
                yield return null;
            }

            float settle = 0f;
            while (settle < 0.16f)
            {
                settle += Time.deltaTime;
                float wobble = Mathf.Sin(settle * 34f) * 0.02f;
                if (instructionRect != null)
                {
                    instructionRect.localScale = instructionBaseScale * (1f + wobble);
                }
                yield return null;
            }

            SetFlashColor(Color.clear);
        }

        private IEnumerator AnimateRoundResult(bool success)
        {
            if (resultText == null)
            {
                yield break;
            }

            ResetPresentationState();
            resultText.gameObject.SetActive(true);
            float t = 0f;
            Color flashColor = success ? new Color(0.3f, 1f, 0.45f, 0f) : new Color(1f, 0.2f, 0.2f, 0f);

            while (t < resultDuration)
            {
                t += Time.deltaTime;
                float progress = Mathf.Clamp01(t / resultDuration);
                float fade = 1f - progress;
                float punch = Mathf.Sin(progress * Mathf.PI);
                float shakeStrength = success ? 10f : 22f;
                float shake = Mathf.Sin(progress * 42f) * shakeStrength * fade;

                if (gameAreaRect != null)
                {
                    gameAreaRect.anchoredPosition = gameAreaBasePos + new Vector2(shake, success ? 0f : Mathf.Cos(progress * 34f) * 6f * fade);
                    gameAreaRect.localScale = gameAreaBaseScale * (1f + (success ? 0.12f : -0.05f) * punch);
                    gameAreaRect.localRotation = Quaternion.Euler(0f, 0f, success ? Mathf.Sin(progress * 18f) * 2.5f * fade : Mathf.Sin(progress * 30f) * 8f * fade);
                }

                if (resultRect != null)
                {
                    float appear = EaseOutBack(Mathf.Clamp01(progress / 0.3f));
                    float overshoot = success ? 0.45f : 0.25f;
                    resultRect.localScale = resultBaseScale * Mathf.Lerp(0.2f, 1f + overshoot * punch, appear);
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
            Time.timeScale = currentTimeScale;
        }

        private void ResetPresentationState()
        {
            if (gameAreaRect != null)
            {
                gameAreaRect.anchoredPosition = gameAreaBasePos;
                gameAreaRect.localScale = gameAreaBaseScale;
                gameAreaRect.localRotation = gameAreaBaseRotation;
            }

            if (instructionRect != null)
            {
                instructionRect.localScale = instructionBaseScale;
                instructionRect.localRotation = Quaternion.identity;
            }

            if (resultRect != null)
            {
                resultRect.localScale = resultBaseScale;
                resultRect.localRotation = Quaternion.identity;
            }

            if (instructionText != null)
            {
                instructionText.color = instructionBaseColor;
            }

            if (timerText != null)
            {
                timerText.color = timerBaseColor;
                timerText.rectTransform.localScale = Vector3.one;
            }

            SetFlashColor(Color.clear);
        }

        private void SetFlashColor(Color color)
        {
            if (flashOverlay == null) return;
            flashOverlay.color = color;
        }

        private MicroGameBase SpawnMicroGame(string resourcesPath)
        {
            var prefab = Resources.Load<GameObject>(resourcesPath);
            if (prefab == null)
            {
                Debug.LogError($"Microgame prefab not found: Resources/{resourcesPath}");
                var fallback = new GameObject(resourcesPath);
                fallback.transform.SetParent(gameArea.transform, false);
                fallback.AddComponent<RectTransform>();
                return fallback.AddComponent<TapTargetGame>();
            }

            var instance = Instantiate(prefab, gameArea.transform, false);

            // --- [여기가 핵심 수정 부분입니다!] ---
            // 미니게임 프리팹이 생성될 때 위치가 엇나가지 않도록 GameArea에 꽉 차게 묶어줍니다.
            RectTransform rt = instance.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchorMin = Vector2.zero;     // 앵커 왼쪽 아래
                rt.anchorMax = Vector2.one;      // 앵커 오른쪽 위
                rt.offsetMin = Vector2.zero;     // 여백 없앰
                rt.offsetMax = Vector2.zero;     // 여백 없앰
                rt.localScale = Vector3.one;     // 크기 정상화
                rt.anchoredPosition = Vector2.zero; // 정중앙 배치
            }
            // ------------------------------------

            return instance.GetComponent<MicroGameBase>();
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