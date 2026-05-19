using UnityEngine;

public class Draggable : MonoBehaviour
{
    private TargetJoint2D _joint;
    private Rigidbody2D _rb;
    private Collider2D _blockCollider;
    private SpriteRenderer _spriteRenderer; 

    [Header("Glow Settings")]
    public GameObject glowEffect;

    [Header("Anti Cheating")]
    public float maxHoldTime = 3.5f; // max holding time
    private float _currentHoldTimer = 0f; // current holding time
    private Color _originalColor;

    private bool _isDraggingThis = false; 
    private bool _hasSpawnedNext = false; 

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _blockCollider = GetComponent<Collider2D>();
        
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer != null)
        {
            _originalColor = _spriteRenderer.color;
        }

        // Debug.Log($"[Spawner] {gameObject.name} 初始化成功。");
    }

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
        if (_isDraggingThis)
        {
            if (_joint != null)
            {
                _joint.target = GetMouseWorldPos();
            }

            // holding time added up
            _currentHoldTimer += Time.deltaTime;

            // smoothly turnning red
            if (_spriteRenderer != null)
            {
                float progress = _currentHoldTimer / maxHoldTime;
                // Color.Lerp 渐变颜色
                _spriteRenderer.color = Color.Lerp(_originalColor, Color.red, progress);
            }

            // force fall down
            if (_currentHoldTimer >= maxHoldTime)
            {
                Debug.Log($"<color=red>[Anti-Cheat]强制松手！</color>");
                ReleaseObject();
            }
        }

        // mouse up
        if (Input.GetMouseButtonUp(0) && _isDraggingThis)
        {
            ReleaseObject();
        }
    }

    private void StartDrag()
    {
        _isDraggingThis = true;
        _currentHoldTimer = 0f; 
        
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

        // anti collision
        ToggleEnvironmentCollisions(true);
    }

    public void ForceRelease()
    {
        ReleaseObject();
    }

    private void ReleaseObject()
    {
        _isDraggingThis = false;

        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = _originalColor;
        }

        if (_rb != null)
        {
            _rb.freezeRotation = false;
        }
        
        if (_joint != null)
        {
            Destroy(_joint);
            _joint = null;
        }

        // collision back
        ToggleEnvironmentCollisions(false);

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
        if (_blockCollider == null) return;

        // tag Environment
        GameObject[] envObjects = GameObject.FindGameObjectsWithTag("Environment");
        
        foreach (var envObj in envObjects)
        {
            if (envObj != null)
            {
                Collider2D envCollider = envObj.GetComponent<Collider2D>();
                if (envCollider != null)
                {
                    Physics2D.IgnoreCollision(_blockCollider, envCollider, ignore);
                }
            }
        }
    }
}