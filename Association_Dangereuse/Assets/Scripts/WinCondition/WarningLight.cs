using UnityEngine;

public class WarningLight : MonoBehaviour
{
    [SerializeField] private Light myLight;
    [SerializeField] private int minIntensity = 10;
    [SerializeField] private int maxIntensity = 100;
    private float timer = 0.0f;

    private void Update()
    {
        myLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, timer);

        timer += 0.5f * Time.deltaTime;

        if (timer > 1.0f)
        {
            int temp = maxIntensity;
            maxIntensity = minIntensity;
            minIntensity = temp;
            timer = 0.0f;
        }
    }
}
