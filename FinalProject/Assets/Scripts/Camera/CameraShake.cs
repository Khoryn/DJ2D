using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    private Transform cameraTransform;

    public float shakeDuration = 0f;

    public float shakeAmount = 0.7f;
    public float decreaseFactor = 1.0f;

    Vector3 originalPos;

    void Awake()
    {
        cameraTransform = GetComponent(typeof(Transform)) as Transform;
    }

    void OnEnable()
    {
        originalPos = cameraTransform.localPosition;
    }

    void Update()
    {
        if (shakeDuration > 0)
        {
            cameraTransform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;

            shakeDuration -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            shakeDuration = 0f;
            cameraTransform.localPosition = originalPos;
        }
    }
}