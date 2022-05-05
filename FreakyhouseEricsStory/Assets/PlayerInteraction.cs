using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PlayerInteraction : MonoBehaviour
{
    public KeyCode interactionKey;
    public LayerMask mask;

    Vector2 centerpoint;

    Camera camera;
    // Start is called before the first frame update
    void Start()
    {
        centerpoint = new Vector2(Screen.width / 2, Screen.height / 2);
        camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(interactionKey))
        {
            sendInteractionMessage();
        }
    }

    void sendInteractionMessage()
    {
        Ray ray = camera.ScreenPointToRay(centerpoint);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 10, mask, QueryTriggerInteraction.Collide))
        {
            if(hit.collider.tag.Equals("Inspectable"))
                hit.collider.SendMessage("OnInteract");
        }

        
    }
}
