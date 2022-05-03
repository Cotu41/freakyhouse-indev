using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreakyManager : MonoBehaviour
{

    public delegate void FreakyEvent(int [] args);
    public static event FreakyEvent DogSeen, OnPantryKeyPickup;



    // Start is called before the first frame update
    void Start()
    {
        UpstairsDogController.OnDogSeenFirstTime += PlayerMeetsDog;
        Player.OnGetCellarKey += Player_OnGetCellarKey;
    }

    private void Player_OnGetCellarKey()
    {
        LightningDirector.director.LightningStrike();
    }

    private void PlayerMeetsDog()
    {
        LightningDirector.director.LightningStrike();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
