using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace MiniGame
{
    /// <summary>
    /// 미니게임 씬을 직접 Play 할 때 UI·GameManager·EventSystem을 자동 구성합니다.
    /// Main 씬에서 Additive 로드될 때는 GameManager가 이미 있으므로 아무 것도 하지 않습니다.
    /// </summary>
    [DefaultExecutionOrder(-100)]
    public class MicroGameSceneHost : MonoBehaviour
    {
        private void Awake()
        {
            if (FindFirstObjectByType<GameManager>() != null)
                return;

            var microGame = GetComponent<MicroGameBase>();
            if (microGame == null)
                microGame = GetComponentInChildren<MicroGameBase>(true);

            if (microGame == null)
            {
                Debug.LogError("MicroGameSceneHost: MicroGameBase 컴포넌트를 찾을 수 없습니다.");
                return;
            }

            EnsureEventSystem();

            var canvasPrefab = Resources.Load<GameObject>("Prefabs/UI/Canvas");
            if (canvasPrefab == null)
            {
                Debug.LogError("MicroGameSceneHost: Resources/Prefabs/UI/Canvas 를 찾을 수 없습니다.");
                return;
            }

            var canvasInstance = Instantiate(canvasPrefab);
            canvasInstance.transform.localScale = Vector3.one;

            var managerObject = new GameObject("GameManager");
            var manager = managerObject.AddComponent<GameManager>();
            manager.BindUiFromCanvas(canvasInstance.transform);
            manager.EnableStandaloneMode(microGame);
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
    }
}
