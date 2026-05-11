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
    private string tempPasswordConfirmation;
    WWWForm form;

    //Rate limiting variables;
    [Header("Rate limiting")]
    [SerializeField] private int requestLimit = 3;
    //timeFrame is in seconds
    [SerializeField] private int timeFrame = 900;
    private float timer = 0.0f;
    private int requestCounter = 0;

    private void Start()
    {
        form = new WWWForm();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > timeFrame)
        {
            timer = 0.0f;
            requestCounter = 0;
        }
    }

    public void EncryptPasswordInput(string input)
    {
        tempPassword = input;
        foreach(char c in input)
        {
            passwordField.text = passwordField.text.Replace(c, '*');
        }
    }

    public void EncryptPasswordConfirmation(string input)
    {
        tempPasswordConfirmation = input;
        foreach(char c in input)
        {
            passwordConfirmationField.text = passwordConfirmationField.text.Replace(c,'*');
        }
    }

    public void TryCreateAccount()
    {
        tempUserName = usernameField.text;
        tempPassword = passwordField.text;
        tempPasswordConfirmation = passwordConfirmationField.text;

        if (tempUserName == string.Empty || tempPassword == string.Empty || tempPasswordConfirmation == string.Empty)
        {
            //TODO : Implement rejection ui
            CustomDebug.Instance.DebugLog("All fields must be filled.");
            return;
        }

        if (passwordField.text != passwordConfirmationField.text)
        {
            //TODO : Implement rejection ui
            CustomDebug.Instance.DebugLog("Password and password confirmation are different.");
            return;
        }

        //Debug only, need to remove this code of block
        /*else
        {
            CustomDebug.Instance.DebugLog(tempUserName);
            CustomDebug.Instance.DebugLog(tempPassword);
            CustomDebug.Instance.DebugLog(tempPasswordConfirmation);
            return;
        }*/

        requestCounter++;

        if (requestCounter > requestLimit)
        {
            //TODO : Implement rejection ui
            CustomDebug.Instance.DebugLog("Too many requests.");
            return;
        }

        UserInfo tempInfos = new UserInfo();
        tempInfos.SetUserName(tempUserName);
        tempInfos.SetPassword(tempPassword);
        form = tempInfos.GetUserInfosAsForm();
        StartCoroutine(PostRequest("http://sitedemerde.com/CreateAccount.php", form));
    }

    public void TryLogin()
    {
        tempUserName = usernameField.text;
        tempPassword = passwordField.text;

        if (tempUserName == string.Empty || tempPassword == string.Empty)
        {
            //TODO : Implement rejection ui
            CustomDebug.Instance.DebugLog("All fields must be filled.");
            return;
        }

        requestCounter++;

        if (requestCounter > requestLimit)
        {
            //TODO : Implement rejection ui
            CustomDebug.Instance.DebugLog("Too many requests.");
            return;
        }

        UserInfo tempInfos = new UserInfo();
        tempInfos.SetUserName(tempUserName);
        tempInfos.SetPassword(tempPassword);
        form = tempInfos.GetUserInfosAsForm();
        StartCoroutine(PostRequest("http://DangerousAssociationAPI.com/CreateAccount.php", form));
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
