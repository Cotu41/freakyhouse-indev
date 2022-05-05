using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{

    public GameObject doors;

    public InspectionEvent OnEnterElevator;


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
        if(other.tag.Equals("Player"))
        {
            doors.GetComponent<Animator>().SetBool("IsClosed", true);
            OnEnterElevator?.Invoke();
        }
    }
}
