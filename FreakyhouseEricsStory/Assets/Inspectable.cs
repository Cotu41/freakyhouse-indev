using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public delegate void InspectEvent();

[System.Serializable]
public class InspectionEvent : UnityEvent
{

}

[RequireComponent(typeof(Collider))]
public class Inspectable : MonoBehaviour
{

    public InspectionEvent OnInpected;
    public float time_takePhoneOut, time_putPhoneAway;

    public InspectionEvent OnVoiceline;


    public static event PhoneEvent OnPhoneStart, OnPhoneEnd;

    public string name;
    public AudioClip convo;



    void Start()
    {
        Physics.queriesHitTriggers = true;
    }

    private void OnMouseOver()
    {
        
    }

    void OnInteract()
    {
        Player.player.PlayLine(new Player.VoiceLine(convo, time_takePhoneOut, time_putPhoneAway));
        this.OnInpected?.Invoke();
        //StartCoroutine(UpdatePhoneAnims());
    }

    IEnumerator UpdatePhoneAnims()
    {
        yield return new WaitForSeconds(time_takePhoneOut);

        OnPhoneStart?.Invoke();

        yield return new WaitForSeconds(time_putPhoneAway);

        OnPhoneEnd?.Invoke();
    }
}
