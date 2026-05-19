using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartGame : MonoBehaviour
{
    public void LoadLevel1()
    {   
        Time.timeScale = 1f; 

        SceneManager.LoadScene("Level01"); 
    }
}