using TMPro;
using UnityEngine;

public class CustomDebug : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI tmpro;
    [HideInInspector] public static CustomDebug Instance;


    private void Awake()
    {
        Instance = this;
    }

    public void UIDebugLog(string msg)
    {
        tmpro.text = msg;
    }
}
