using UnityEngine;
using UnityEngine.Audio;


public class EnemyMovement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private float moveSpeed = 2.0f;
    

    [SerializeField] private GameObject _enemyHitbox;
    [SerializeField] private GameObject _deadHitbox;
    [SerializeField] private GameObject _takeAttackHitbox;
    [SerializeField] private AudioClip enemyDeathSound;

    private Animator anim; //LUKAS
    private SpriteRenderer rend;
    private AudioSource audioSource;

    private bool isDestroyed = false;

    private void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>(); //LUKAS
        audioSource = GetComponent<AudioSource>(); //Lukas
        _enemyHitbox.SetActive(true);
        _deadHitbox.SetActive(false);
        _takeAttackHitbox.SetActive(true);


    }

    private void Update()
    {
        anim.SetBool("isDestroyed", isDestroyed);
        anim.SetFloat("MoveSpeed", Mathf.Abs (moveSpeed));

        if (moveSpeed < 0)
        {
            rend.flipX = true;
        }

        if (moveSpeed > 0)
        {
            rend.flipX = false;
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Translate(new Vector2(moveSpeed, 0) * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("EnemyBlock")) // om den kolliderar med tag enemy block
        {
            moveSpeed = -moveSpeed; // inverterar moveSpeed
        }

     /*   if (other.gameObject.CompareTag("AttackCheck"))
        {
           other.gameObject.GetComponent<PlayerController>().HandleMovement();

            if (other.transform.position.x > transform.position.x)
            {
                other.gameObject.GetComponent<PlayerController>().TakeKnockback(knockbackForce, upwardsForce);
            }
            else
            {
                other.gameObject.GetComponent<PlayerController>().TakeKnockback(-knockbackForce, upwardsForce);
            }
        }*/
    }
    private void OnTriggerEnter2D(Collider2D othern)
    {
        if (othern.CompareTag("AttackCheck")) // kollar om det som kolliderar har tagen player
        {



           /* Rigidbody2D rgbd = othern.attachedRigidbody;

            if (rgbd != null)
            {

                rgbd.linearVelocity = new Vector2(rgbd.linearVelocity.x, 0); // nollställer fall farten så att bounciness inte påverkas av hur snabbt man faller på objektet
                rgbd.AddForce(new Vector2(0, bounciness));
            }*/

            moveSpeed = 0;
            audioSource.PlayOneShot(enemyDeathSound, 0.3f);
            isDestroyed = true;
            _enemyHitbox.SetActive(false);
            _deadHitbox.SetActive(true);
            _takeAttackHitbox.SetActive(false);
            //tar bort fienden

        }
    }
}
