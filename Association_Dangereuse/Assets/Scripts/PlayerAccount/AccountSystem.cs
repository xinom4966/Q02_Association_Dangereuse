using TMPro;
using UnityEngine;

public class AccountSystem : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameField;
    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private TMP_InputField passwordConfirmationField;
    private string tempUserName;
    private string tempPassword;

    public void TryCreateAccount()
    {
        if (passwordField.text != passwordConfirmationField.text)
        {
            //TODO : Implement rejection ui
            CustomDebug.Instance.DebugLog("Password and password confirmation are different.");
            return;
        }
        
    }
}
