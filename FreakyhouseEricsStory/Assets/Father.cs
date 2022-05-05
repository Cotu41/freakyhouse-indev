using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Father : MonoBehaviour
{

    bool hunting = false;
    public LayerMask detectionMask;
    public Transform EricsRoom;
    public float lingerTime = 5f;

    public Transform FathersRoom;

    public AudioSource footsteps;

    public AudioClip slow, brisk, fast;

    NavMeshAgent nav;

    Transform currentDest;
    bool chasing = false;

    // Start is called before the first frame update
    void Start()
    {
        Player.OnGetCellarKey += Player_OnGetCellarKey;
        nav = GetComponent<NavMeshAgent>();
        
    }

    private void Player_OnGetCellarKey()
    {
        hunting = true;
    }



    // Update is called once per frame
    void Update()
    {
        if (nav.isStopped) footsteps.Stop();
        if(hunting)
        {
            if (currentDest == null)
            {
                currentDest = EricsRoom;
                nav.SetDestination(EricsRoom.position);
                footsteps.clip = brisk;
                footsteps.Play();
            }
            else
            {
                if(nav.remainingDistance <= nav.stoppingDistance)
                {
                    if(currentDest == EricsRoom)
                    {
                        currentDest = FathersRoom;
                        nav.SetDestination(FathersRoom.position);
                        footsteps.clip = slow;
                        StartCoroutine(Linger());
                    }
                    else
                    {
                        hunting = false;
                    }
                }
            }
            
        }


    }

    IEnumerator Linger()
    {
        nav.isStopped = true;
        yield return new WaitForSeconds(lingerTime);
        nav.isStopped = false;
        footsteps.Play();
    }

    private void OnTriggerStay(Collider other)
    {
        if(!chasing && other.tag.Equals("Player"))
        {
            RaycastHit hit;
            if(Physics.Raycast(transform.position, other.transform.position-transform.position, out hit, 5))
            {

                if (hit.collider.tag.Equals("Player"))
                {
                    StopAllCoroutines();

                    StartCoroutine(Chase());
                }
            }
        }
    }

    IEnumerator Chase()
    {

        footsteps.clip = fast;
        nav.speed = 7;
        nav.angularSpeed = 130;
        chasing = true;
        footsteps.Play();
        nav.isStopped = false;
        while(true)
        {
            nav.SetDestination(Player.player.transform.position);
            yield return new WaitForSeconds(0.33f);
        }
    }

    public void EndChase()
    {
        StopAllCoroutines();
        nav.isStopped = true;
        footsteps.Stop();

    }
}
