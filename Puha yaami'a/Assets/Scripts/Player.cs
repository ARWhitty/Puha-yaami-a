using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Tailored Fields
    [Header("Tailored Fields")]
    [Tooltip("The amount the player moves each frame side to side")]
    [SerializeField] private float moveAmount;
    [Tooltip("The amount of force the player jumps with")]
    [SerializeField] private float jumpForce;
    [Tooltip("The amount of force applied when the player dashes")]
    [SerializeField] private float dashSpeed;
    [Tooltip("The force of gravity affecting the player. Higher = more pull")]
    [SerializeField] private float gravAmt;
    [Tooltip("The length of a player's dash, in seconds")]
    [SerializeField] private float startDashTime;
    [Tooltip("Speed the player climbs a ladder")]
    [SerializeField] private float ladderClimbSpeed;
    [Tooltip("The delay, in seconds, before the player can begin to clide once they've jumped")]
    [SerializeField] private float glideDelayTimer;
    [Tooltip("Multiplier for how much extra a player moves horizontally when gliding side to side. Numbers should range from 1.01 and up")]
    [SerializeField] private float glideMoveModifier;
    [Tooltip("Makes wind stronger or weaker on the player. Values may range from 0.01 and up")]
    [SerializeField] private float windGlideModifier;
    [Tooltip("Reduces the gravity on the player while gliding. Values should range between 0.01 and 1 (no modification)")]
    [SerializeField] private float glideGravModifier;
    [Tooltip("The curve for how fast a player returns to normal gravity while gliding. Values should range between 0.01 and 1, the alrger the number the faster the fall as glide continues")]
    [SerializeField] private float glideCurveModifier;
    [Tooltip("The cooldown before a player may dash again, in seconds")]
    [SerializeField] private float dashCooldown;
    [Tooltip("How long, in seconds, a player may hold the jump button to jump higher")]
    [SerializeField] private float jumpTimer;
    [Tooltip("The extra force added when holding the jump key, values between 0.8-1.2 seem to work best")]
    [SerializeField] private float additiveJumpAmount;

    #endregion

    #region events
    //0 is normal, 1 is bouncy, 2 is sticky, 3 is death, 4 is score loss
    public delegate void PlatformCollisions(int type);
    public static event PlatformCollisions OnCollide;

    public delegate void TriggerPassthroughs(string type, Collider2D obj);
    public static event TriggerPassthroughs OnTrigger;
    #endregion

    #region Internal Fields
    //-1 is left, 1 is right, 0 is not moving
    private int direction_KB, direction_CTRL;
    private int num_jumps;

    [SerializeField]private bool isDashing, isGliding, isClimbing, inWind, startDashCd, canDash, climbPressed, onLadder, isJumping;
    private bool canGlide = false;
    private bool startGlideTimer = false;
    private bool canDoubleJump = false;

    private Vector2 currentWindForce = Vector2.zero;

    private Vector3 moveVector, climbVector, widthOffset, jumpNudge;

    [SerializeField] private bool dblJumpUnlocked = false;
    [SerializeField] private bool dashUnlocked = false;
    [SerializeField] private bool glideUnlocked = false;

    private Rigidbody2D playerRB;

    private float dashTime;
    private float currJumpForce;
    private float glideGravAmt;
    private float glideDelayTimerCount;
    private float dashCd;
    private float ladderCheckDistance;

    private float spriteWidth;
    private float collHeight;
    private float jumpTimerCount;
    private int currDirection = 0;

    private LayerMask groundedFilter;
    private LayerMask ladderFilter;
    #endregion

    #region Start/Update/Enable

    // Start is called before the first frame update
    void Start()
    {
        moveVector = new Vector3(moveAmount, 0f);
        climbVector = new Vector3(0f, ladderClimbSpeed);
        jumpNudge = new Vector3(0f, 0.3f);

        playerRB = this.GetComponent<Rigidbody2D>();
        playerRB.gravityScale = gravAmt;
        glideGravAmt = gravAmt * glideGravModifier;
        glideDelayTimerCount = glideDelayTimer;

        //isGrounded = true;
        isDashing = false;
        isGliding = false;
        isClimbing = false;
        startDashCd = false;
        canDoubleJump = false;
        climbPressed = false;
        isJumping = false;

        dashTime = startDashTime;
        dashCd = dashCooldown;
        canDash = true;

        direction_KB = 0;
        direction_CTRL = 0;
        num_jumps = 0;
        ladderCheckDistance = 6.0f;

        currJumpForce = jumpForce;

        groundedFilter = LayerMask.GetMask("Platforms");
        ladderFilter = LayerMask.GetMask("Climbable");

        spriteWidth = (float)this.GetComponent<SpriteRenderer>().bounds.size.x;
        collHeight = (float)this.GetComponent<Collider2D>().bounds.size.y / 2;
        widthOffset = new Vector3(spriteWidth/2 - 2.0f, 0, 0);

        jumpTimerCount = jumpTimer;
    }

    private void FixedUpdate()
    {
        currDirection = getDirFromAxis("Horizontal");
        Move(currDirection);
    }

    // Update is called once per frame
    void Update()
    {
        //Horizontal Movement


        if(Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        if(Input.GetButton("Jump"))
        {
            ContinueJump();
        }

        if(Input.GetButtonUp("Jump"))
        {
            isJumping = false;
        }

        if(Input.GetButtonDown("Glide"))
        { 
            Glide();
        }
        if(Input.GetButtonUp("Glide"))
        {
            isGliding = false;
        }
        // if we're already gliding, curve our glide fall speed
        if (isGliding)
        {
            if (glideGravAmt <= gravAmt)
                glideGravAmt += glideCurveModifier;

            //Lower gravity, mark us as gliding
            playerRB.AddForce(currentWindForce * windGlideModifier);
            playerRB.gravityScale = glideGravAmt;
        }

        if (Input.GetButtonDown("Dash"))
        {
            Dash(currDirection);
        }

        if(Input.GetAxisRaw("Vertical") != 0)
        {
            Climb(getDirFromAxis("Vertical"));
        }

        //if we're not dashing/gliding/climbing, turn gravity back on pls
        if (!isDashing && !isGliding && !isClimbing)
        {
            playerRB.gravityScale = gravAmt;
            glideGravAmt = gravAmt * glideGravModifier;
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
                startDashCd = true;
                canDash = false;
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

        //Cooldown between dashes
        if(startDashCd)
        {
            dashCd -= Time.deltaTime;
            if(dashCd <= 0)
            {
                dashCd = dashCooldown;
                startDashCd = false;
                canDash = true;
            }
        }

        //if we arent grounded we can look to glide and apply wind force
        if(!IsGrounded())
        {
            playerRB.AddForce(currentWindForce);
            startGlideTimer = true;
        }
        //If we are set the jumps we have available to our maximum
        else
        {
            num_jumps = GetMaxJumps();
        }
    }
    #endregion

    private int getDirFromAxis(string axisName)
    {
        float axisAmt = Input.GetAxisRaw(axisName);
        if (axisAmt > 0)
            return 1;
        if (axisAmt < 0)
            return -1;
        return 0;
    }

    private void Move(int dir)
    {
        if(!isDashing)
        {
            //If we're gliding, modify our movement to be a little faster
            if (isGliding)
                this.transform.position += moveVector * glideMoveModifier * dir;
            //otherwise just move along the vector
            else
                this.transform.position += moveVector * dir;
        }
    }

    private void ContinueJump()
    {
        //if we're already mid-jump, keep adding force while our timer ticks
        if (isJumping)
        {
            if (jumpTimerCount <= 0)
            {
                isJumping = false;
            }
            else
            {
                jumpTimerCount -= Time.deltaTime;
                playerRB.AddForce(Vector2.up * additiveJumpAmount, ForceMode2D.Impulse);
            }
        }
    }

    private void Jump()
    {
        if(num_jumps > 0)
        {
            isJumping = true;
            jumpTimerCount = jumpTimer;
            //set the upwards velocity to 0 so we don't have additive jump, then add the force
            playerRB.velocity = Vector2.zero;
            //this is here so that the player is not instantly marked grounded again and able to triple jump
            //may want to make this better in the future? Clever workaround solution for now
            this.transform.position += jumpNudge;
            playerRB.AddForce(Vector2.up * currJumpForce, ForceMode2D.Impulse);
            num_jumps -= 1;
        }
    }

    private int GetMaxJumps()
    {
        if(dblJumpUnlocked)
        {
            return 2;
        }
        else
        {
            return 1;
        }
    }

    private void Glide()
    {
        if (glideUnlocked)
        {
            //set up glide if necessary
            if (!IsGrounded() && !isDashing && canGlide)
            {
                //if we just started gliding, zero out our velocity so we stop jumping as soon as we start the glide
                if (!isGliding)
                {
                    playerRB.velocity = Vector2.zero;
                }
                isGliding = true;
            }
        }
    }

    private void Dash(int dir)
    {
        if (dashUnlocked)
        {
            if (canDash && !isDashing && dir != 0)
            {
                isDashing = true;
                //set the gravity scale to 0 so we get a straight midair dash if necessary

                playerRB.gravityScale = 0;
                playerRB.velocity = new Vector2(dir, 0) * dashSpeed;
            }
        }
    }

    private void Climb(int dir)
    {
        if(onLadder)
        {
            playerRB.velocity = Vector2.zero;
            playerRB.gravityScale = 0;
            isClimbing = true;
            transform.position += climbVector * dir;
        }
    }

    private bool IsGrounded()
    {
        RaycastHit2D hitCenter = Physics2D.Raycast(transform.position, Vector2.down, collHeight, groundedFilter);
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position - widthOffset, Vector2.down, collHeight, groundedFilter);
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position + widthOffset, Vector2.down, collHeight, groundedFilter);

        //DEBUG stuff for my own sanity. Please do not delete until everything is done
/*        Debug.DrawRay(transform.position + widthOffset, Vector2.down * collHeight, Color.blue);
        Debug.DrawRay(transform.position, Vector2.down * collHeight, Color.blue);
        Debug.DrawRay(transform.position - widthOffset, Vector2.down * collHeight, Color.blue);*/
        if (hitCenter.collider != null|| hitLeft.collider != null || hitRight.collider != null)
        {
            return true;
        }
        return false;
    }

    #region Collisions
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Bouncy_Platform"))
        {
            OnCollide(1);
        }
        if (col.gameObject.CompareTag("Sticky_Platform"))
        {
            OnCollide(2);
        }
        if (col.gameObject.CompareTag("Fail_Platform"))
        {
            OnCollide(3);
        }
    }

    void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.tag.Contains("Platform"))
        {
            isGliding = false;
            canGlide = false;
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Bouncy_Platform"))
            currJumpForce = jumpForce;
        if (col.gameObject.CompareTag("Sticky_Platform"))
            currJumpForce = jumpForce;
    }
    #endregion

    #region Triggers
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Score_Loss"))
        {
            OnCollide(4);
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if(col.gameObject.CompareTag("Climbable"))
        {
            onLadder = true;
        }
        if(col.gameObject.CompareTag("Checkpoint"))
        {
            OnTrigger("Checkpoint", col);
        }

        if(col.gameObject.CompareTag("Double_Jump_Unlock"))
        {
            OnTrigger("Double_Jump_Unlock", null);
        }

        if(col.gameObject.CompareTag("Dash_Unlock"))
        {
            OnTrigger("Dash_Unlock", null);
        }

        if(col.gameObject.CompareTag("Glide_Unlock"))
        {
            OnTrigger("Glide_Unlock", null);
        }

        if(col.gameObject.CompareTag("Wind"))
        {
            inWind = true;
            currentWindForce = col.gameObject.GetComponent<Wind>().getForce();
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Climbable"))
        {
            onLadder = false;
            isClimbing = false;
        }
        if (col.gameObject.CompareTag("Wind"))
        {
            inWind = false;
            currentWindForce = Vector2.zero;
        }
    }
    #endregion

    public void SetJumpForce(float newForce)
    {
        currJumpForce = newForce;
    }

    public float GetDefaultJumpForce()
    {
        return jumpForce;
    }

    public void UnlockAbility(int ability)
    {
        switch(ability)
        {
            case 0:
                dblJumpUnlocked = true;
                break;
            case 1:
                dashUnlocked = true;
                break;
            case 2:
                glideUnlocked = true;
                break;
        }
    }

    #region accessors/public modifiers
    /// <summary>
    /// Called from OnFail in GameManager to avoid cooldown locking immediately after dying
    /// </summary>
    public void ResetCooldowns()
    {
        dashCd = dashCooldown;
        canDash = true;
        isClimbing = false;
        isDashing = false;
        isGliding = false;
    }
    #endregion

}
