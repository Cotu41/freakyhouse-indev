using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorMan : MonoBehaviour
{

    public float chance = 0.33f;
    public float time = 0.7f;

    public GameObject[] ghosts;
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
        foreach (GameObject g in ghosts)
            g.SetActive(true);

        StartCoroutine(KeepGhostsUntil());
    }

    IEnumerator KeepGhostsUntil()
    {
        yield return new WaitForSeconds(time);
        foreach (GameObject g in ghosts)
            g.SetActive(false);
    }
}
