using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void OnPressNewGame()
    {
        SceneManager.LoadScene("GameScene"); // Replace "GameScene" with the name of your empty game scene.
    }

    public void OnPressLoadGame()
    {
        SceneManager.LoadScene("GameScene"); // Replace "GameScene" with the name of your empty game scene.
    }

    public void OnPressExit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
