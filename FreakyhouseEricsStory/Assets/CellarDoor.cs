using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellarDoor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Player.OnGetCellarKey += Player_OnGetCellarKey;
    }

    private void Player_OnGetCellarKey()
    {
        this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
           
    }
}
