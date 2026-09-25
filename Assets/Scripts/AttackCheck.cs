using UnityEngine;

public class AttackCheck : MonoBehaviour
{
    [HideInInspector] public bool IsTouchingObject;
    [HideInInspector] public float ObjectDirection;
    [SerializeField] private LayerMask objectLayer, objectLayer2;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((objectLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            Vector2 direction = other.transform.position - transform.position;

            IsTouchingObject = true;
            ObjectDirection = Mathf.Sign(direction.x);

        }
        else if ((objectLayer2.value & (1 << other.gameObject.layer)) != 0)
        {
            Vector2 direction = other.transform.position - transform.position;

            IsTouchingObject = true;
            ObjectDirection = Mathf.Sign(direction.x);

        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if ((objectLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            IsTouchingObject = false;
        }
        else if ((objectLayer2.value & (1 << other.gameObject.layer)) != 0)
        {
            IsTouchingObject = false;
        }
    }
}