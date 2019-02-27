using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aura : MonoBehaviour
{
    public PlayerController playerScript;

    public AnimationCurve boostCurve;
    public float multiplier;

    float baseForwardSpeed;
    float curveTime = 4f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (curveTime < 3.0f)
        {
            applyBoostAura();
        }
    }

    public void applyBoostAura()
    {
        curveTime += Time.deltaTime;
        float curveAmount = boostCurve.Evaluate(curveTime) * baseForwardSpeed;
        playerScript.forwardSpeed = curveAmount;
        Debug.Log(playerScript.forwardSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        startBoostAura();
    }

    public void startBoostAura()
    {
        baseForwardSpeed = playerScript.forwardSpeed;
        curveTime = 0f;
        Debug.Log("oui ca marche");
    }
}
