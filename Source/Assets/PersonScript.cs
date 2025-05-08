using System.Collections;
using UnityEngine;

public class PersonScript : MonoBehaviour
{
    public float speed = 5f;       
    public float jumpForce = 8f;      
    public Animator animator;        
    public Rigidbody rb; 
    private bool isJumping = false;
    private bool isGrounded = true;
    private bool isCroutching = false;
    private bool isDead = false;

    public Transform cameraTransform;
    public Vector3 cameraOffset = new Vector3(0, 5, -12);

    AudioManagerScript audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManagerScript>();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        if (cameraTransform != null)
        {
            Vector3 targetPosition = transform.position + cameraOffset;
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetPosition, Time.deltaTime * speed);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded && !isJumping && !isCroutching)
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) && isGrounded && !isJumping && !isCroutching)
        {
            Croutch();
        }

    }

    void Jump()
    {
        rb.linearVelocity = Vector3.up * jumpForce;
        isJumping = true;
        isGrounded = false;
        animator.SetTrigger("JumpTrigger");
        audioManager.PlaySFX(audioManager.jump);

    }

    void Croutch()
    {
        isCroutching = true;
        animator.SetTrigger("CroutchTrigger");
        transform.localScale = new Vector3(transform.localScale.x, 0.8f, transform.localScale.z);
        StartCoroutine(ResetHeightAfterDelay(1f));
    }

    IEnumerator ResetHeightAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        transform.localScale = new Vector3(transform.localScale.x, 1f, transform.localScale.z);
        isCroutching = false;
    }

    // This function checks if the player is on the ground or in the air
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJumping = false;
            isGrounded = true;
            animator.SetBool("isRunning", true);
        }
    }


    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Death"))
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        animator.SetTrigger("Dying");
        audioManager.PlaySFX(audioManager.death);
        rb.linearVelocity = Vector3.zero;
        speed = 0;
    }
}