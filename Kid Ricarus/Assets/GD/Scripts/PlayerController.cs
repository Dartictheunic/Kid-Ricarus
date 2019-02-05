using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    #region Variables Feel
    [Header("Variables Déplacement")]
    [Header("Variables Feel")]
    [Tooltip("Vitesse à laquelle le joueur avance devant lui")]
    [Range(1f, 15f)]
    public float forwardSpeed;
    [Tooltip("Vitesse maximale du joueur")]
    public float maxForwardSpeed;
    [Header("Variables rotation")]
    public float rotationSpeed;
    public float maxXRotation;
    public float maxYRotation;
    [Tooltip("Angle que l'on peut donner au téléphone au maximum")]
    public float maxInputTaken;

    #endregion


    #region liens à faire
    [Header("Liens à faire")]
    [Space(30)]
    [Tooltip("Le truc qui sert à fake la rotation, plus représentatif du gyro du téléphone")]
    public Turner turner;
    public CameraController cam;
    #endregion

    #region variables Prog
    [Header("Variables pour la prog, mainly")]
    [Tooltip("L'état actuel du joueur, public pour simplifier le débug")]
    public PlayerState actualPlayerState = PlayerState.flying;
    [Tooltip("Le joueur doit-il avancer ?")]
    public bool mustMoveForward = true;

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
    float zAccelerationDelta;
    Rigidbody playerBody;
    #endregion

    public IEnumerator CheckSmartphoneAngle()
    {
        yield return new WaitForSeconds(.1f);
        basePhoneAngle = Input.acceleration;
    }

    public void SetRotationSpeed(float newSpeed)
    {
        rotationSpeed = newSpeed;
        rotationSpeedText.text = rotationSpeed.ToString();
    }

    private void Start()
    {
        Input.gyro.enabled = true;
        playerBody = GetComponent<Rigidbody>();
        StartCoroutine(CheckSmartphoneAngle());
    }

    public void ResetAcceleration()
    {
        basePhoneAngle = Input.acceleration;
        transform.rotation = Quaternion.identity;
        targetRotation = Vector3.zero;
        transform.position = Vector3.zero;
    }

    private void FixedUpdate()
    {
        Move();
        Rotate();

#if !UNITY_EDITOR
        UpdateTextsDebug();
#endif

    }

#region Gestion Deplacement

    public Vector3 GetPhoneRotations()
    {
        truePhoneDelta = Input.acceleration - basePhoneAngle;
        xAccelerationDelta = -truePhoneDelta.x;
        zAccelerationDelta = (truePhoneDelta.y - truePhoneDelta.z) / 2;

        float newXRotation = Mathf.InverseLerp(-maxInputTaken, maxInputTaken,targetRotation.x);
        float newYRotation = Mathf.InverseLerp(-maxInputTaken, maxInputTaken,targetRotation.z);

        newXRotation = Mathf.InverseLerp(-maxInputTaken, maxInputTaken, zAccelerationDelta);

        newYRotation = Mathf.InverseLerp(-maxInputTaken, maxInputTaken, xAccelerationDelta);

        Vector3 calculatedVector = new Vector3(Mathf.Lerp(-maxXRotation, maxXRotation, newXRotation), Mathf.Lerp(-maxYRotation, maxYRotation, newYRotation), 0);
        calculatedVector.y *= -1;
        return calculatedVector;
    }

    public void Rotate()
    {
        Vector3 phoneRotations = GetPhoneRotations();
        gyroRotationRate.text = "Vector 3 taken : " + phoneRotations;

        targetRotation = phoneRotations;
        transform.eulerAngles = Vector3.Lerp(myEulerAngles, targetRotation, rotationSpeed);
        RotateTurner();
        cam.UpdateCamera(myEulerAngles.x, myEulerAngles.y);
        gyroAttitude.text = "MY eulerAngles :  " + myEulerAngles;
        gyroRotationRateUnbiaised.text = "Character eulerAngles : " + transform.eulerAngles;
    }

    public void RotateTurner()
    {
        Vector3 turnerRotation = new Vector3(targetRotation.x, -targetRotation.z*4, 0);
        turner.UpdateTurnerRotation(turnerRotation);
    }

    public void Move()
    {
        if (mustMoveForward)
        {
            transform.Translate(transform.forward * forwardSpeed * 0.01f);
        }
    }

    #endregion

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            ResetAcceleration();
        }

        myEulerAngles = transform.eulerAngles;
        if (myEulerAngles.x > 180)
        {
            myEulerAngles.x -= 360;
        }
        if (myEulerAngles.y > 180)
        {
            myEulerAngles.y -= 360;
        }
        if (myEulerAngles.z > 180)
        {
            myEulerAngles.z -= 360;
        }
    }

    #region Debug

    public void UpdateTextsDebug()
    {
        gyroUserAcceleration.text = "Turner rotation " + turner.transform.eulerAngles;
        Acceleration.text = "Acceleration : " + Input.acceleration.ToString();
    }

#endregion

    public enum PlayerState
    {
        grounded,
        flying
    }
}
