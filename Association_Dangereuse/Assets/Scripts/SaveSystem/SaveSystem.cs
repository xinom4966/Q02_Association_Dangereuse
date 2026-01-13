using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class SaveSystem : MonoBehaviour
{
    public class Save
    {
        public int dayNum;
        public int moneyNum;
    }
    
    private List<Save> savesList = new List<Save>();
    [SerializeField] private List<TextMeshProUGUI> textsList = new List<TextMeshProUGUI>();
    [SerializeField] private int saveLimit;

    private void OnEnable()
    {
        WWWForm form = new WWWForm();
        form = UserInfo.GetInstance().GetUserInfosAsForm();
        StartCoroutine(PostRequest("http://sitedemerde.com/myapi", form));
    }

    IEnumerator PostRequest(string uri, WWWForm form)
    {
        UnityWebRequest webRequest = UnityWebRequest.Post(uri, form);
        yield return webRequest.SendWebRequest();
        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Something went wrong while uploading data.");
        }
        for (int i = 0; i < saveLimit; i++)
        {
            StartCoroutine(GetRequest("http://sitedemerde.com/myapi"));
        }
        DisplaySaves();
    }

    IEnumerator GetRequest(string uri)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(uri);
        yield return webRequest.SendWebRequest();
        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            Save fetchedSave = JsonConvert.DeserializeObject<Save>(webRequest.downloadHandler.text);
            savesList.Add(fetchedSave);
        }
        else
        {
            Debug.LogError("Something went wrong while downloading data.");
        }
    }

    private void DisplaySaves()
    {
        int increment = 0;
        foreach (Save save in savesList)
        {
            textsList[increment].text = "Save " + increment + ", day: " + save.dayNum + ", Money: " + save.moneyNum;
            increment++;
        }
    }
}
