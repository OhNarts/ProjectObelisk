
using UnityEngine;
using UnityEngine.SceneManagement;

public class Elevator : MonoBehaviour, Interactable
{
    public void Interact(PlayerController player)
    {
        player.HealthHandler.Heal(100);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
