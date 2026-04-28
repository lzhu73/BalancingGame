using UnityEngine;

public class RodBalance : MonoBehaviour
{
    private Rigidbody2D rb;
    public float yOffset = -1.0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // gravity position
        rb.centerOfMass = new Vector2(0, yOffset);
    }
}