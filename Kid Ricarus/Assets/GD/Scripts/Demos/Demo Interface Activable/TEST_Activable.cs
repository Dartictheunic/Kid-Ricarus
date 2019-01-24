using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST_Activable : MonoBehaviour, IActivatable //Mettre l'interface ici force à avoir un void de l'interface
{
    bool hasBeenActivated;

    public void ActivateItem()
    {
        if (!hasBeenActivated)
        {
            hasBeenActivated = true;
            GetComponent<MeshRenderer>().material.color = Color.green;
        }
    }
}
