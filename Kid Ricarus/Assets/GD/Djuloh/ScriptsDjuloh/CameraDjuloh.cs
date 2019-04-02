using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraDjuloh : MonoBehaviour
{
    #region variables
    [Header("LES TRANSFORMS")]
    public Text camText;
    public Transform target;
    public Transform origin;
    public Transform xTransformLeft;
    public Transform xTransformRight;
    public Transform yTransformUp;
    public Transform yTransformDown;
    public Transform idleTransform;


    [Header("VARIABLES")]
    public Vector3 idlePos;
    Vector3 baseOffset;
    Quaternion baseRotation;
    Camera cam;
    bool updateCamera;   
    float newX;
    float newY;
    float newZ;

    [Header("ANIM")]
    public float camSpeed;
    public AnimationCurve UpCurve, DownCurve, LeftCurve, RightCurve, TopCurve, BottomCurve;

    #endregion


    void Start()
    {
        gameObject.transform.position = idlePos;
        gameObject.transform.rotation = new Quaternion(0,0,0,0);
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
                newX = Mathf.LerpUnclamped(origin.position.x, xTransformLeft.position.x, Mathf.Abs(xOffset));
                /*        if (gameObject.transform.position.x >= xTransformRight.position.x - 1)
                        {
                            newX = Mathf.Lerp(xTransformRight.position.x, xTransformLeft.position.x, Mathf.Abs(xOffset));

                        }
                        else
                        {
                            newX = Mathf.Lerp(origin.position.x, xTransformLeft.position.x, Mathf.Abs(xOffset));

                        }
                        */
            }

            else
            {
                newX = Mathf.LerpUnclamped(origin.position.x, xTransformRight.position.x, Mathf.Abs(xOffset));
              /*
                if (gameObject.transform.position.x <= xTransformLeft.position.x + 1)
                {
                    newX = Mathf.Lerp(xTransformLeft.position.x, xTransformRight.position.x, Mathf.Abs(xOffset));

                }
                else
                {
                    newX = Mathf.Lerp(origin.position.x, xTransformRight.position.x, Mathf.Abs(xOffset));

                }
                */
            }

            if (yOffset >= 0)
            {
                newY = Mathf.Lerp(origin.position.y, yTransformUp.position.y, Mathf.Abs(yOffset));
               
            }

            else
            {
                newY = Mathf.Lerp(origin.position.y, yTransformDown.position.y, Mathf.Abs(yOffset));
            }

           
            newZ = idleTransform.position.z;
            
            

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
        baseOffset = target.position - idlePos;
    }
}
