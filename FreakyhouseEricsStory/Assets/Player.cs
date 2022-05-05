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

    CinemachineBrain vm;

    public delegate void PlayerEvent();
    public static event PlayerEvent OnGetCellarKey;
    public Texture2D cursor;
    public LayerMask seenMask;

    public Transform head;
    bool frozen = false;
    bool lookFrozen = false;

    public bool inDialogue = false;
    VoiceLine current;

    public PhoneAnimationManager phone;
    public AudioSource voice;

    Coroutine speech;
    void Start()
    {
        //Camera.main.
        if (player != null)
            Debug.LogWarning("MULTIPLE PLAYERS IN SCENE");
        player = this;
        Cursor.lockState = CursorLockMode.Locked;
        eyes = Camera.main;
        vm = GetComponentInChildren<CinemachineBrain>();

        speech = StartCoroutine(SayPendingVoicelines());
        //Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);

    }

    public void GiveKey()
    {
        OnGetCellarKey();
    }

    public void EnableLookFreeze() { setLookFreeze(true); }
    public void DisableLookFreeze() { setLookFreeze(false); }

    public void setLookFreeze(bool freeze)
    {
        return;
        lookFrozen = freeze;
        if (vm != null)
        {
            vm.enabled = !freeze;
        }
    }

    public void KO()
    {
        Debug.Log("OOF!");
        OnKnockout?.Invoke();
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

    public class VoiceLine : System.IEquatable<VoiceLine>
    {
        AudioClip clip;
        public readonly float phoneIn, phoneOut;
        public float currentProgress;
        public VoiceLine(AudioClip clip,  float phoneIn = 0, float phoneOut = 0, float startTime = 0)
        {
            this.clip = clip;
            this.phoneIn = phoneIn;
            this.phoneOut = phoneOut;
            this.currentProgress = startTime;
            if (this.phoneOut > clip.length) this.phoneOut = clip.length;
        }

        public AudioClip Clip()
        {
            return clip;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as VoiceLine);
        }

        public bool Equals(VoiceLine other)
        {

            if (other != null)
            {

                if(clip.name.Equals(other.clip.name))
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            return false;


        }

        public override int GetHashCode()
        {
            return clip.GetHashCode();
        }
    }


    Stack<VoiceLine> linesToSay = new Stack<VoiceLine>();

    public AudioClip [] interruptionLines;
    public AudioClip[] backToConvoLines;

    public void PlayLine(VoiceLine line, bool force = false)
    {
        if(voice.isPlaying)
        {
            if(linesToSay.Count >= 9)
            {
                if (!force) return;
            }

            Debug.Log(linesToSay.Contains(line));
            if (linesToSay.Contains(line) || current.Equals(line)) return;

            Debug.Log("logged stop at time: " + current.currentProgress);
            linesToSay.Push(current);
            linesToSay.Push(new VoiceLine(backToConvoLines[Random.Range(0, backToConvoLines.Length)]));
            linesToSay.Push(line);
            linesToSay.Push(new VoiceLine(interruptionLines[Random.Range(0, interruptionLines.Length)]));
            voice.Stop();
            StopCoroutine(speech);
            speech = StartCoroutine(SayPendingVoicelines());
            //current = null;
        }
        else
        {
            linesToSay.Push(line);
        }
    }

    public void StopAllLines()
    {
        StopCoroutine(speech);
        if (voice.isPlaying) voice.Stop();
        inDialogue = false;
        linesToSay.Clear();
        speech = StartCoroutine(SayPendingVoicelines());
    }


    float progressInLine = 0;
    IEnumerator SayPendingVoicelines()
    {
        while (true)
        {
            
            if (!voice.isPlaying && linesToSay.Count > 0)
            {
                current = linesToSay.Pop();
                voice.time = (float)((int)current.currentProgress);
                progressInLine = current.currentProgress;
                voice.clip = current.Clip();
                voice.Play();
                
                voice.SetScheduledEndTime(AudioSettings.dspTime + (current.Clip().length - current.currentProgress));
                Debug.Log(current.Clip().name + " started at " + voice.time + "s (" + current.currentProgress + ")");
                
            }

            if (voice.isPlaying)
            {
                float p_in = current.phoneIn;
                float p_out = current.phoneOut;
                progressInLine += Time.deltaTime;
                current.currentProgress = progressInLine;
                if (progressInLine >= p_out)
                {
                    phone.OnPhoneEnd();
                }
                else if (progressInLine >= p_in)
                {
                    phone.OnPhoneStart();
                }
            }
            else
            {
                phone.OnPhoneEnd();
            }
            yield return null;
            
        }

    }


    private void OnApplicationFocus(bool focus)
    {
        if(focus)
        {
            setLookFreeze(false);
            Debug.Log("Focus Gained");
        }
        else
        {
            setLookFreeze(true);
            Debug.Log("Focus Lost");
        }
    }



    // Update is called once per frame
    void Update()
    {
        Vector3 movement = Vector3.zero;

        //if(lookFrozen) vm.ForceCameraPosition(Camera.main.transform.position, Camera.main.transform.rotation);


        if (!frozen)
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
        {
            setLookFreeze(lookFrozen ? false : true);
            
        }

        transform.position += movement * speed * Time.deltaTime;
    }
}
