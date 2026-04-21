using UnityEngine;

public class RodBalancer : MonoBehaviour
{
    [Header("References")]
    public Transform pivotPoint;
    public Transform[] blocks;

    [Header("Balance Settings")]
    public float tiltStrength = 10f;
    public float maxTiltAngle = 25f;
    public float smoothSpeed = 5f;

    private float currentAngle = 0f;

    void Update()
    {
        float totalTorque = CalculateTorque();

        // base on distance
        float targetAngle = totalTorque * tiltStrength;

        // limit max tilt
        targetAngle = Mathf.Clamp(targetAngle, -maxTiltAngle, maxTiltAngle);

        // smooth tilt
        currentAngle = Mathf.Lerp(currentAngle, targetAngle, Time.deltaTime * smoothSpeed);

        transform.rotation = Quaternion.Euler(0f, 0f, currentAngle);
    }

    float CalculateTorque()
    {
        float totalTorque = 0f;

        foreach (Transform block in blocks)
        {
            if (block == null) continue;

            BlockWeight bw = block.GetComponent<BlockWeight>();
            if (bw == null) continue;

            // block distance from pivot
            float distanceFromPivot = block.position.x - pivotPoint.position.x;

            // torque = weight * distance
            totalTorque += bw.weight * distanceFromPivot;
        }

        return totalTorque;
    }
}