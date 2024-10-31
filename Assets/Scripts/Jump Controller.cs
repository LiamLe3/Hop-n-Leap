using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class JumpController : MonoBehaviour
{
    Rigidbody2D myRigidbody;
    ArrowController arrowController;
    FlipController flipController;
    Animator myAnimator;
    AudioSource audioSource;


    [SerializeField] float minJumpPower = 2f;
    [SerializeField] float jumpMultiplier = 2f;
    [SerializeField] float gravity = 1.5f;

    [SerializeField] Vector2 boxSize;
    [SerializeField] float castDistance;

    [SerializeField] LayerMask platformLayer;
    [SerializeField] AudioClip jumpSFX;

    float startTime;
    bool bufferJump;
    bool jumpHeld;
    bool isWalled;

    void Awake()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        arrowController = GetComponent<ArrowController>();
        flipController = GetComponent<FlipController>();
    }

    void Update()
    {
        Airborne();

        if(jumpHeld && !bufferJump)
        {
            arrowController.UpdateArrowIndicator(startTime);
        }
    }
    
    void OnJump(InputValue value)
    {
        if(value.isPressed)
        {
            jumpHeld = true;
            if(IsGrounded())
                startTime = Time.time;
            else
                bufferJump = true;

            myAnimator.SetBool("isPreparing", true);
        }
        else
        {
            jumpHeld = false;
            myAnimator.SetBool("isPreparing", false);
            myAnimator.SetBool("isJumping", true);

            if(!IsGrounded()) return; //Do nothing if froggy is in the air

            ApplyJumpForce();
            
            if(isWalled)
                JumpOffWall();

            arrowController.UpdateArrowIndicator(0);
            audioSource.PlayOneShot(jumpSFX);
        }
    }

    public bool IsGrounded()
    {
        int rotation = 0;

        if(isWalled)
            rotation = 90;

        if(Physics2D.BoxCast(transform.position, boxSize, rotation, -transform.up, castDistance, platformLayer))
            return true;
        else
            return false;
    }

    public bool IsWalled()
    {
        return isWalled;
    }

    //Reverts sprite to normal orientation.
    void JumpOffWall()
    {
        isWalled = false;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        myRigidbody.gravityScale = gravity;
        flipController.FlipSprite();
    }

    void ApplyJumpForce()
    {
        float heldTime = Time.time - startTime;
        float jumpPower = minJumpPower + jumpMultiplier * Mathf.Clamp(heldTime, 0, 1);
        myRigidbody.velocity += CalculateJumpDirection(jumpPower, arrowController.GetArrowAngle());
    }

    //Calculates x-axis and y-axis force for the resultant jump power at a given angle.
    Vector2 CalculateJumpDirection(float jumpPower, float angle)
    {
        float horizontalForce = jumpPower * Mathf.Cos(angle * Mathf.Deg2Rad);
        float verticalForce = jumpPower * Mathf.Sin(angle * Mathf.Deg2Rad);

        return new Vector2 (horizontalForce, verticalForce);
    }

    void Airborne()
    {
        if(IsGrounded())
        {
            myAnimator.SetBool("isFalling", false);
            myAnimator.SetBool("isJumping", false);

            if(bufferJump)
            {
                startTime = Time.time;
                bufferJump = false;
            }
            return;
        }

        CheckVelocityChange();
    }

    void CheckVelocityChange()
    {
        if(myRigidbody.velocity.y > Mathf.Epsilon)
        {
            myAnimator.SetBool("isJumping", true);
            myAnimator.SetBool("isFalling", false); 
        }
        else if(myRigidbody.velocity.y < Mathf.Epsilon)
        {
            myAnimator.SetBool("isJumping", false);
            myAnimator.SetBool("isFalling", true);
        }
    }

    //Called when froggy jumps into or touches the wall.
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Platforms"))
        {
            int direction = 1;
            if(!flipController.IsFlipped())
                direction *=  -1;

            isWalled = true;
            transform.rotation = Quaternion.Euler(0, 0, direction * 90);
            myRigidbody.gravityScale = 0;

            flipController.ToggleFlip();
            flipController.FlipArrow();
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position - transform.up * castDistance, boxSize);

        if (jumpHeld && IsGrounded())
        {
            Vector2 startPosition = transform.position;

            // Simulate the current jump power based on how long the jump button has been held
            float heldTime = Time.time - startTime;
            float jumpPower = minJumpPower + jumpMultiplier * Mathf.Clamp(heldTime, 0, 2);
            float angle = arrowController.GetArrowAngle();

            // Calculate the initial velocity from the jump power and angle
            Vector2 initialVelocity = CalculateJumpDirection(jumpPower, angle);

            // Gravity setting (this should match your Rigidbody2D's gravity scale)
            float gravity = 1.5f;

            // Initialize simulation parameters
            Vector2 currentPosition = startPosition;
            Vector2 currentVelocity = initialVelocity;
            int resolution = 50;   // Higher resolution for smoother arc
            float timeStep = 0.05f; // Small time step for more accurate simulation

            // Draw the arc by simulating physics over time
            for (int i = 0; i < resolution; i++)
            {
                // Simulate the effects of gravity on vertical velocity
                currentVelocity.y += gravity * Physics2D.gravity.y * timeStep;

                // Calculate the new position based on current velocity
                Vector2 newPosition = currentPosition + currentVelocity * timeStep;

                // Draw a line segment from the previous position to the new position
                Gizmos.color = Color.red;
                Gizmos.DrawLine(currentPosition, newPosition);

                // Update current position for the next step in the simulation
                currentPosition = newPosition;
            }
        }
    }
}