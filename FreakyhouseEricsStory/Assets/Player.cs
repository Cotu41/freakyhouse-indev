using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update

    public InspectionEvent OnKnockout;


    public float speed = 10;
    Camera eyes;

    public static Player player;

    public CinemachineVirtualCamera vm;

    public delegate void PlayerEvent();
    public static event PlayerEvent OnGetCellarKey;
    public Texture2D cursor;
    public LayerMask seenMask;

    public Transform head;
    bool frozen = false;
    public bool inDialogue = false;


    public AudioSource voice;

    void Start()
    {
        //Camera.main.
        if (player != null)
            Debug.LogWarning("MULTIPLE PLAYERS IN SCENE");
        player = this;
        Cursor.lockState = CursorLockMode.Locked;
        eyes = Camera.main;
        vm = GetComponentInChildren<CinemachineVirtualCamera>();
        //Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);

    }

    public void GiveKey()
    {
        OnGetCellarKey();
    }

    public void toggleInDialogue()
    {
        if (inDialogue) setInDialogue(false);
        if (!inDialogue) setInDialogue(true);
    }

    public void toggleLookFreeze()
    {
        

    }

    public void KO()
    {
        Debug.Log("OOF!");
        OnKnockout?.Invoke();
    }

    public void setInDialogue(bool inDialogue)
    {
        if(inDialogue)
        {
            Cursor.lockState = CursorLockMode.Confined;
            frozen = true;
            inDialogue = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            frozen = false;
            inDialogue = false;
        }
    }

    public static bool FrustrumCast(Collider target)
    {
        /*
        Vector3 toTarget = (target.position - transform.root.position).normalized;
        //float dotprod = Vector3.Dot()
        if (Vector3.Dot(toTarget, transform.root.forward) > 0)
        {
            transform.LookAt(target);
        }
        */

        Vector3 screenPos = player.eyes.WorldToScreenPoint(target.ClosestPointOnBounds(player.eyes.transform.position));
        if (screenPos.x < 0 || screenPos.x > Screen.width) return false;
        if (screenPos.y < 0 || screenPos.y > Screen.height) return false;

        // if it's within our frustrum, now we check the cast



        RaycastHit hit;
        Ray ray = player.eyes.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray: ray, out hit, 70f, player.seenMask) == false) return false;

        if (hit.collider.Equals(target)) return true;

        return false;

        



    }

    public void PlayLine(AudioClip line, bool force = false)
    {
        if(voice.isPlaying)
        {
            if(force)
            {

            }
        }
    }

    public void StopAllLines()
    {
        if (voice.isPlaying) voice.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 movement = Vector3.zero;

        if(!frozen)
        {

            



            float x=0, z=0;
            if(Input.GetKey(KeyCode.W))
            {
                z += 1;
            }
            if(Input.GetKey(KeyCode.S))
            {
                z -= 1;
            }
            if(Input.GetKey(KeyCode.A))
            {
                x -= 1;
            }
            if(Input.GetKey(KeyCode.D))
            {
                x += 1;
            }
            movement = Camera.main.transform.forward * z;
            movement += Camera.main.transform.right * x;
            movement.y = 0;
            movement.Normalize();

            

        }

        if (Input.GetKeyDown(KeyCode.K))
            OnGetCellarKey();

        if (Input.GetKeyDown(KeyCode.Escape))
            toggleLookFreeze();
        transform.position += movement * speed * Time.deltaTime;
    }
}
