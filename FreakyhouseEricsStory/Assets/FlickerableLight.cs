using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickerableLight : MonoBehaviour
{
    Light light;
    float og_intensity;

    public int number_of_flickers = 3;
    public float flickerMinDuration, flickerMaxDuration;
    public float flicker_intensity = 0.6f;

    // Start is called before the first frame update
    void Start()
    {
        LightningDirector.OnLightningStrike += LightningDirector_OnLightningStrike;
        light = GetComponent<Light>();
        og_intensity = light.intensity;
    }

    private void LightningDirector_OnLightningStrike()
    {
        StartCoroutine(Flicker());
    }

    IEnumerator Flicker()
    {

        float[] flickerValues = new float[number_of_flickers+1];
        for(int i = 0; i < flickerValues.Length; i++)
        {
            flickerValues[i] = Random.Range(0, og_intensity*flicker_intensity);
        }

        flickerValues[flickerValues.Length - 1] = 0;
        int index = 0;
        float dur = Random.Range(flickerMinDuration, flickerMaxDuration);
        float t = 0;
        while(true)
        {
            light.intensity = Mathf.Lerp(og_intensity, og_intensity - flickerValues[index], t/dur);
            t += Time.deltaTime;

            if(t/dur >= 1)
            {
                index++;
                t = 0;
                if (index >= flickerValues.Length) break;
            }
            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
