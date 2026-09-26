using UnityEngine;

public class PowerupController : MonoBehaviour
{
    public string abilityName;
    private PlayerAbilities _playerAbilities;
    void Start()
    {

        _playerAbilities = FindAnyObjectByType<PlayerAbilities>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            
            switch (abilityName)
            {
                case "Jump":
                    _playerAbilities.UnlockJump();
                    break;
                case "WallJump":
                    _playerAbilities.UnlockWallJump();
                    break;
                case "DoubleJump":
                    _playerAbilities.UnlockDoubleJump();
                    break;
                case "Dash":
                    _playerAbilities.UnlockDash();
                    break;
                case "Glide":
                    _playerAbilities.UnlockGlide();
                    break;
                case "Attack":
                    _playerAbilities.UnlockAttack();
                    break;
                case "Sprint":
                    _playerAbilities.UnlockSprint();
                    break;
                case "Slide":
                    _playerAbilities.UnlockSlide();
                    break;
                default:
                    Debug.LogWarning("Unknown ability: " + abilityName);
                    break;
            }
            Debug.Log("Power-up collected!");
            Destroy(gameObject);
        }
    }
}
