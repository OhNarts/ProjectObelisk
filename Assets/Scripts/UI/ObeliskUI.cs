using UnityEngine;


public class ObeliskUI : MonoBehaviour
{
    [SerializeField] private Canvas _pauseMenu;
    [SerializeField] private Canvas _inGameUI;

    void Start() {
        if (GameManager.Paused) {
            _inGameUI.enabled = false;
            _pauseMenu.enabled = true;
        } else {
            _inGameUI.enabled = true;
            _pauseMenu.enabled = false;
        }
        GameManager.OnGamePauseChange += OnGamePauseChange;
    }

    void OnDestroy() {
        GameManager.OnGamePauseChange -= OnGamePauseChange;
    }

    private void OnGamePauseChange(object sender, OnGamePauseChangeArgs e) {
        if (e.Paused) {
            _inGameUI.enabled = false;
            _pauseMenu.enabled = true;
        } else {
            _inGameUI.enabled = true;
            _pauseMenu.enabled = false;
        }
    }
}
