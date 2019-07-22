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

    #endregion

    #region events
    //0 is normal, 1 is bouncy, 2 is sticky, 3 is death, 4 is score loss
    public delegate void PlatformCollisions(int type);
    public static event PlatformCollisions OnCollide;

    public delegate void TriggerPassthroughs(string type, GameObject obj);
    public static event TriggerPassthroughs OnTrigger;
    #endregion

    #region Internal Fields
    //-1 is left, 1 is right, 0 is not moving
    private int direction_KB, direction_CTRL;
    private int num_jumps;

    [SerializeField]private bool isDashing, isGliding, isClimbing, inWind, startDashCd, canDash;
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

    private float spriteWidth;
    private float collHeight;

    private LayerMask groundedFilter;
    #endregion

    #region Start/Update
    // Start is called before the first frame update
    void Start()
    {
        moveVector = new Vector3(moveAmount, 0f);
        climbVector = new Vector3(0f, ladderClimbSpeed);
        jumpNudge = new Vector3(0f, 0.02f);

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

        dashTime = startDashTime;
        dashCd = dashCooldown;
        canDash = true;

        direction_KB = 0;
        direction_CTRL = 0;
        num_jumps = 0;

        currJumpForce = jumpForce;

        groundedFilter = LayerMask.GetMask("Platforms");

        spriteWidth = (float)this.GetComponent<SpriteRenderer>().bounds.size.x;
        collHeight = (float)this.GetComponent<Collider2D>().bounds.size.y / 2;
        widthOffset = new Vector3(spriteWidth/2 - 2.0f, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        //Horizontal Movement
        direction_KB = getDirFromAxis("Horiz_KB");
        direction_CTRL = getDirFromAxis("Horiz_CTRL");

        Move(direction_KB);
        Move(direction_CTRL);

        if(Input.GetButtonDown("Jump_KB") || Input.GetButtonDown("Jump_CTRL"))
        {
            Jump();
        }

        if(Input.GetAxisRaw("Glide_KB") > 0 || Input.GetAxisRaw("Glide_CTRL") > 0)
        {
            Glide();
        }

        if (Input.GetButtonDown("Dash_KB"))
        {
            Dash(direction_KB);
        }

        if (Input.GetButtonDown("Dash_CTRL"))
        {
            Dash(direction_CTRL);
        }

        //If I let go of glide button, we should stop gliding for this frame
/*        if (Input.GetAxisRaw("Glide_KB") == 0 || Input.GetAxisRaw("Glide_CTRL") == 0)
        {
            isGliding = false;
        }*/

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
        if(!isGrounded())
        {
            playerRB.AddForce(currentWindForce);
            startGlideTimer = true;
        }
        //If we are set the jumps we have available to 1 and reset our glide gravity
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

    private void Jump()
    {
        if(num_jumps > 0)
        {
            //set the upwards velocity to 0 so we don't have additive jump, then add the force
            playerRB.velocity = Vector2.zero;
            //this is here so that the player is not instantly marked grounded again and able to triple jump
            //may want to make this better in the future? Clever workaround solution for now
            this.transform.position += jumpNudge;
            playerRB.AddForce(Vector2.up * currJumpForce, ForceMode2D.Impulse);
            canDoubleJump = true;
            num_jumps-=1;
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
            // if we're already gliding, curve our glide fall speed
            if(isGliding)
            {
                if(glideGravAmt <= gravAmt)
                    glideGravAmt += glideCurveModifier;
            }
            //Otherwise set up the glide
            else if (!isGrounded() && !isDashing && canGlide)
            {
                //if we just started gliding, zero out our velocity so we stop jumping as soon as we start the glide
                if (!isGliding)
                {
                    playerRB.velocity = Vector2.zero;
                }
                isGliding = true;
            }

            //Lower gravity, mark us as gliding
            playerRB.AddForce(currentWindForce * windGlideModifier);
            playerRB.gravityScale = glideGravAmt;
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

    private bool isGrounded()
    {
        RaycastHit2D hitCenter = Physics2D.Raycast(transform.position, Vector2.down, collHeight, groundedFilter);
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position - widthOffset, Vector2.down, collHeight, groundedFilter);
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position + widthOffset, Vector2.down, collHeight, groundedFilter);

        //DEBUG stuff for my own sanity. Please do not delete until everything is done
        /*Debug.DrawRay(transform.position + widthOffset, Vector2.down * collHeight, Color.blue);
        Debug.DrawRay(transform.position, Vector2.down * collHeight, Color.blue);
        Debug.DrawRay(transform.position - widthOffset, Vector2.down * collHeight, Color.blue);*/
        if ((hitCenter.collider != null && hitCenter.collider.gameObject.tag.Contains("Platform")) || (hitLeft.collider != null && hitLeft.collider.gameObject.tag.Contains("Platform")) || (hitRight.collider != null && hitRight.collider.gameObject.tag.Contains("Platform")))
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
            if (Input.GetKey(KeyCode.W))
            {
                playerRB.gravityScale = 0;
                this.transform.position += climbVector;
                isClimbing = true;
            }
        }

        if(col.gameObject.CompareTag("Checkpoint"))
        {
            OnTrigger("Checkpoint", col.gameObject);
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
            isClimbing = false;
        }
        if(col.gameObject.CompareTag("Wind"))
        {
            inWind = false;
            currentWindForce = Vector2.zero;
        }
    }
    #endregion

    public void setJumpForce(float newForce)
    {
        currJumpForce = newForce;
    }

    public float getDefaultJumpForce()
    {
        return jumpForce;
    }

    public void unlockAbility(int ability)
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
