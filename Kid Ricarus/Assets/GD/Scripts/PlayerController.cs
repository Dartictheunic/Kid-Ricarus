using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class PlayerController : MonoBehaviour
{
    #region Variables Feel
    [Header("Variables Déplacement")]
    [Header("Variables Feel")]
    [Tooltip("Vitesse à laquelle le joueur avance devant lui")]
    [Range(.0001f, 1f)]
    public float forwardSpeed;
    [Tooltip("Vitesse maximale du joueur")]
    public float maxForwardSpeed;
    [Tooltip("Puissance de la gravité sur le joueur")]
    public float gravity;
    [Header("Variables rotation")]
    public float rotationSpeed;
    public float maxXRotation;
    public float maxYRotation;
    [Tooltip("Angle que l'on peut donner au téléphone au maximum")]
    public float maxInputTaken;
    [Tooltip("Temps où le joueur peut incliner le téléphone vers le haut de base")]
    public float essenceDeSecours;
    [Tooltip("Fenetre pendant laquelle le brackage est checké")]
    public float timeForBraquage;


    [Header("Variables pour l'éditeur")]
    [Tooltip("A quel point la souris transmet d'input")]
    public float mouseScale;
    #endregion


    #region liens à faire
    [Header("Liens à faire")]
    [Space(30)]
    [Tooltip("La caméra dans la scène")]
    public CameraController cam;
    #endregion

    #region variables Prog
    [Header("Variables pour la prog, mainly")]
    [Tooltip("L'état actuel du joueur, public pour simplifier le débug")]
    public PlayerState actualPlayerState = PlayerState.flying;
    [Tooltip("Le joueur doit-il avancer ?")]
    public bool mustMoveForward = true;
    [Tooltip("Energie accumulée en piqué")]
    public float energieAccumuleePique;
    [Tooltip("Multiplicateur d'énergie du piqué")]
    [Range(0f,1f)]
    public float piqueMultiplicator;

    #region debug text
    [Header("Debug ou temporaire")]
    public Text Acceleration;
    public Text gyroAttitude;
    public Text gyroRotationRate;
    public Text gyroRotationRateUnbiaised;
    public Text gyroUserAcceleration;
    public Text rotationSpeedText;
    #endregion

    Vector3 basePhoneAngle;
    Vector3 truePhoneDelta;
    Vector3 targetRotation;
    Vector3 myEulerAngles;
    float xAccelerationDelta;
    float lastXRotation;
    float lastYPosition;
    float timeBeforeResetRotation;
    float zAccelerationDelta;
    Rigidbody playerBody;
    float timeSpentInPique;

    #endregion

    public IEnumerator CheckSmartphoneAngle()
    {
        yield return new WaitForSeconds(.1f);
        basePhoneAngle = Input.acceleration;
    }

    public void SetRotationSpeed(float newSpeed)
    {
        rotationSpeed = newSpeed;
    }

    private void Start()
    {
        Input.gyro.enabled = true;
        playerBody = GetComponent<Rigidbody>();
        StartCoroutine(CheckSmartphoneAngle());
        ForcesDictionnaryScript.forcesDictionnaryScript.AddForce("Gravity", new Vector3(0,gravity, 0));
    }

    public void ChangePlayerGravity(float newGravityMultiplier)
    {
        ForcesDictionnaryScript.forcesDictionnaryScript.AddForce("Gravity", new Vector3(0,newGravityMultiplier, 0));
    }

    public void ResetPlayerGravity()
    {
        ForcesDictionnaryScript.forcesDictionnaryScript.AddForce("Gravity", new Vector3(0,gravity, 0));
    }

    public void ResetPlayer()
    {
        basePhoneAngle = Input.acceleration;
        transform.rotation = Quaternion.identity;
        targetRotation = Vector3.zero;
        transform.position = Vector3.zero;
        cam.ResetCameraPosition();
    }

    private void FixedUpdate()
    {
        Move();

#if !UNITY_EDITOR
        if(actualPlayerState == PlayerState.flying)
        {
            Rotate();
        }
#endif

#if UNITY_EDITOR
        EditorControls();

        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetPlayer();
        }

        if (Input.GetKey(KeyCode.E))
        {
            forwardSpeed += Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.A))
        {
            forwardSpeed -= Time.deltaTime;
        }

#endif
    }



    #region Gestion Deplacement
    public IEnumerator TemporaryReturnOfDrop(float DropTime)
    {
        yield return new WaitForSeconds(DropTime);
        mustMoveForward = true;
        ForcesDictionnaryScript.forcesDictionnaryScript.RemoveForce("Drop");
        actualPlayerState = PlayerState.pique;
        lastYPosition = transform.position.y;
    }

    public IEnumerator TemporaryEndAscend(float AscendTime)
    {
        yield return new WaitForSeconds(AscendTime);
        mustMoveForward = true;
        actualPlayerState = PlayerState.flying;
    }

    public Vector3 GetPhoneRotations()
    {
        // Store la Old X rotation, faire le delta entre les 2 et si le mathf.abs du delta est supérieur à X et que la valeur de new X > .8 on braque
        truePhoneDelta = Input.acceleration - basePhoneAngle;
        xAccelerationDelta = -truePhoneDelta.x;
        zAccelerationDelta = (truePhoneDelta.y - truePhoneDelta.z) / 2;
        gyroRotationRateUnbiaised.text = "XRotation :" + zAccelerationDelta;

        if(timeBeforeResetRotation > timeForBraquage)
        {
            if(Mathf.Abs(lastXRotation - zAccelerationDelta) > .45f)
            {
                rotationSpeedText.text = "BRAK";
                if(actualPlayerState == PlayerState.flying)
                {
                    mustMoveForward = false;
                    ForcesDictionnaryScript.forcesDictionnaryScript.AddForce("Drop", transform.forward * forwardSpeed / 10);
                }

                else if (actualPlayerState == PlayerState.pique && zAccelerationDelta <.1f)
                {
                    ForcesDictionnaryScript.forcesDictionnaryScript.AddForce("FinPique", new Vector3(0f,energieAccumuleePique/timeSpentInPique,0f), timeSpentInPique, false);
                    actualPlayerState = PlayerState.forceAscend;
                }
            }

            else
            {
                rotationSpeedText.text = "pas brak";
            }

            timeBeforeResetRotation = 0f;
            lastXRotation = zAccelerationDelta;
        }

        else
        {
            timeBeforeResetRotation += Time.deltaTime;
        }

        float newXRotation = Mathf.InverseLerp(-maxInputTaken, maxInputTaken,targetRotation.x);
        float newYRotation = Mathf.InverseLerp(-maxInputTaken, maxInputTaken,targetRotation.z);

        newXRotation = Mathf.InverseLerp(-maxInputTaken, maxInputTaken, zAccelerationDelta);

        newYRotation = Mathf.InverseLerp(-maxInputTaken, maxInputTaken, xAccelerationDelta);
        gyroRotationRate.text = "New XRotation :" + newXRotation;
        gyroAttitude.text = "newYRotation :  " + newYRotation;

        Vector3 calculatedVector = new Vector3(Mathf.Lerp(-maxXRotation, maxXRotation, newXRotation), Mathf.Lerp(-maxYRotation, maxYRotation, newYRotation), 0);
        calculatedVector.y *= -1;
        return calculatedVector;
    }

    public void Rotate()
    {
        Vector3 phoneRotations = GetPhoneRotations();

        if (actualPlayerState == PlayerState.flying)
        {

    #pragma warning disable CS0618 // Type or member is obsolete
            transform.RotateAround(Vector3.up, phoneRotations.y * Time.deltaTime * rotationSpeed);
    #pragma warning restore CS0618 // Type or member is obsolete


            transform.eulerAngles = new Vector3(Mathf.Lerp(myEulerAngles.x, phoneRotations.x, rotationSpeed), transform.eulerAngles.y, transform.eulerAngles.z);
            cam.UpdateCamera(Mathf.InverseLerp(0, maxYRotation, Mathf.Abs(phoneRotations.y)) * Mathf.Sign(phoneRotations.y), Mathf.InverseLerp(0, maxXRotation, Mathf.Abs(phoneRotations.x)) * Mathf.Sign(phoneRotations.x));
        }

        else if(actualPlayerState == PlayerState.pique)
        {

            if(phoneRotations.y > .7f)
            {
                phoneRotations.x = phoneRotations.x / 2;
            }

            if(phoneRotations.y < .1f)
            {
                actualPlayerState = PlayerState.flying;
            }

            else
            {
                energieAccumuleePique += (lastYPosition - transform.position.y) * piqueMultiplicator;
            }

            timeSpentInPique += Time.deltaTime;
            lastYPosition = transform.position.y;
        }
    }

    public void Move()
    {
        if (mustMoveForward)
        {
            playerBody.MovePosition(transform.position + forwardSpeed * transform.forward + ForcesDictionnaryScript.forcesDictionnaryScript.ReturnAllForces());
        }

        else
        {
            playerBody.MovePosition(transform.position + ForcesDictionnaryScript.forcesDictionnaryScript.ReturnAllForces());
        }
    }

    #endregion

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            ResetPlayer();
        }
        myEulerAngles = returnGoodEulers(transform.eulerAngles);
    }

    public Vector3 returnGoodEulers(Vector3 vectorToReturn)
    {
        if (vectorToReturn.x > 180)
        {
            vectorToReturn.x -= 360;
        }
        if (vectorToReturn.y > 180)
        {
            vectorToReturn.y -= 360;
        }
        if (vectorToReturn.z > 180)
        {
            vectorToReturn.z -= 360;
        }

        return vectorToReturn;
    }

    #region Debug

    public void EditorControls()
    {
        Vector3 mouseRotation = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        mouseRotation -= new Vector3(.5f, .5f);
        mouseRotation *= mouseScale;
        targetRotation = new Vector3(-mouseRotation.y, mouseRotation.x);

#pragma warning disable CS0618 // Type or member is obsolete
        transform.RotateAround(Vector3.up, targetRotation.y * Time.deltaTime * Time.deltaTime);
#pragma warning restore CS0618 // Type or member is obsolete


        transform.eulerAngles = new Vector3(Mathf.Lerp(myEulerAngles.x, targetRotation.x, rotationSpeed), transform.eulerAngles.y, transform.eulerAngles.z);
        cam.UpdateCamera(Mathf.InverseLerp(0, maxYRotation, Mathf.Abs(targetRotation.y)) * Mathf.Sign(targetRotation.y), Mathf.InverseLerp(0, maxXRotation, Mathf.Abs(targetRotation.x)) * Mathf.Sign(targetRotation.x));
    }

    #endregion

    public enum PlayerState
    {
        grounded,
        flying,
        interacting,
        pique,
        forceAscend
    }
}