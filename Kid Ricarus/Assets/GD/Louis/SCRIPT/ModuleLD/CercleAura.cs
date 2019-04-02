using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CercleAura : MonoBehaviour
{
    public AnimationCurve boostCurve;
    private Transform target; 
    public float baseForwardSpeed;
    public float curveTime = 4f;
    public float duration ;
    
    

    // Update is called once per frame 
    void FixedUpdate()
    {
        if (curveTime < duration)
        {
            applyBoostAura();
        }
    }

    public void applyBoostAura()
    {
        curveTime += Time.deltaTime;
        float curveAmount = boostCurve.Evaluate(curveTime) * baseForwardSpeed;
        ForcesDictionnaryScript.forcesDictionnaryScript.AddForce("Boost " + transform.name, target.forward * curveAmount); 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerControllerDjuloh>() != null)
        {     
         startBoostAura();
         target = other.transform;
            StartCoroutine(StopAcceleration());
        }
    }

    public void startBoostAura()
    {
        curveTime = 0f;
        Debug.Log("oui ca marche");
    }

    private IEnumerator StopAcceleration()
    {
        yield return new WaitForSeconds(duration + 0.1f);
        ForcesDictionnaryScript.forcesDictionnaryScript.RemoveForce("Boost " + transform.name);

    }


}