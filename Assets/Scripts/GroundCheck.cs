using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [HideInInspector] public bool IsGrounded;

    [SerializeField] private LayerMask groundLayer;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((groundLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            IsGrounded = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if ((groundLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            IsGrounded = false;
        }
    }


}