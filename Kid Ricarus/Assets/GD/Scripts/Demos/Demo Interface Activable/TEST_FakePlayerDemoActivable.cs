using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST_FakePlayerDemoActivable : MonoBehaviour
{
    [SerializeField]
    Rigidbody mybody;

    void FixedUpdate()
    {
        mybody.velocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0, 0);
    }
}
