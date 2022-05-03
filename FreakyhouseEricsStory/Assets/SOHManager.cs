using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SOHManager : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject[] states;

    public int currentIndex = 0;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            nextState();
        }
    }

    public void nextState()
    {

        states[currentIndex].SetActive(false);

        if (currentIndex < states.Length-1)
        {
            currentIndex++;
        }
        else
        {
            currentIndex = 0;
        }

        states[currentIndex].SetActive(true);

    }

    public void setState(int index)
    {
        if (index > states.Length - 1 || index == currentIndex || currentIndex < 0)
            return;

        states[currentIndex].SetActive(false);
        currentIndex = index;
        states[currentIndex].SetActive(true);
    }
}
