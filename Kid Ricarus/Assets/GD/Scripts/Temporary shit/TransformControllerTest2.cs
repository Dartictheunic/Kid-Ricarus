using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformControllerTest2 : MonoBehaviour
{
    public Transform player;

    public Vector3 offsetToAdd;
    Vector3 baseOffset;
    // Start is called before the first frame update
    void Awake()
    {
        baseOffset = player.position - transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.position + baseOffset;
    }
}