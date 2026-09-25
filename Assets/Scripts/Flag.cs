using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Flag : MonoBehaviour
{
    [SerializeField] private GameObject panel, finsishedText, unfinishedText; 
    [SerializeField] private int levelIndex; 
  



    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player")) 
        {
                finsishedText.SetActive(true); 
                Invoke(nameof(LoadNextLevel), 3.0f);     
        }

    }
    private void LoadNextLevel() 
    {
        SceneManager.LoadScene(levelIndex); 
    }


    private void OnTriggerExit2D(Collider2D other) 
    {
        finsishedText.SetActive(false); 
       
    }
}
