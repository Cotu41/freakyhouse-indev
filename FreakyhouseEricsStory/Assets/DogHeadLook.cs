using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DogHeadLook : MonoBehaviour
{

    public bool looking = true;
    public Transform target;


    Vector3 toTarget;
    // Start is called before the first frame update
    void Start()
    {
        UpstairsDogController.OnWaypointArrival += UpstairsDogController_OnWaypointArrival;
    }

    private void UpstairsDogController_OnWaypointArrival(DogWaypoint point, UpstairsDogController dog)
    {
        if (!point.shouldLookAtPlayer) looking = false;
        else looking = true;
    }

    // Update is called once per frame
    void Update()
    {
        


        if (looking)
        {

                
            Vector3 toTarget = (target.position - transform.root.position).normalized;
            //float dotprod = Vector3.Dot()
            if (Vector3.Dot(toTarget, transform.root.forward) > 0)
            {
                transform.LookAt(target);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Debug.DrawLine(transform.position, transform.position + toTarget);
    }
}
