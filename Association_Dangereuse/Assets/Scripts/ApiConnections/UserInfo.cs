using UnityEngine;

public class UserInfo : MonoBehaviour
{
    private static UserInfo instance;
    private int userID;
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

    public int GetUserID() {  return userID; }
    public string GetUserName() { return userName; }
    public string GetUserPassword() { return userPassword; }
    public WWWForm GetUserInfosAsForm()
    {
        WWWForm form = new WWWForm();
        form.AddField("userID", userID);
        form.AddField("username", userName);
        form.AddField("password", userPassword);
        return form;
    }
}
