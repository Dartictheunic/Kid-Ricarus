﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraControllerTest2 : MonoBehaviour
{
    public AnimationCurve cameraElasticityBig;
    public AnimationCurve cameraElasticitySmall;
    public float bigCamSpeed;
    public float smallCamSpeed;
    public float verticalCamSpeed;
    public float accelerationCap;
    public Transform target;

    Vector3 baseOffset;
    Quaternion baseRotation;
    Camera cam;
    private Vector3 screenPoint;
    private Vector3 offset;

    public Transform O;
    public Transform X;
    public Transform Y;

    public float debugValueX;
    public float debugValueY;

    float baseDistance;

    public Text cameraText;

    bool updateCamera;

    void Start()
    {
        cam = GetComponent<Camera>();
        Vector3 angles = transform.eulerAngles;
        baseRotation = transform.rotation;
        updateCamera = true;
    }

    private void Update()
    {
        transform.LookAt(target);

        if (Input.GetMouseButton(0))
        {
            UpdateCamera(-debugValueX, -debugValueY);
        }

        else if (Input.GetMouseButton(0))
        {
            UpdateCamera(debugValueX, debugValueY);
        }

        else
        {
            UpdateCamera(0, 0);
        }
    }

    public void UpdateCamera(float xVelocity, float yVelocity)
    {
        if (updateCamera)
        {
            Vector3 objective = Vector3.zero;

            if (Mathf.Abs(xVelocity) > 5 && Vector3.Distance(X.position, transform.position) > accelerationCap) //La cam doit aller très vite
            {
                objective.x = Mathf.Lerp(O.position.x * Mathf.Sign(xVelocity * -1), X.position.x * Mathf.Sign(xVelocity * -1), xVelocity * Time.deltaTime * bigCamSpeed * cameraElasticityBig.Evaluate(Mathf.Abs(xVelocity)));
            }

            else if (Mathf.Abs(xVelocity) > 5 && Vector3.Distance(X.position, transform.position) <= accelerationCap) //La cam doit aller doucement
            {
                objective.x = Mathf.Lerp(O.position.x * Mathf.Sign(xVelocity * -1), X.position.x * Mathf.Sign(xVelocity * -1), xVelocity * Time.deltaTime * smallCamSpeed * cameraElasticityBig.Evaluate(Mathf.Abs(xVelocity)));
            }

            else //La cam doit rester près du player
            {
                objective.x = Mathf.Lerp(transform.position.x, O.position.x * Mathf.Sign(xVelocity * -1), xVelocity * Time.deltaTime * smallCamSpeed * cameraElasticitySmall.Evaluate(Mathf.Abs(xVelocity)));
            }

            if (Mathf.Abs(yVelocity) > 5 && Vector3.Distance(Y.position, transform.position) > accelerationCap)
            {
                objective.y = Mathf.Lerp(target.position.y * Mathf.Sign(yVelocity * -1), Y.position.y * Mathf.Sign(yVelocity * -1), yVelocity * Time.deltaTime * bigCamSpeed * cameraElasticityBig.Evaluate(Mathf.Abs(yVelocity)));
            }

            else if (Mathf.Abs(xVelocity) > 5 && Vector3.Distance(Y.position, transform.position) <= accelerationCap) 
            {
                objective.y = Mathf.Lerp(target.position.y * Mathf.Sign(yVelocity * -1), Y.position.y * Mathf.Sign(yVelocity * -1), yVelocity * Time.deltaTime * smallCamSpeed * cameraElasticityBig.Evaluate(Mathf.Abs(yVelocity)));
            }

            else
            {
                objective.y = Mathf.Lerp(transform.position.y, target.position.x * Mathf.Sign(yVelocity * -1), yVelocity * Time.deltaTime * smallCamSpeed * cameraElasticitySmall.Evaluate(Mathf.Abs(yVelocity)));
            }

            objective.z = baseDistance *-1;

            transform.position = objective;
        }
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360f)
            angle += 360f;
        if (angle > 360f)
            angle -= 360f;

        return Mathf.Clamp(angle, min, max);
    }

    public void ResetCameraPosition()
    {
        transform.position = baseOffset;
        transform.rotation = baseRotation;
    }

    void Awake()
    {
        baseOffset = target.position - transform.position;
        baseDistance = Vector3.Distance(target.position, transform.position);
    }
}
