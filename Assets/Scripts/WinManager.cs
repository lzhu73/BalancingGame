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

        Draggable[] allBlocks = FindObjectsOfType<Draggable>();

        if (allBlocks.Length == 0)
        {
            ResetWinSystem();
            return;
        }

        bool isEverythingStationary = true;

        if (bluePointLeft != null && bluePointRight != null)
        {
            float leftMoveDist = Vector3.Distance(bluePointLeft.position, _lastLeftPosition);
            float rightMoveDist = Vector3.Distance(bluePointRight.position, _lastRightPosition);

            if (leftMoveDist > pointerStaticThreshold || rightMoveDist > pointerStaticThreshold)
            {
                isEverythingStationary = false;
            }
        }

        // check velocity
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

            // win condition
            if (currentStableTimer >= requiredStableTime)
            {
                TriggerWin();
            }
        }
        else
        {
            currentStableTimer = 0f;
            _printCooldownTimer = 1.0f;
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
        // wait 1s
        yield return new WaitForSeconds(delayBeforeSceneChange);

        SceneManager.LoadScene("EndScene");
    }
}