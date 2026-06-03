using UnityEngine;
using UnityEngine.UI;

namespace MiniGame
{
    public abstract class MicroGameBase : MonoBehaviour
    {
        public abstract string Instruction { get; }

        protected bool cleared;
        protected bool failed;
        protected bool running;
        private RectTransform gameAreaOverride;
        private static Sprite cachedSprite;
        private static Font cachedFont;

        public void SetGameArea(RectTransform area) => gameAreaOverride = area;

        public virtual void Setup() { }
        public virtual void StartGame()
        {
            cleared = false;
            failed = false;
            running = true;
        }
        public virtual void EndGame() { running = false; }
        public virtual void Cleanup() { }

        public bool IsCleared() => cleared;
        public bool IsFailed() => failed;

        protected RectTransform GameArea
        {
            get
            {
                if (gameAreaOverride != null)
                    return gameAreaOverride;

                if (GameManager.Instance != null && GameManager.Instance.gameArea != null)
                    return GameManager.Instance.gameArea.GetComponent<RectTransform>();

                return null;
            }
        }

        protected float GameHalfW
        {
            get
            {
                var area = GameArea;
                return area != null ? area.rect.width * 0.5f : 400f;
            }
        }

        protected float GameHalfH
        {
            get
            {
                var area = GameArea;
                return area != null ? area.rect.height * 0.5f : 225f;
            }
        }

        protected GameObject SpawnPrefab(string resourcesPath, Transform parent)
        {
            var prefab = Resources.Load<GameObject>(resourcesPath);
            if (prefab == null)
            {
                Debug.LogError($"Gameplay prefab not found: Resources/{resourcesPath}");
                var fallback = new GameObject(resourcesPath);
                fallback.transform.SetParent(parent, false);
                fallback.AddComponent<RectTransform>();
                return fallback;
            }

            var instance = Instantiate(prefab, parent, false);
            return instance;
        }

        protected Sprite WhiteSprite
        {
            get
            {
                if (cachedSprite == null)
                {
                    var tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
                    tex.SetPixel(0, 0, Color.white);
                    tex.Apply();
                    cachedSprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
                }
                return cachedSprite;
            }
        }

        protected Font DefaultFont
        {
            get
            {
                if (cachedFont == null)
                {
                    cachedFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                }
                return cachedFont;
            }
        }

        protected GameObject CreateBox(string name, Color color, Vector2 position, Vector2 size, Transform parent)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = position;
            rt.sizeDelta = size;
            var image = go.AddComponent<Image>();
            image.sprite = WhiteSprite;
            image.color = color;
            image.type = Image.Type.Simple;
            return go;
        }

        protected Text CreateLabel(string name, string text, Transform parent, Vector2 position, Vector2 size, int fontSize, TextAnchor align, Color color)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = position;
            rt.sizeDelta = size;
            var label = go.AddComponent<Text>();
            label.font = DefaultFont;
            label.text = text;
            label.fontSize = fontSize;
            label.alignment = align;
            label.color = color;
            label.horizontalOverflow = HorizontalWrapMode.Overflow;
            label.verticalOverflow = VerticalWrapMode.Overflow;
            label.raycastTarget = false;
            return label;
        }
    }
}
