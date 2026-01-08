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
        for (int i = 0; i < saveLimit; i++)
        {
            StartCoroutine(GetRequest("http://sitedemerde.com/phpfile"));
        }
        DisplaySaves();
    }

    IEnumerator GetRequest(string uri)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(uri);
        yield return webRequest.SendWebRequest();
        switch (webRequest.result)
        {
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.DataProcessingError:
                Debug.LogError(string.Format("Something went wrong : {0}", webRequest.error));
                break;
            case UnityWebRequest.Result.Success:
                Save fetchedSave = JsonConvert.DeserializeObject<Save>(webRequest.downloadHandler.text);
                savesList.Add(fetchedSave);
                break;
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
