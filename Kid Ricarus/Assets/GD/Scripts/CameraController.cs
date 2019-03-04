using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    public Text camText;
    public float camSpeed;
    public Transform target;
    public Transform origin;
    public Transform xTransformLeft;
    public Transform xTransformRight;
    public Transform yTransformUp;
    public Transform yTransformDown;

    Vector3 baseOffset;
    Quaternion baseRotation;
    Camera cam;
    bool updateCamera;

    float newX;
    float newY;
    float newZ;

    void Start()
    {
        cam = GetComponent<Camera>();
        Vector3 angles = transform.eulerAngles;
        baseRotation = transform.rotation;
        updateCamera = true;
    }

    public void UpdateCamera(float xOffset, float yOffset)
    {
        if (updateCamera)
        {
            camText.text = xOffset.ToString() + "x   " + yOffset.ToString() + "y   ";
            Vector3 actualPosition = transform.position;

            if (xOffset >= 0)
            {
                newX = Mathf.Lerp(origin.position.x, xTransformLeft.position.x, Mathf.Abs(xOffset));
            }

            else
            {
                newX = Mathf.Lerp(origin.position.x, xTransformRight.position.x, Mathf.Abs(xOffset));
            }

            if (yOffset >= 0)
            {
                newY = Mathf.Lerp(origin.position.y, yTransformUp.position.y, Mathf.Abs(yOffset));
            }

            else
            {
                newY = Mathf.Lerp(origin.position.y, yTransformDown.position.y, Mathf.Abs(yOffset));
            }
            
            newZ = origin.position.z;

            Debug.Log("xOffset : " + xOffset + "   yOffset" + yOffset);
            Vector3 toPosition = new Vector3(newX, newY, newZ);
            //Debug.Log("From position : " + transform.position +  "To Position + " + toPosition + "Delta :" + Time.deltaTime * camSpeed);
            transform.position = Vector3.Lerp(transform.position, toPosition, Time.deltaTime * camSpeed * Vector3.Distance(transform.position, toPosition));

            transform.LookAt(target);
        }
    }

    public void ResetCameraPosition()
    {
        transform.position = baseOffset;
        transform.rotation = baseRotation;
    }

    void Awake()
    {
        baseOffset = target.position - transform.position;
    }

}