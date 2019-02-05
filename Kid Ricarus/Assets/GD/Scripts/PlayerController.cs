using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    #region Variables Feel
    [Header("Variables Feel")]
    [Header("Variables Déplacement")]
    [Tooltip("Vitesse à laquelle le joueur avance devant lui")]
    [Range(1f, 15f)]
    public float forwardSpeed;
    [Header("Variables rotation")]
    public float rotationSpeed;
    public float maxXRotation;
    public float maxZRotation;
    [Tooltip("Angle que l'on peut donner au téléphone au maximum")]
    public float maxInputTaken;

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
    #endregion

    Vector3 basePhoneAngle;
    Vector3 truePhoneDelta;
    float xAccelerationDelta;
    float zAccelerationDelta;
    Rigidbody playerBody;
    #endregion

    public IEnumerator CheckSmartphoneAngle()
    {
        yield return new WaitForSeconds(.1f);
        basePhoneAngle = Input.acceleration;
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

    public void Rotate()
    {
        truePhoneDelta = Input.acceleration - basePhoneAngle;
        xAccelerationDelta = -truePhoneDelta.x;
        zAccelerationDelta = (truePhoneDelta.y - truePhoneDelta.z) / 2;
        float newXRotation = Mathf.InverseLerp(-maxInputTaken, maxInputTaken, zAccelerationDelta);
        float newZRotation = Mathf.InverseLerp(-maxInputTaken, maxInputTaken, xAccelerationDelta);
        Vector3 mabite = new Vector3(Mathf.Lerp(-maxXRotation, maxXRotation, newXRotation), 0, Mathf.Lerp(-maxZRotation, maxZRotation, newZRotation));
        gyroRotationRate.text = "Vector 3 taken : " + mabite;
        transform.eulerAngles = mabite;
    }

    public void Move()
    {
        if (mustMoveForward)
        {
            playerBody.AddForce(transform.forward * forwardSpeed);
        }
    }

    #endregion

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            ResetAcceleration();
        }
    }

    #region Debug

    public void UpdateTextsDebug()
    {
        Acceleration.text = "Acceleration : " + Input.acceleration.ToString();
        gyroAttitude.text = "Gyro.attitude : " + Input.gyro.attitude.ToString();
        //gyroRotationRateUnbiaised.text = "Gyro.rotationRateUnbiaised : " + Input.gyro.rotationRateUnbiased.ToString();
        gyroUserAcceleration.text = "Gyro.userAcceleration : " + Input.gyro.userAcceleration.ToString();
    }

#endregion
    public enum PlayerState
    {
        grounded,
        flying
    }
}
