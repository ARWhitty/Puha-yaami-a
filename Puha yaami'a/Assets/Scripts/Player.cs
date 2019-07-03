using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Tailored Fields
    [SerializeField] private float moveAmount;
    [SerializeField] private float jumpForce;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float gravAmt;
    [SerializeField] private float dashTime;
    [SerializeField] private float startDashTime;
    [SerializeField] private float ladderClimbSpeed;
    [SerializeField] private float glideDelayTimer;
    [SerializeField] private float glideMoveModifier;

    [SerializeField] private int glideGravModifier;

        #endregion

    #region Internal Fields
    //-1 is left, 1 is right, 0 is not moving
    private int direction;

    [SerializeField]private bool isGrounded, isDashing, isGliding, isClimbing;
    private bool canGlide = false;
    private bool startGlideTimer = false;

    private Vector3 moveVector, climbVector;

    private Rigidbody2D playerRB;

    private float currJumpForce;
    private float glideGravAmt;
    private float glideDelayTimerCount;
    #endregion

    #region Start/Update
    // Start is called before the first frame update
    void Start()
    {
        moveVector = new Vector3(moveAmount, 0f);
        climbVector = new Vector3(0f, ladderClimbSpeed);
        playerRB = this.GetComponent<Rigidbody2D>();
        playerRB.gravityScale = gravAmt;
        glideGravAmt = gravAmt / glideGravModifier;
        glideDelayTimerCount = glideDelayTimer;

        isGrounded = true;
        isDashing = false;
        isGliding = false;
        isClimbing = false;

        dashTime = startDashTime;

        direction = 0;

        currJumpForce = jumpForce;

    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetKey(KeyCode.D))
        {
            //If we're gliding, modify our movement to be a little faster
            if (isGliding)
                this.transform.position += moveVector * glideMoveModifier;
            else
                this.transform.position += moveVector;
            direction = 1;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            //If we're gliding, modify our movement to be a little faster
            if (isGliding)
                this.transform.position -= moveVector * glideMoveModifier;
            else
                this.transform.position -= moveVector;
            direction = -1;
        }

        //Jump
        if(Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            playerRB.AddForce(Vector2.up * currJumpForce, ForceMode2D.Impulse);
        }

        //If I let go of spacebar, we should stop gliding for this frame
        if (Input.GetKeyUp(KeyCode.Space) && isGliding)
        {
            isGliding = false;
        }

        //Glide, only if not dashing or on the ground
        if (Input.GetKey(KeyCode.Space) && !isGrounded && !isDashing && canGlide)
        {
            //if we just started gliding, zero out our velocity so we stop jumping as soon as we start the glide
            if(!isGliding)
            {
                playerRB.velocity = Vector2.zero;
            }
            //Lower gravity, mark us as gliding
            playerRB.gravityScale = glideGravAmt;
            isGliding = true;
        }

        //Dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing)
        {
            isDashing = true;
            //set the gravity scale to 0 so we get a straight midair dash if necessary
            if(!isGrounded)
            {
                playerRB.gravityScale = 0;
            }
            //add to the velocity
            if(direction == -1)
            {
                playerRB.velocity = Vector2.left * dashSpeed;
            }
            else
            {
                playerRB.velocity = Vector2.right * dashSpeed;
            }
            
        }

        //if we're not dashing/gliding/climbing, turn gravity back on pls
        if (!isDashing && !isGliding && !isClimbing)
        {
            playerRB.gravityScale = gravAmt;
        }
        //If we are, decrease our timer
        if(isDashing)
        {
            //decrease dash time
            dashTime -= Time.deltaTime;
            if (dashTime <= 0)
            {
                dashTime = startDashTime;
                playerRB.velocity = Vector2.zero;
                isDashing = false;
            }
        }
        //If we are in the middle of a jump, start our glide timer so we delay when we can glide
        if(startGlideTimer)
        {
            glideDelayTimerCount -= Time.deltaTime;
            if(glideDelayTimerCount <= 0)
            {
                glideDelayTimerCount = glideDelayTimer;
                canGlide = true;
                startGlideTimer = false;
            }
        }

        //if we arent grounded we can look to glide
        if(!isGrounded)
        {
            startGlideTimer = true;
        }    
    }
        #endregion

    #region Collisions
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Bouncy_Platform"))
            currJumpForce *= 1.5f;
        if (col.gameObject.CompareTag("Sticky_Platform"))
            currJumpForce *= 0.5f;
        if (col.gameObject.CompareTag("Fail_Platform"))
            OnFail();
    }

    void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.tag.Contains("Platform"))
        {
            isGrounded = true;
            isGliding = false;
            canGlide = false;
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.tag.Contains("Platform"))
            isGrounded = false;
        if (col.gameObject.CompareTag("Bouncy_Platform"))
            currJumpForce = jumpForce;
        if (col.gameObject.CompareTag("Sticky_Platform"))
            currJumpForce = jumpForce;
    }
    #endregion

    #region Triggers
    void OnTriggerStay2D(Collider2D col)
    {
        if(col.gameObject.CompareTag("Climbable"))
        {
            if (Input.GetKey(KeyCode.W))
            {
                playerRB.gravityScale = 0;
                this.transform.position += climbVector;
                isClimbing = true;
            }
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Climbable"))
        {
            isClimbing = false;
        }
    }
    #endregion

    /// <summary>
    /// Handles fail state stuff
    /// </summary>
    private void OnFail()
    {
        transform.position = new Vector2(10.72f, 1.95f);
    }

}
