using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turner : MonoBehaviour
{
    public void UpdateTurnerRotation(Vector3 newEulers)
    {
        transform.eulerAngles = newEulers;
    }
}
