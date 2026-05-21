using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; 
using TMPro;

public class WinManager : MonoBehaviour
{
    [Header("Required Stable Time")]
    public float requiredStableTime = 5f; 
    private float currentStableTimer = 0f;

    [Header("Delay after Win (s)")]
    public float delayBeforeSceneChange = 1.0f;

    [Header("Countdown UI")]
    public TextMeshProUGUI countdownText;

    [Header("Pointer Buffer")]
    public float pointerStaticThreshold = 0.002f; 

    [Header("Speed Buffer")]
    public float blockVelocityThreshold = 0.05f; 

    [Header("Balance Pointers")]
    public Transform bluePointLeft;  
    public Transform bluePointRight; 

    [Header("Spawn Point")]
    public Transform queueHolder; 

    [Header("Initial Area")]
    // 如果方块距离 QueueHolder 的距离小于这个值，说明它还没被拔出来，属于作弊/未准备状态
    public float antiCheatRadius = 1.5f; 

    private Vector3 _lastLeftPosition;
    private Vector3 _lastRightPosition;

    private bool isGameWon = false;
    private float _printCooldownTimer = 0f;

    void Start()
    {
        if (bluePointLeft != null) _lastLeftPosition = bluePointLeft.position;
        if (bluePointRight != null) _lastRightPosition = bluePointRight.position;
        
        if (countdownText != null) countdownText.text = ""; 
    }

    void Update()
    {
        if (isGameWon) return;

        Draggable[] allBlocks = FindObjectsByType<Draggable>(FindObjectsSortMode.None);

        if (allBlocks.Length == 0)
        {
            ResetWinSystem();
            return;
        }

        bool isEverythingStationary = true;

        // No stay in initial place
        if (queueHolder != null)
        {
            foreach (var block in allBlocks)
            {
                if (block != null)
                {
                    float distToSpawn = Vector3.Distance(block.transform.position, queueHolder.position);
                    
                    if (distToSpawn < antiCheatRadius)
                    {
                        isEverythingStationary = false;
                        
                        _printCooldownTimer += Time.deltaTime;
                        if (_printCooldownTimer >= 1.0f)
                        {
                            Debug.Log($"<color=#FFD700>⚠️ [Anti-Cheat] blocks around initial pos, 不开始倒计时！</color>");
                            _printCooldownTimer = 0f;
                        }
                        break;
                    }
                }
            }
        }

        // check pointers
        if (isEverythingStationary && bluePointLeft != null && bluePointRight != null)
        {
            float leftMoveDist = Vector3.Distance(bluePointLeft.position, _lastLeftPosition);
            float rightMoveDist = Vector3.Distance(bluePointRight.position, _lastRightPosition);

            if (leftMoveDist > pointerStaticThreshold || rightMoveDist > pointerStaticThreshold)
            {
                isEverythingStationary = false;
            }
        }

        // check blocks
        if (isEverythingStationary)
        {
            foreach (var block in allBlocks)
            {
                if (block != null)
                {
                    Rigidbody2D rb = block.GetComponent<Rigidbody2D>();
                    if (rb != null && rb.linearVelocity.magnitude > blockVelocityThreshold)
                    {
                        isEverythingStationary = false;
                        break;
                    }
                }
            }
        }

        // count down
        if (isEverythingStationary)
        {
            currentStableTimer += Time.deltaTime;
            _printCooldownTimer += Time.deltaTime;

            float timeLeft = requiredStableTime - currentStableTimer;
            if (countdownText != null)
            {
                if (timeLeft <= 3.0f && timeLeft > 0)
                {
                    int displaySeconds = Mathf.CeilToInt(timeLeft);
                    countdownText.text = displaySeconds.ToString();
                }
                else
                {
                    countdownText.text = "";
                }
            }

            if (_printCooldownTimer >= 1.0f)
            {
                int secondsLeft = Mathf.CeilToInt(timeLeft);
                if (secondsLeft > 0)
                {
                    Debug.Log($"<color=#32CD32>[检测中]倒计时: {secondsLeft}s</color>");
                }
                _printCooldownTimer = 0f;
            }

            if (currentStableTimer >= requiredStableTime)
            {
                TriggerWin();
            }
        }
        else
        {
            currentStableTimer = 0f;
            
            if (_printCooldownTimer > 1.0f) _printCooldownTimer = 1.0f; 
            if (countdownText != null) countdownText.text = "";
        }

        SavePointerPositions();
    }

    private void ResetWinSystem()
    {
        currentStableTimer = 0f;
        if (countdownText != null) countdownText.text = "";
        SavePointerPositions();
    }

    private void SavePointerPositions()
    {
        if (bluePointLeft != null) _lastLeftPosition = bluePointLeft.position;
        if (bluePointRight != null) _lastRightPosition = bluePointRight.position;
    }

    void TriggerWin()
    {
        isGameWon = true;

        if (countdownText != null) countdownText.text = "WIN!";
        Debug.Log("<color=yellow>WIN! Loading endscene...</color>");

        StartCoroutine(DelayAndChangeScene());
    }

    private IEnumerator DelayAndChangeScene()
    {
        yield return new WaitForSeconds(delayBeforeSceneChange);
        SceneManager.LoadScene("EndScene");
    }

    void OnDrawGizmosSelected()
    {
        if (queueHolder != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(queueHolder.position, antiCheatRadius);
        }
    }
}