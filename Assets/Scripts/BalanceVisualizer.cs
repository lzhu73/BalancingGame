using UnityEngine;

public class BalanceVisualizer : MonoBehaviour
{
    [Header("Rod End Points")]
    public Transform rodLeftPoint;   
    public Transform rodRightPoint;  

    [Header("Measure Pointers")]
    public Transform bluePointLeft;  
    public Transform bluePointRight; 

    [Header("Pointer's Y Limits")]
    public float minY = -3.5f; 
    public float maxY = 3.5f;  

    void Update()
    {
        if (rodLeftPoint != null && bluePointLeft != null)
        {
            float targetY = rodLeftPoint.position.y;

            // clamp can lock it inside the interval
            float clampedY = Mathf.Clamp(targetY, minY, maxY);

            bluePointLeft.position = new Vector3(bluePointLeft.position.x, clampedY, bluePointLeft.position.z);
        }

        if (rodRightPoint != null && bluePointRight != null)
        {
            float targetY = rodRightPoint.position.y;

            float clampedY = Mathf.Clamp(targetY, minY, maxY);

            bluePointRight.position = new Vector3(bluePointRight.position.x, clampedY, bluePointRight.position.z);
        }
    }
}