using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartGame : MonoBehaviour
{
    public void LoadLevel1()
    {
        SceneManager.LoadScene("Level01"); 
    }
}