using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneAnimationManager : MonoBehaviour
{
    Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = this.GetComponent<Animator>();
        CollisionVoiceline.OnPhoneStart += OnPhoneStart;
        CollisionVoiceline.OnPhoneEnd += OnPhoneEnd;
        Inspectable.OnPhoneStart += OnPhoneStart;
        Inspectable.OnPhoneEnd += OnPhoneEnd;
    }

    private void OnPhoneEnd()
    {
        anim.SetBool("OnPhone", false);
    }

    private void OnPhoneStart()
    {
        anim.SetBool("OnPhone", true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
