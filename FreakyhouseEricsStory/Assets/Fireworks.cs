using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class Fireworks : MonoBehaviour
{
    public AnimationCurve BurstCurve;

    Light light;
    Vector3 og_position;
    float og_intensity;
    public float duration = 2;
    Color[] colors = { Color.white, Color.blue, Color.yellow, Color.red };
    AudioSource sound;
    // Start is called before the first frame update
    void Start()
    {
        og_position = transform.position;
        light = GetComponent<Light>();
        og_intensity = light.intensity;
        light.intensity = 0;
        StartCoroutine(TriggerFirework());
        sound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator TriggerFirework()
    {
        while(true)
        {
            transform.position = og_position + Vector3.right * Random.Range(0, 20);
            light.color = colors[Random.Range(0, colors.Length)];

            StartCoroutine(Burst());

            yield return new WaitForSeconds(Random.Range(2, 3)) ;
        }
    }

    IEnumerator Burst()
    {
        
        float elapsed = 0;
        while(true)
        {
            elapsed += Time.deltaTime;
            light.intensity = og_intensity*BurstCurve.Evaluate(elapsed);
            
            if (elapsed >= duration) break;
            yield return null;
        }

        
    }
}
