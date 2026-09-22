using UnityEngine;
using UnityEngine.SceneManagement;

public class ClearGame : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Invoke(nameof(ChangeScene), 2f);
        }
    }

    void ChangeScene()
    {
        SceneManager.LoadScene(1); 
    }
}
