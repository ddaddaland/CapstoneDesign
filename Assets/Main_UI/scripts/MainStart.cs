using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainStart : MonoBehaviour
{
    public string gameScene;
    public string uiScene;
    public void OnClickStartButton()
    {
        GameManager.instance.StartLoad();
    }
}
