using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleGateNegative : MonoBehaviour
{
    GameObject[] affected;

    // Start is called before the first frame update
    void Start()
    {
        affected = GetComponentInParent<ObjectToggleGate>().affected;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            foreach (GameObject o in affected) o.SetActive(false);
        }
    }
}
