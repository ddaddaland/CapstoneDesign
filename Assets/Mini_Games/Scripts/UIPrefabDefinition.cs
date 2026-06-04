using UnityEngine;
using UnityEngine.UI;

namespace MiniGame
{
    [AddComponentMenu("MiniGame/UI Prefab Definition")]
    public class UIPrefabDefinition : MonoBehaviour
    {
        public Vector2 anchorMin = new Vector2(0.5f, 0.5f);
        public Vector2 anchorMax = new Vector2(0.5f, 0.5f);
        public Vector2 pivot = new Vector2(0.5f, 0.5f);
        public Vector2 anchoredPosition = Vector2.zero;
        public Vector2 size = new Vector2(100f, 100f);
        public bool stretchToParent;

        public bool addImage = true;
        public Color imageColor = Color.white;
        public bool raycastTarget;

        [Header("Sprite Image (선택)")]
        [Tooltip("여기에 스프라이트를 연결하면 단색 대신 이미지로 표시됩니다.")]
        public Sprite imageSprite;

        [Tooltip("Simple: 원본 비율 유지 / Sliced: 9-slice / Filled: 게이지 등에 사용")]
        public Image.Type imageType = Image.Type.Simple;

        [Tooltip("true로 설정하면 이미지가 RectTransform 크기에 맞게 늘어납니다.")]
        public bool preserveAspect = false;
     

        public bool addOutline;
        public Color outlineColor = Color.black;
        public Vector2 outlineDistance = new Vector2(1f, -1f);

        public bool addButton;

        public bool addLabel;
        [TextArea] public string labelText = string.Empty;
        public Vector2 labelPosition = Vector2.zero;
        public Vector2 labelSize = new Vector2(160f, 50f);
        public int labelFontSize = 24;
        public TextAnchor labelAlignment = TextAnchor.MiddleCenter;
        public Color labelColor = Color.white;
        public bool labelRaycastTarget;
        public bool labelStretchToParent = true;

        public bool addLabelOutline;
        public Color labelOutlineColor = Color.black;
        public Vector2 labelOutlineDistance = new Vector2(1f, -1f);

        private static Sprite whiteSprite;
        private static Font defaultFont;

        private void Awake()
        {
            Apply();
        }

        public void Apply()
        {
            var rt = GetOrAdd<RectTransform>(gameObject);
            if (stretchToParent)
            {
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;
            }
            else
            {
                rt.anchorMin = anchorMin;
                rt.anchorMax = anchorMax;
                rt.pivot = pivot;
                rt.anchoredPosition = anchoredPosition;
                rt.sizeDelta = size;
            }

            if (addImage)
            {
                GetOrAdd<CanvasRenderer>(gameObject);
                var image = GetOrAdd<Image>(gameObject);


                if (imageSprite != null)
                {
                    image.sprite = imageSprite;
                }
                else
                {
                    image.sprite = WhiteSprite;
                }

                image.type = imageType;
                image.preserveAspect = preserveAspect;
              

                image.color = imageColor;
                image.raycastTarget = raycastTarget;

                if (addButton)
                {
                    var button = GetOrAdd<Button>(gameObject);
                    button.targetGraphic = image;
                }
            }

            if (addOutline)
            {
                var outline = GetOrAdd<Outline>(gameObject);
                outline.effectColor = outlineColor;
                outline.effectDistance = outlineDistance;
            }

            if (addLabel)
            {
                var labelTransform = transform.Find("Label");
                GameObject labelObject;
                if (labelTransform == null)
                {
                    labelObject = new GameObject("Label");
                    labelObject.transform.SetParent(transform, false);
                }
                else
                {
                    labelObject = labelTransform.gameObject;
                }

                var labelRt = GetOrAdd<RectTransform>(labelObject);
                if (labelStretchToParent)
                {
                    labelRt.anchorMin = Vector2.zero;
                    labelRt.anchorMax = Vector2.one;
                    labelRt.pivot = new Vector2(0.5f, 0.5f);
                    labelRt.offsetMin = Vector2.zero;
                    labelRt.offsetMax = Vector2.zero;
                }
                else
                {
                    labelRt.anchorMin = labelRt.anchorMax = labelRt.pivot = new Vector2(0.5f, 0.5f);
                    labelRt.anchoredPosition = labelPosition;
                    labelRt.sizeDelta = labelSize;
                }

                GetOrAdd<CanvasRenderer>(labelObject);
                var label = GetOrAdd<Text>(labelObject);
                label.font = DefaultFont;
                label.text = labelText;
                label.fontSize = labelFontSize;
                label.alignment = labelAlignment;
                label.color = labelColor;
                label.horizontalOverflow = HorizontalWrapMode.Overflow;
                label.verticalOverflow = VerticalWrapMode.Overflow;
                label.raycastTarget = labelRaycastTarget;

                if (addLabelOutline)
                {
                    var outline = GetOrAdd<Outline>(labelObject);
                    outline.effectColor = labelOutlineColor;
                    outline.effectDistance = labelOutlineDistance;
                }
            }
        }

        private static T GetOrAdd<T>(GameObject target) where T : Component
        {
            var component = target.GetComponent<T>();
            return component != null ? component : target.AddComponent<T>();
        }

        private static Sprite WhiteSprite
        {
            get
            {
                if (whiteSprite == null)
                {
                    var texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
                    texture.SetPixel(0, 0, Color.white);
                    texture.Apply();
                    whiteSprite = Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f));
                }

                return whiteSprite;
            }
        }

        private static Font DefaultFont
        {
            get
            {
                if (defaultFont == null)
                {
                    defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                }

                return defaultFont;
            }
        }
    }
}