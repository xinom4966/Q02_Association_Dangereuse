using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CharacterMenu : MonoBehaviour
{
    public void TogglePauseMenu(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }
    }

    public void TogglePauseMenu()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public void ReturnToLobby()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
}
