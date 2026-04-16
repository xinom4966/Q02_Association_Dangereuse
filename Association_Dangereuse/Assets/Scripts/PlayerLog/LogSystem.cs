using Newtonsoft.Json;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class LogSystem : MonoBehaviour
{
    [SerializeField] private TMP_InputField profileNameField;
    [SerializeField] private TMP_InputField profilePasswordField;
    private string tempUserName;
    private string tempPassword;
    WWWForm form = new WWWForm();

    public void Login()
    {
        tempUserName = profileNameField.text;
        tempPassword = profilePasswordField.text;
        UserInfo tempInfos = new UserInfo();
        tempInfos.SetUserName(tempUserName);
        tempInfos.SetPassword(tempPassword);
        form = tempInfos.GetUserInfosAsForm();
        StartCoroutine(PostRequest("http://sitedemerde.com/LogPlayer.php", form));
    }

    public void Logout()
    {
        UserInfo.GetInstance().EraseInfos();
    }

    IEnumerator PostRequest(string uri, WWWForm form)
    {
        UnityWebRequest webRequest = UnityWebRequest.Post(uri, form);
        yield return webRequest.SendWebRequest();
        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            CustomDebug.Instance.DebugLog("Something went wrong while uploading data.");
        }
        StartCoroutine(GetRequest(uri));
    }

    IEnumerator GetRequest(string uri)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(uri);
        yield return webRequest.SendWebRequest();
        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            bool logWasSuccessful = JsonConvert.DeserializeObject<bool>(webRequest.downloadHandler.text);
            if (logWasSuccessful)
            {
                UserInfo.GetInstance().SetUserName(tempUserName);
                UserInfo.GetInstance().SetPassword(tempPassword);
            }
            else
            {
                CustomDebug.Instance.DebugLog("Log was unsuccessful.(wrong password)");
            }
        }
        else
        {
            CustomDebug.Instance.DebugLog("Something went wrong while downloading data.");
        }
    }
}
