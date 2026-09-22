using UnityEngine;

public class Killzone : MonoBehaviour
{
    private PlayerController _playerController;

    void Start()
    {
        _playerController = FindAnyObjectByType<PlayerController>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _playerController.Respawn();
        }
    }
}
