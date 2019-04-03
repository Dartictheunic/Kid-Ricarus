using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CourantAerien : MonoBehaviour
{
    public GameObject entree, sortie;
    public float speedBoost; 
    private Vector3 direction;


    private void Start()
    {
        direction = (sortie.transform.position - entree.transform.position).normalized;
        Debug.Log(direction);
    }



    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerControllerDjuloh>() != null)
        {
            ForcesDictionnaryScript.forcesDictionnaryScript.AddForce ("Courant" + transform.parent.name , direction * speedBoost); 
            print("ok");
        }
        else
        {
            print("nope");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerControllerDjuloh>() !=null)
        {
            ForcesDictionnaryScript.forcesDictionnaryScript.RemoveForce("Courant" + transform.parent.name);
        }
    }

}
