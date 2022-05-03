using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TVLight : MonoBehaviour
{

    public bool doing = true;

    Light light;

    // Start is called before the first frame update
    void Start()
    {
        light = GetComponent<Light>();

        StartCoroutine(showNewLight());
    }

    // Update is called once per frame
    void Update()
    {
        //light.intensity = Mathf.Round(Mathf.PerlinNoise(Random.Range(0, 1), Time.time/2)*100)/100*6.46f;


        
    }

    IEnumerator showNewLight()
    {
        while (true)
        {
            float red = Mathf.PerlinNoise(Random.Range(0, 1), Random.Range(0, 500)) * 255;
            float blue = Mathf.PerlinNoise(Random.Range(2, 3), Random.Range(0, 500)) * 255;
            float green = Mathf.PerlinNoise(Random.Range(4, 5), Random.Range(0, 500)) * 255;


            light.intensity = Random.Range(30, 90);
            yield return new WaitForSeconds(Random.Range(0.3f, 3));
        }
    }
}
