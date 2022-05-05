using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void PhoneEvent();
[RequireComponent(typeof(Collider))]
public class CollisionVoiceline : MonoBehaviour
{
    public string[] allowedTags = { "Player" };

    public bool oneshot = true;
    public AudioClip convo;

    public float time_takePhoneOut, time_putPhoneAway;

    public InspectionEvent OnVoiceline;


    public static event PhoneEvent OnPhoneStart, OnPhoneEnd;

    bool hit = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    bool allowsTag(string otherTag)
    {
        foreach(string t in allowedTags)
        {
            if (t.Equals(otherTag)) return true;
        }
        return false;
    }

    public void ManualFire()
    {
        if (!(hit && oneshot))
        {


            hit = true;
            //Player.player.PlayLine(convo, true);

            Player.VoiceLine line = new Player.VoiceLine(convo, time_takePhoneOut, time_putPhoneAway);
            Player.player.PlayLine(line, true);


            OnVoiceline?.Invoke();
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!(hit && oneshot) && allowsTag(other.tag))
        {
            hit = true;
            //Player.player.PlayLine(convo, true);

            Player.VoiceLine line = new Player.VoiceLine(convo, time_takePhoneOut, time_putPhoneAway);
            Player.player.PlayLine(line, true);

            
            OnVoiceline?.Invoke();
        }
    }

    IEnumerator UpdatePhoneAnims()
    {
        yield return new WaitForSeconds(time_takePhoneOut);

        OnPhoneStart?.Invoke();

        yield return new WaitForSeconds(time_putPhoneAway);

        OnPhoneEnd?.Invoke();
        this.gameObject.SetActive(false);
    }

    
}
