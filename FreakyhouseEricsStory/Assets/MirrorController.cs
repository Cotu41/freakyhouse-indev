using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorController : MonoBehaviour
{
    // Start is called before the first frame update

    Camera cam;

    bool enableReflections = false;
    ReflectionProbe probe;

    void Start()
    {
        cam = Camera.main;
        probe = GetComponentInChildren<ReflectionProbe>();
        //probe.refreshMode = UnityEngine.Rendering.ReflectionProbeRefreshMode.ViaScripting;
        probe.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        //Vector3 t = new Vector3(transform.position.x, transform.position.y, cam.transform.position.z);
        //transform.position = Vector3.Lerp(transform.position, t, 0.99f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            enableReflections = true;
            //probe.refreshMode = UnityEngine.Rendering.ReflectionProbeRefreshMode.EveryFrame;
            probe.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            enableReflections = false;
            //probe.refreshMode = UnityEngine.Rendering.ReflectionProbeRefreshMode.ViaScripting;
            probe.enabled = false;
        }
    }
}
