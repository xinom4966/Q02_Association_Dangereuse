using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CustomDebug : MonoBehaviour
{
    [SerializeField] GameObject debugItemPrefab;
    [SerializeField] GameObject debugItemParent;
    [SerializeField] int maxDebugItems;
    [SerializeField] float maxDestroyTime;
    [SerializeField] float offset;
    float currentTime;
    List<GameObject> debugItems = new List<GameObject>();
    public static CustomDebug Instance;
    
    //allows debuging in build
    //yes i know all of this is overkill, but it might become realy usefull for other projects
    private void Awake()
    {
        Instance = this;
        currentTime = maxDestroyTime;
    }

    private void Update()
    {
        if (currentTime <= 0)
        {
            if (debugItems.Count > 0 && debugItems != null)
            {
                foreach (GameObject debugItem in debugItems)
                {
                    Destroy(debugItem);
                }
                debugItems.Clear();
            }
            return;
        }
        currentTime -= Time.deltaTime;
    }

    public void DebugLog(string msg)
    {
        currentTime = maxDestroyTime;
        GameObject tempDebugItem;
        if (debugItems.Count >= maxDebugItems)
        {
            GameObject temp = debugItems[0];
            debugItems.RemoveAt(0);
            Destroy(temp);
            foreach (GameObject debugItem in debugItems)
            {
                debugItem.transform.position += new Vector3(0, offset, 0);
            }
        }
        
        if (debugItems.Count == 0)
        {
            tempDebugItem = Instantiate(debugItemPrefab, debugItemParent.transform);
        }
        else
        {
            tempDebugItem = Instantiate(debugItemPrefab, debugItemParent.transform);
            tempDebugItem.transform.position -= new Vector3(0, offset * debugItems.Count, 0);
        }
        tempDebugItem.GetComponent<TextMeshProUGUI>().text = msg;
        debugItems.Add(tempDebugItem);
        
    }
}
