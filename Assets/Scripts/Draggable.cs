using UnityEngine;

public class Draggable : MonoBehaviour
{
    private TargetJoint2D _joint;
    private Rigidbody2D _rb;
    private Collider2D _blockCollider;
    private Collider2D[] _allEnvironmentColliders;

    [Header("Glow Settings")]
    public GameObject glowEffect;
    private bool _hasBeenTouched = false;

    private bool _isDraggingThis = false; 
    private bool _hasSpawnedNext = false; 

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

        Debug.Log($"[Spawner] {gameObject.name} 初始化，主动 Update 检测模式。");
    }

    // void Update()
    // {
    //     if (Input.GetMouseButtonDown(0))
    //     {
    //         // isolate UI
    //         if (UnityEngine.EventSystems.EventSystem.current != null && 
    //             UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return;

    //         if (!_isDraggingThis)
    //         {
    //             Vector2 mousePos = GetMouseWorldPos();


    //             if (_blockCollider != null && _blockCollider.OverlapPoint(mousePos))
    //             {
    //                 Debug.Log($"<color=green>[Active Click Success] 检测到 {gameObject.name}！开始拖拽。</color>");
    //                 StartDrag();
    //             }
    //         }
    //     }

    //     // dragging
    //     if (_isDraggingThis && _joint != null)
    //     {
    //         _joint.target = GetMouseWorldPos();
    //     }

    //     // mouse up
    //     if (Input.GetMouseButtonUp(0) && _isDraggingThis)
    //     {
    //         Debug.Log($"[Active Click Release] {gameObject.name} 释放。");
    //         ReleaseObject();
    //     }
    // }


    void Update()
    {
        // onclick
        if (Input.GetMouseButtonDown(0))
        {
            if (!_isDraggingThis)
            {
                Vector2 mousePos = GetMouseWorldPos();

                if (_blockCollider != null && _blockCollider.OverlapPoint(mousePos))
                {
                    StartDrag();
                }
            }
        }

        // dragging
        if (_isDraggingThis && _joint != null)
        {
            _joint.target = GetMouseWorldPos();
        }

        // mouse up
        if (Input.GetMouseButtonUp(0) && _isDraggingThis)
        {
            Debug.Log($"[Active Click Release] {gameObject.name} 释放。");
            ReleaseObject();
        }
    }

    private void StartDrag()
    {
        _isDraggingThis = true;
        _hasBeenTouched = true;
        if (glowEffect != null) glowEffect.SetActive(false);
        
        if (_rb != null)
        {
            _rb.freezeRotation = true;
            _rb.WakeUp(); 
        }

        if (_joint != null) Destroy(_joint);

        _joint = gameObject.AddComponent<TargetJoint2D>();
        _joint.anchor = transform.InverseTransformPoint(GetMouseWorldPos());
        _joint.maxForce = 1000 * _rb.mass;
        _joint.dampingRatio = 1f;
        _joint.frequency = 10f;

        ToggleEnvironmentCollisions(true); //
    }

    public void ForceRelease()
    {
        ReleaseObject();
    }

    private void ReleaseObject()
    {
        _isDraggingThis = false;

        if (_rb != null)
        {
            _rb.freezeRotation = false;
        }
        
        if (_joint != null)
        {
            Destroy(_joint);
            _joint = null;
        }

        ToggleEnvironmentCollisions(false); //

        if (!_hasSpawnedNext)
        {
            _hasSpawnedNext = true;
            BlockSpawner spawner = FindAnyObjectByType<BlockSpawner>();
            if (spawner != null)
            {
                spawner.SpawnNext();
            }
        }
    }

    private Vector2 GetMouseWorldPos()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return new Vector2(pos.x, pos.y);
    }

    void ToggleEnvironmentCollisions(bool ignore)
    {
        if (_blockCollider == null || _allEnvironmentColliders == null) return;

        foreach (var envCollider in _allEnvironmentColliders)
        {
            if (envCollider != null)
            {
                Physics2D.IgnoreCollision(_blockCollider, envCollider, ignore);
            }
        }
    }

    private void OnDestroy()
    {
        ToggleEnvironmentCollisions(false);
    }
}