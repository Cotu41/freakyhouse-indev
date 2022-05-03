using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningWindow : MonoBehaviour
{
    MeshRenderer rend;
    Light light;

    float originalStrength;

    public Material normal, lightning;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<MeshRenderer>();
        light = GetComponentInChildren<Light>();
        originalStrength = light.intensity;
        light.intensity = 0;
    }

    // Update is called once per frame
    void Update()
    {
        rend.material.Lerp(normal, lightning, LightningDirector.LightningLevel);
        light.intensity = Mathf.Lerp(0, originalStrength, LightningDirector.LightningLevel);

    }
}
