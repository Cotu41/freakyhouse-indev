using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Looper : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("Player"))
        {
            Player.player.transform.rotation = Quaternion.Inverse(Player.player.transform.rotation);
            other.transform.rotation = Quaternion.Inverse(other.transform.rotation);
        }
        
    }
}
