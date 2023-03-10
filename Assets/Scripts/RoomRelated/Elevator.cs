
using UnityEngine;
using UnityEngine.SceneManagement;

public class Elevator : MonoBehaviour, Interactable
{
    public void Interact(PlayerController player)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
