using UnityEngine;

public class Draggable : MonoBehaviour
{
    private TargetJoint2D _joint;
    private Rigidbody2D _rb;
    private Collider2D _blockCollider;
    private Collider2D[] _allEnvironmentColliders;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _blockCollider = GetComponent<Collider2D>();

        GameObject[] envObjects = GameObject.FindGameObjectsWithTag("Environment");
        _allEnvironmentColliders = new Collider2D[envObjects.Length];
        for (int i = 0; i < envObjects.Length; i++)
        {
            _allEnvironmentColliders[i] = envObjects[i].GetComponent<Collider2D>();
        }
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

        ToggleEnvironmentCollisions(true);
        // if (_rodCollider != null)
        // {
        //     Physics2D.IgnoreCollision(_blockCollider, _rodCollider, true);
        // }
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

        ToggleEnvironmentCollisions(false);
        // if (_rodCollider != null)
        // {
        //     Physics2D.IgnoreCollision(_blockCollider, _rodCollider, false);
        // }
    }

    private Vector2 GetMouseWorldPos()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return new Vector2(pos.x, pos.y);
    }

    void ToggleEnvironmentCollisions(bool ignore)
    {
        foreach (var envCollider in _allEnvironmentColliders)
        {
            if (envCollider != null)
            {
                Physics2D.IgnoreCollision(_blockCollider, envCollider, ignore);
            }
        }
    }
}