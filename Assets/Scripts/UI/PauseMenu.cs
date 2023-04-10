using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public void Resume() {
        GameManager.UnPause();
    }

    public void RestartRoom() {
        if (GameManager.CurrentState != GameState.Combat) return;
        PlayerState.RevertToLastRoom();
        GameManager.UnPause();
    }

    public void RestartLevel() {
        Debug.LogWarning("Level restart not yet fully implemented");
        PlayerState.RevertToLevelStart();
        GameManager.UnPause();
    }

    public void QuitMainMenu() {
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }
    
    public void Quit() {
        Debug.Log("QUIT");
        Application.Quit();
    }
}
