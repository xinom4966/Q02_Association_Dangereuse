using UnityEngine;

public class JoinLobbyButton : MonoBehaviour
{
    public bool needPassword;
    public string lobbyId;

    public void JoinLobbyButtonClicked()
    {
        LobbyManager.Instance.JoinLobby(lobbyId, needPassword);
    }
}
