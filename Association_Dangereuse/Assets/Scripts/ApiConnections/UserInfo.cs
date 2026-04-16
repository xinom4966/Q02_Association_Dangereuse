using UnityEngine;

public class UserInfo : MonoBehaviour
{
    private static UserInfo instance;
    private string userName;
    private string userPassword;

    private void Awake()
    {
        if (UserInfo.GetInstance() == null)
        {
            UserInfo.instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public static UserInfo GetInstance()
    {
        return instance;
    }

    public string GetUserName() { return userName; }
    public void SetUserName(string newName)
    {
        userName = newName;
    }
    public string GetUserPassword() { return userPassword; }
    public void SetPassword(string newPassWord)
    {
        userPassword = newPassWord;
    }
    public WWWForm GetUserInfosAsForm()
    {
        WWWForm form = new WWWForm();
        form.AddField("pseudo", userName);
        form.AddField("passWrd", userPassword);
        return form;
    }

    public void EraseInfos()
    {
        userName = "";
        userPassword = "";
    }
}