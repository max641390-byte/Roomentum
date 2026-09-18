using UnityEngine;

public class Wallcheck : MonoBehaviour
{
    [HideInInspector] public bool IsTouchingWall;
    [HideInInspector] public float WallDirection;
    [SerializeField] private LayerMask wallLayer;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((wallLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            IsTouchingWall = true;
            WallDirection = other.transform.position.x > transform.position.x ? -1 : 1;
            Debug.Log("Wall detected!");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if ((wallLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            IsTouchingWall = false;
        }
    }
}