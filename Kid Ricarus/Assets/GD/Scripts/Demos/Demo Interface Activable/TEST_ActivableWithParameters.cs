using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST_ActivableWithParameters : MonoBehaviour, IActivatable
{
    public Color colorToDo;
    bool hasBeenActivated;

    public void ActivateItem()
    {
        if (!hasBeenActivated)
        {
            hasBeenActivated = true;
            DoColor(colorToDo);
        }
    }

    void DoColor(Color color)
    {
        GetComponent<MeshRenderer>().material.color = color;
    }
}
