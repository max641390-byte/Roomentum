using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    [SerializeField] private GameObject noteTextOne;
    [SerializeField] private GameObject noteTextTwo;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            noteTextOne.SetActive(false);
            noteTextTwo.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            noteTextOne.SetActive(true);
            noteTextTwo.SetActive(false);
        }
    }
}
