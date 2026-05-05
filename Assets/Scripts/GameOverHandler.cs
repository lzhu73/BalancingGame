using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverHandler : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Environment") || collision.GetComponent<Draggable>() != null)
        {
            Debug.Log("Game Over: " + collision.gameObject.name);
            TriggerGameOver();
        }
    }

    void TriggerGameOver()
    {
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        SceneManager.LoadScene("EndScene"); 
    }
}