using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpstairsDogController : MonoBehaviour
{
    public DogWaypoint[] sitpoints;
    float OFFSET_Y = -0.5f;
    SkinnedMeshRenderer mesh;
    public Material normal, cloaked, tux;

    public delegate void DogWaypointEvent(DogWaypoint point, UpstairsDogController dog);
    public delegate void DogFreakyEvent();

    public static event DogWaypointEvent OnWaypointArrival;
    public static event DogFreakyEvent OnDogSeenFirstTime;
    


    bool isCloaked = false;

    bool seenForFirstTime = false;
    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponentInChildren<SkinnedMeshRenderer>();
        StartCoroutine(WeepingAngelRoutine());
        StartCoroutine(Blinking());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if(!mesh.isVisible)
                movePositions();
        }
        if (!seenForFirstTime && Player.FrustrumCast(GetComponent<Collider>()))
        {
            seenForFirstTime = true;
            Debug.Log("Dog has been seen for the first time.");
            OnDogSeenFirstTime();

        }

      
    }

    IEnumerator WeepingAngelRoutine()
    {
        while (true)
        {
            
            yield return new WaitForSeconds(Random.Range(0, 10));
            if (seenForFirstTime && !mesh.isVisible) movePositions(); 
        }
    }

    void movePositions()
    {
        Debug.Log("dog moved!");
        int randIndex;
        int tries = 0;
        int maxTries = 10;
        do
        {
            randIndex = Random.Range(0, sitpoints.Length);
            if (tries >= maxTries) break;
            tries++;
        } while (!sitpoints[randIndex].isClear());


        DogWaypoint point = sitpoints[0];
        float minDist = 100000;
        foreach(DogWaypoint w in sitpoints)
        {
            if((w.transform.position - Player.player.transform.position).sqrMagnitude < minDist)
            {
                minDist = (w.transform.position - Player.player.transform.position).sqrMagnitude;
                point = w;
            }
        }

        StartCoroutine(WaitUntilMove(point));



    }

    private void OnBecameInvisible()
    {
        Debug.Log("No Longer Spotted");
        if (Random.Range(0, 30) < 4) movePositions();
    }

    IEnumerator WaitUntilMove(DogWaypoint point)
    {
        do
        {
            yield return null;
        } while (!point.isClear());
        this.transform.position = point.transform.position + new Vector3(0, OFFSET_Y, 0);
        this.transform.rotation = point.transform.rotation;

        setCloak(point.shouldCloak);
        OnWaypointArrival(point, this);
    }

    IEnumerator Blinking()
    {
        while(true)
        {
            mesh.material.SetColor("Emission", Color.black);
            yield return new WaitForSeconds(Random.Range(2, 5));
            mesh.material.SetColor("Emission", Color.white);
            yield return new WaitForSeconds(0.5f);
        }
    }


    void setCloak(bool cloak)
    {
        isCloaked = cloak;
        if (isCloaked)
        {
            mesh.material = this.cloaked;

        }
        else
        {
            mesh.material = this.normal;
        }
    }
}
