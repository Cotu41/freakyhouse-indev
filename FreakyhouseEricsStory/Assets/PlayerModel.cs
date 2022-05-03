using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : MonoBehaviour
{
    Camera cam;
    public Transform tieTo;
    public Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(0, tieTo.rotation.eulerAngles.y, 0);
        transform.position = Player.player.transform.position + offset;

    }

    private void FixedUpdate()
    {

    }
}
