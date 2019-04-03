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
    [Tooltip("Multiplicateur d'énergie du piqué")]
    [Range(0f, 1f)]
    public float piqueMultiplicator;


    [Header("Variables pour l'éditeur")]
    [Tooltip("A quel point la souris transmet d'input")]
    public float mouseScale;
    #endregion


    #region liens à faire
    [Header("Liens à faire")]
    [Space(30)]
    [Tooltip("La caméra dans la scène")]
    public CameraController cam;
    [Tooltip("Le trigger en enfant du player (souvent une sphère)")]
    public PlayerActivator trigger;
    #endregion

    #region variables Prog
    [Header("Variables pour la prog, mainly")]
    [Tooltip("L'état actuel du joueur, public pour simplifier le débug")]
    public PlayerState actualPlayerState = PlayerState.flying;
    [Tooltip("Le joueur doit-il avancer ?")]
    public bool mustMoveForward = true;
    [Tooltip("Energie accumulée en piqué")]
    public float energieAccumuleePique;
    [Tooltip("Le vecteur 3 appliqué au joueur à la frame précédente")]
    public Vector3 playerLastMovement;

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
    bool invertHorizontal;
    bool invertVertical;
    float yDelta;
    float timeBeforeResetRotation;
    float zAccelerationDelta;
    float timeSpentInPique;
    float actualEssence;
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
    }

    private void Start()
    {
        Input.gyro.enabled = true;
        playerBody = GetComponent<Rigidbody>();
        StartCoroutine(CheckSmartphoneAngle());
        ForcesDictionnaryScript.forcesDictionnaryScript.AddForce("Gravity", new Vector3(0, gravity, 0));
        actualEssence = essenceDeSecours;
    }

    public void ChangePlayerGravity(float newGravityMultiplier)
    {
        ForcesDictionnaryScript.forcesDictionnaryScript.AddForce("Gravity", new Vector3(0, newGravityMultiplier, 0));
    }

    public void ResetPlayerGravity()
    {
        ForcesDictionnaryScript.forcesDictionnaryScript.AddForce("Gravity", new Vector3(0, gravity, 0));
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
        yDelta = lastYPosition - transform.position.y;
        Move();

#if !UNITY_EDITOR
        UpdatePlayer();
#endif

#if UNITY_EDITOR
        EditorControls();
#endif

        gyroUserAcceleration.text = actualPlayerState.ToString();

        lastYPosition = transform.position.y;
    }



    #region Gestion Deplacement
    public IEnumerator TemporaryReturnOfDrop(float DropTime)
    {
        float oldSpeed = cam.camSpeed;
        cam.camSpeed = 0f;
        yield return new WaitForSeconds(DropTime);
        mustMoveForward = true;
        cam.camSpeed = oldSpeed;
        ForcesDictionnaryScript.forcesDictionnaryScript.RemoveForce("Drop");
        actualPlayerState = PlayerState.pique;
    }

    public IEnumerator TemporaryEndAscend(float AscendTime)
    {
        yield return new WaitForSeconds(AscendTime);
        mustMoveForward = true;
        actualPlayerState = PlayerState.flying;
    }

    public void Drop()
    {
        actualPlayerState = PlayerState.drop;
        mustMoveForward = false;
        ForcesDictionnaryScript.forcesDictionnaryScript.AddForce("Drop", transform.forward * forwardSpeed / 100);
        transform.eulerAngles = new Vector3(Mathf.Lerp(myEulerAngles.x, maxXRotation, rotationSpeed), transform.eulerAngles.y, transform.eulerAngles.z);
        timeSpentInPique += Time.deltaTime;
        // AJOUTER UNE LIGNE POUR RAMENER LA CAMERA EN POSITION AU DESSUS DU JOUEUR
        StartCoroutine(TemporaryReturnOfDrop(1.5f));
    }

    public void UpdatePlayer()
    {
        Vector3 phoneRotations = GetPhoneRotations();
        gyroRotationRate.text = "phonerotations :" + phoneRotations;

        if (actualPlayerState == PlayerState.flying)
        {
            if (phoneRotations.x < -5f)
            {
                if (energieAccumuleePique > 0f)
                {
                    energieAccumuleePique += yDelta;
                }

                else if(actualEssence > 0f)
                {
                    actualEssence += yDelta;
                    Mathf.Clamp(actualEssence, -.2f, essenceDeSecours);
                }

                else
                {
                    Drop();
                    return;
                }
            }

            else
            {
                if(actualEssence < essenceDeSecours)
                {
                    Mathf.Clamp(actualEssence, -.2f, essenceDeSecours);
                }
            }

#pragma warning disable CS0618 // Type or member is obsolete
            transform.RotateAround(Vector3.up, phoneRotations.y * Time.deltaTime * rotationSpeed);
#pragma warning restore CS0618 // Type or member is obsolete

            transform.eulerAngles = new Vector3(Mathf.Lerp(myEulerAngles.x, phoneRotations.x, rotationSpeed), transform.eulerAngles.y, transform.eulerAngles.z);
            cam.UpdateCamera(Mathf.InverseLerp(0, maxYRotation, Mathf.Abs(phoneRotations.y)) * Mathf.Sign(phoneRotations.y), Mathf.InverseLerp(0, maxXRotation, Mathf.Abs(phoneRotations.x)) * Mathf.Sign(phoneRotations.x));

            if (phoneRotations.x > 15)
            {
                actualPlayerState = PlayerState.pique;
            }

        }

        else if (actualPlayerState == PlayerState.pique)
        {

            if (phoneRotations.x < 15f)
            {
                phoneRotations.y = phoneRotations.y / 2;

                if (timeSpentInPique > 1f)
                {
                    actualPlayerState = PlayerState.flying;
                }

                else
                {
                    energieAccumuleePique += yDelta * piqueMultiplicator;
                }
            }

            else
            {
                energieAccumuleePique += yDelta * piqueMultiplicator;
            }

#pragma warning disable CS0618 // Type or member is obsolete
            transform.RotateAround(Vector3.up, phoneRotations.y * Time.deltaTime * rotationSpeed);
#pragma warning restore CS0618 // Type or member is obsolete

            transform.eulerAngles = new Vector3(Mathf.Lerp(myEulerAngles.x, maxXRotation, rotationSpeed), transform.eulerAngles.y, transform.eulerAngles.z);
            timeSpentInPique += Time.deltaTime;
            cam.UpdateCamera(Mathf.InverseLerp(0, maxYRotation, Mathf.Abs(phoneRotations.y)) * Mathf.Sign(phoneRotations.y), Mathf.InverseLerp(0, maxXRotation, Mathf.Abs(phoneRotations.x)) * Mathf.Sign(phoneRotations.x));

        }

        else if (actualPlayerState == PlayerState.forceAscend)
        {
            transform.eulerAngles = new Vector3(Mathf.Lerp(myEulerAngles.x, -maxXRotation, rotationSpeed), transform.eulerAngles.y, transform.eulerAngles.z);
            // METTRE DU CODE POUR METTRE LA CAMERA SOUS LE JOUEUR                 
            cam.UpdateCamera(Mathf.InverseLerp(0, maxYRotation, Mathf.Abs(phoneRotations.y)) * Mathf.Sign(phoneRotations.y), Mathf.InverseLerp(0, maxXRotation, Mathf.Abs(phoneRotations.x)) * Mathf.Sign(phoneRotations.x));

            return;
        }

        else if (actualPlayerState == PlayerState.drop)
        {
            transform.eulerAngles = new Vector3(Mathf.Lerp(myEulerAngles.x, maxXRotation, rotationSpeed), transform.eulerAngles.y, transform.eulerAngles.z);
            // METTRE DU CODE POUR METTRE LA CAMERA AU DESSUS LE JOUEUR                 
            cam.UpdateCamera(Mathf.InverseLerp(0, maxYRotation, Mathf.Abs(phoneRotations.y)) * Mathf.Sign(phoneRotations.y), Mathf.InverseLerp(0, maxXRotation, Mathf.Abs(phoneRotations.x)) * Mathf.Sign(phoneRotations.x));

        }

        gyroRotationRateUnbiaised.text = "Essence : " + actualEssence.ToString();
        gyroAttitude.text = "Energie accumulée : " + energieAccumuleePique;
    }

    public void Move()
    {
        if (mustMoveForward)
        {
            playerLastMovement = forwardSpeed * transform.forward + ForcesDictionnaryScript.forcesDictionnaryScript.ReturnAllForces(); 
            playerBody.MovePosition(transform.position + playerLastMovement);
        }

        else
        {
            playerLastMovement = forwardSpeed * transform.forward + ForcesDictionnaryScript.forcesDictionnaryScript.ReturnAllForces();
            playerBody.MovePosition(transform.position + ForcesDictionnaryScript.forcesDictionnaryScript.ReturnAllForces());
        }
    }


    public Vector3 GetPhoneRotations()
    {
        truePhoneDelta = Input.acceleration - basePhoneAngle;
        xAccelerationDelta = -truePhoneDelta.x;
        zAccelerationDelta = (truePhoneDelta.y - truePhoneDelta.z) / 2;

        if (timeBeforeResetRotation > timeForBraquage)
        {
            if (Mathf.Abs(lastXRotation - zAccelerationDelta) > .35f)
            {
                rotationSpeedText.text = "BRAK";
                if (actualPlayerState == PlayerState.flying)
                {
                    Drop();
                }

                else if (actualPlayerState == PlayerState.pique && zAccelerationDelta < .1f)
                {
                    ForcesDictionnaryScript.forcesDictionnaryScript.AddForce("FinPique", new Vector3(0f, energieAccumuleePique / timeSpentInPique, 0f), timeSpentInPique, false);
                    actualPlayerState = PlayerState.forceAscend;
                    StartCoroutine(TemporaryEndAscend(timeSpentInPique));
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

        float newXRotation = Mathf.InverseLerp(-maxInputTaken, maxInputTaken, targetRotation.x);
        float newYRotation = Mathf.InverseLerp(-maxInputTaken, maxInputTaken, targetRotation.z);

        newXRotation = Mathf.InverseLerp(-maxInputTaken, maxInputTaken, zAccelerationDelta);

        newYRotation = Mathf.InverseLerp(-maxInputTaken, maxInputTaken, xAccelerationDelta);


        Vector3 calculatedVector = new Vector3(Mathf.Lerp(-maxXRotation, maxXRotation, newXRotation), Mathf.Lerp(-maxYRotation, maxYRotation, newYRotation), 0);
        
        if(!invertHorizontal)
        {
            calculatedVector.y *= -1;
        }

        if(invertVertical)
        {
            calculatedVector.x *= -1;
        }
        return calculatedVector;
    }

    #endregion

    private void Update()
    {
        if (Input.touchCount > 0 && !trigger.activated && actualPlayerState == PlayerState.flying)
        {
            trigger.Activate();
            mustMoveForward = false;
            actualPlayerState = PlayerState.interacting;
        }

        else if (Input.touchCount == 0 && trigger.activated)
        {
            trigger.DeActivate();
            mustMoveForward = true;
            actualPlayerState = PlayerState.flying;
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

    public void InvertHorizontal(bool doyou)
    {
        invertHorizontal = doyou;
    }

    public void InvertVertical(bool doyou)
    {
        invertVertical = doyou;
    }
    #endregion

    public enum PlayerState
    {
        grounded,
        flying,
        interacting,
        drop,
        pique,
        forceAscend
    }
}