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
            actualPos = transform.position;

            if (xOffset >= 0)
            {
                
                Debug.Log("go gauche");
                newX = Mathf.Lerp(actualPos.x, xTransformLeft.position.x, Mathf.Abs(xOffset));

                SetCameraCurve(StandardCurve,0);
                //  updatePos = false;

            }

            else if (xOffset <= 0)
            {
                Debug.Log("go droite");
                newX = Mathf.Lerp(actualPos.x, xTransformRight.position.x, Mathf.Abs(xOffset));

                SetCameraCurve(StandardCurve,0);
                // updatePos = false;
            }

            if (yOffset >= 0)
            {
                Debug.Log("en haut");
                newY = Mathf.Lerp(actualPos.y, yTransformUp.position.y, Mathf.Abs(yOffset));

                SetCameraCurve(StandardCurve,0);
                // updatePos = false;
            }

            else
            {
                Debug.Log("en bas");
                newY = Mathf.Lerp(actualPos.y, yTransformDown.position.y, Mathf.Abs(yOffset));

                SetCameraCurve(StandardCurve,0);
                //  updatePos = false;
            }

      /*      if (player.actualPlayerState == PlayerControllerDjuloh.PlayerState.pique)
            {
               
                Debug.Log("piqué");
                newX = Mathf.LerpUnclamped(origin.position.x, topTransform.position.x, xOffset );
                newY = Mathf.LerpUnclamped(yTransformUp.position.y, topTransform.position.y, yOffset);
                newZ = Mathf.LerpUnclamped(yTransformUp.position.z, topTransform.position.z, yOffset);
                currentCurve = TopCurve;
                //  updatePos = false;

            }
            */
            newZ = origin.position.z;

            animTimePos += Time.deltaTime *0.1f;
            
            Vector3 toPosition = new Vector3(newX, newY, newZ);
           
            //Debug.Log("From position : " + transform.position +  "To Position + " + toPosition + "Delta :" + Time.deltaTime * camSpeed);
            transform.position = Vector3.Lerp(transform.position, toPosition, currentCurve.Evaluate(animTimePos)* camSpeed * Vector3.Distance(transform.position, toPosition));
            Debug.Log(currentCurve.Evaluate(animTimePos));
            transform.LookAt(target);
        }
    }

    public void SetCameraCurve(AnimationCurve curve, float timeCurve)
    {
        animTimePos = timeCurve;
        currentCurve = curve;
       

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
