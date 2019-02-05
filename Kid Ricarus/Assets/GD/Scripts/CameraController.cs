using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public AnimationCurve cameraElasticity;

    public Transform target;

    Vector3 baseOffset;

    Camera cam;
    private Vector3 screenPoint;
    private Vector3 offset;
    public float distance = 5.0f;
    public float xSpeed = 50.0f;
    public float ySpeed = 50.0f;

    public float yMinLimit = -80f;
    public float yMaxLimit = 80f;

    public float distanceMin = .5f;
    public float distanceMax = 200f;

    public float smoothTime = 2f;

    public float rotationYAxis = 0.0f;
    float rotationXAxis = 0.0f;

    float velocityX = 0.0f;
    float velocityY = 0.0f;
    float moveDirection = -1;


    void Start()
    {
        cam = GetComponent<Camera>();
        Vector3 angles = transform.eulerAngles;
        rotationYAxis = (rotationYAxis == 0) ? angles.y : rotationYAxis;
        rotationXAxis = angles.x;
    }

    public void UpdateCamera(float yOffset, float xOffset)
    {
        velocityX += xSpeed * xOffset * cameraElasticity.Evaluate(Mathf.Abs(xOffset)) * Time.deltaTime;
        velocityY += ySpeed * yOffset * cameraElasticity.Evaluate(Mathf.Abs(yOffset)) * Time.deltaTime *-1;
        rotationYAxis += velocityX;
        rotationXAxis -= velocityY;

        rotationXAxis = ClampAngle(rotationXAxis, yMinLimit, yMaxLimit);
        rotationYAxis = ClampAngle(rotationYAxis, yMinLimit, yMaxLimit);

        Quaternion fromRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
        Quaternion toRotation = Quaternion.Euler(rotationXAxis, rotationYAxis, 0);
        Quaternion rotation = toRotation;

        Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
        Vector3 position = rotation * negDistance + target.position;

        transform.rotation = rotation;
        transform.position = position;

        velocityX = Mathf.Lerp(velocityX, 0, Time.deltaTime * smoothTime);
        velocityY = Mathf.Lerp(velocityY, 0, Time.deltaTime * smoothTime);
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360f)
            angle += 360f;
        if (angle > 360f)
            angle -= 360f;

        return Mathf.Clamp(angle, min, max);
    }

    void Awake()
    {
        baseOffset = target.position - transform.position;
    }
}
