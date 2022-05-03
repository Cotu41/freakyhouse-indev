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

    private void OnTriggerEnter(Collider other)
    {
        if (!(hit && oneshot) && other.tag.Equals("Player"))
        {
            hit = true;
            Player.player.voice.PlayOneShot(convo);
            if (time_putPhoneAway == -1) time_putPhoneAway = convo.length;
            else
            {
                time_putPhoneAway -= time_takePhoneOut;
            }
            OnVoiceline?.Invoke();
            StartCoroutine(UpdatePhoneAnims());
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
