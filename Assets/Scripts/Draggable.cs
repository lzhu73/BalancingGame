using UnityEngine;

public class Draggable : MonoBehaviour
{
    private TargetJoint2D _joint;
    private Rigidbody2D _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void OnMouseDown()
    {
        // when drag stop all rotation
        _rb.freezeRotation = true;

        // when click create a joint
        _joint = gameObject.AddComponent<TargetJoint2D>();
        
        _joint.anchor = transform.InverseTransformPoint(GetMouseWorldPos());
        
        _joint.maxForce = 1000 * _rb.mass;
        _joint.dampingRatio = 1f;
        _joint.frequency = 10f;
    }

    void OnMouseDrag()
    {
        if (_joint != null)
        {
            _joint.target = GetMouseWorldPos();
        }
    }

    void OnMouseUp()
    {
        // when dragging stopped unlock rotation
        _rb.freezeRotation = false;
        
        // when not onclick let gravity control box again
        if (_joint != null)
        {
            Destroy(_joint);
        }
    }

    private Vector2 GetMouseWorldPos()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return new Vector2(pos.x, pos.y);
    }
}