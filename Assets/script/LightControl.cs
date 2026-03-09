using UnityEngine;
public class LightControl : MonoBehaviour
{
    public bool enableLight = true;
    private Light spotLight;
    public float flashFrequency;
    public float flashDuration;
    private float lastFlash;
    void Start()
    {
        spotLight = GetComponent<Light>();
        lastFlash = Time.time;
    }

    void Update()
    {
        if (enableLight)
        {
            if (Time.time - lastFlash > flashDuration)
            {
                spotLight.enabled = false;
            }
            if (Time.time - lastFlash > 1 / flashFrequency)
            {
                spotLight.enabled = true;
                lastFlash = Time.time;
            }
        }
    }
}
