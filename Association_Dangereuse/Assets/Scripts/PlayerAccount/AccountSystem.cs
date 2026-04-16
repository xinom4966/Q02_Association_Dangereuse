using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class AccountSystem : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameField;
    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private TMP_InputField passwordConfirmationField;
    private string tempUserName;
    private string tempPassword;
    WWWForm form = new WWWForm();

    public void TryCreateAccount()
    {
        if (passwordField.text != passwordConfirmationField.text)
        {
            //TODO : Implement rejection ui
            CustomDebug.Instance.DebugLog("Password and password confirmation are different.");
            return;
        }
        tempUserName = usernameField.text;
        tempPassword = passwordField.text;
        UserInfo tempInfos = new UserInfo();
        tempInfos.SetUserName(tempUserName);
        tempInfos.SetPassword(tempPassword);
        form = tempInfos.GetUserInfosAsForm();
        StartCoroutine(PostRequest("http://sitedemerde.com/LogPlayer.php", form));
    }

    IEnumerator PostRequest(string uri, WWWForm form)
    {
        UnityWebRequest webRequest = UnityWebRequest.Post(uri, form);
        yield return webRequest.SendWebRequest();
        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            CustomDebug.Instance.DebugLog("Something went wrong while uploading data.");
        }
    }
}
