using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActivator : MonoBehaviour
{
    public bool activated;
    public float activationRadius;
    [Range(0f,1f)]
    public float activationSpeed;

    Vector3 activationVector;

    private void FixedUpdate()
    {
        if (activated && transform.localScale.x < activationRadius)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, activationVector, activationSpeed);
        }

        if(!activated && transform.localScale.x > .01f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, activationSpeed);
        }
    }

    private void Start()
    {
        activationVector = new Vector3(activationRadius, activationRadius, activationRadius);
    }

    public void Activate()
    {
        activated = true;
    }

    public void DeActivate()
    {
        activated = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<IActivatable>() != null)
        {
            Debug.Log("Activation !");
            other.GetComponent<IActivatable>().ActivateItem();
        }
    }
}
