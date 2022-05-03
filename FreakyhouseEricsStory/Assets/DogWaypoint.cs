using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogWaypoint : MonoBehaviour
{
    public bool shouldCloak = false;
    public bool shouldLookAtPlayer = true;

    Renderer renderer;

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<Renderer>();
    }

    public bool isClear()
    {
        if (renderer.isVisible) return false;
        return true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
