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
    public Transform topTransform, bottomTransform, farLeftTransform, farRightTransform;


    [Header("VARIABLES")]
    public Vector3 idlePos, actualPos;
    public PlayerControllerDjuloh player;
    private bool updatePos;
    private float animTimePos;
    private Transform currentTransform;
    Vector3 baseOffset;
    Quaternion baseRotation;
    Camera cam;
    bool updateCamera;   
    float newX;
    float newY;
    float newZ;

    [Header("ANIM")]
    public float camSpeed;
    public AnimationCurve StandardCurve, TopCurve, BottomCurve;
    private AnimationCurve currentCurve;
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
                Debug.Log("go gauche");
                newX = Mathf.Lerp(actualPos.x, xTransformLeft.position.x, Mathf.Abs(xOffset));
              
                currentCurve = StandardCurve;
                if (gameObject.transform.position.x <= xTransformLeft.position.x - 0.5 || gameObject.transform.position.x >= xTransformLeft.position.z + 0.5)
                {

                    updatePos = true;
                }
            }

            else
            {
                Debug.Log("go droite");
                newX = Mathf.Lerp(actualPos.x, xTransformRight.position.x, Mathf.Abs(xOffset));
             
                currentCurve = StandardCurve;
                if (gameObject.transform.position.x <= xTransformRight.position.x - 0.5 || gameObject.transform.position.x >= xTransformRight.position.z + 0.5)
                {

                    updatePos = true;
                }
            }

            if (yOffset >= 0)
            {
                Debug.Log("en haut");
                newY = Mathf.Lerp(actualPos.y, yTransformUp.position.y, Mathf.Abs(yOffset));
                newZ = idleTransform.position.z;
                currentCurve = StandardCurve;
                if (gameObject.transform.position.y <= yTransformUp.position.y - 0.5 || gameObject.transform.position.y >= yTransformUp.position.z + 0.5)
                {

                    updatePos = true;
                }
            }

            else
            {
                Debug.Log("en bas");
                newY = Mathf.Lerp(actualPos.y, yTransformDown.position.y, Mathf.Abs(yOffset));
                newZ = idleTransform.position.z;
                currentCurve = StandardCurve;
                if (gameObject.transform.position.y <= yTransformDown.position.y - 0.5 || gameObject.transform.position.y >= yTransformDown.position.z + 0.5)
                {

                    updatePos = true;
                }
            }

            if (player.actualPlayerState == PlayerControllerDjuloh.PlayerState.pique)
            {
               
                Debug.Log("piqué");
                newX = Mathf.LerpUnclamped(origin.position.x, topTransform.position.x, xOffset );
                newY = Mathf.LerpUnclamped(yTransformUp.position.y, topTransform.position.y, yOffset);
                newZ = Mathf.LerpUnclamped(yTransformUp.position.z, topTransform.position.z, yOffset);
                currentCurve = TopCurve;

                if(gameObject.transform.position.z <= topTransform.position.z - 1 || gameObject.transform.position.z >= topTransform.position.z + 1)
                {

                    updatePos = true;
                }
            }


            animTimePos += Time.deltaTime *0.1f;
            
            Vector3 toPosition = new Vector3(newX, newY, newZ);
            transform.position = toPosition;
            //Debug.Log("From position : " + transform.position +  "To Position + " + toPosition + "Delta :" + Time.deltaTime * camSpeed);
            //transform.position = Vector3.Lerp(transform.position, toPosition, currentCurve.Evaluate(animTimePos)* Vector3.Distance(transform.position, toPosition));
            Debug.Log(currentCurve.Evaluate(animTimePos));
            transform.LookAt(target);
        }
    }

    public void SetCamPos(){
        if(updatePos == true)
        {
            animTimePos = 0;
            actualPos = gameObject.transform.position;
            updatePos = false;
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
