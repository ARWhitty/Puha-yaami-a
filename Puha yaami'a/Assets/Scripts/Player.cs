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
    [Tooltip("List of Any Triggers created in the Animation Window")]
    [SerializeField] private List<string> animTriggers;
    [Tooltip("List of Any bools created in the Animation Window")]
    [SerializeField] private List<string> animbools;
    [Tooltip("here for tailoring when to start the land animation")]
    [SerializeField] private float landAnimOffset;
    [Tooltip("here for tailoring when to start the Glide End animation")]
    [SerializeField] private float glideEndAnimOffset;
    [Tooltip("The speed at which the background parallax moves")]
    [SerializeField] private float parallaxSpeed;

    [Tooltip("Give me the Paralax object in the scene :)")]
    public FreeParallax backgroundParallax;

    public Camera camera;

    #endregion

    #region Events
    //0 is normal, 1 is bouncy, 2 is sticky, 3 is death, 4 is score loss
    public delegate void PlatformCollisions(int type);
    public static event PlatformCollisions OnCollide;

    public delegate void TriggerPassthroughs(string type, Collider2D obj);
    public static event TriggerPassthroughs OnTrigger;
    #endregion

    #region Internal Fields
    private int num_jumps;
    private int prev_dir = 1;

    [SerializeField]private bool isDashing, isGliding, isClimbing, inWind, startDashCd, canDash, onLadder, isJumping;
    private bool canGlide = false;
    private bool startGlideTimer = false;

    private Vector2 currentWindForce = Vector2.zero;

    private Vector3 moveVector, climbVector, widthOffset, jumpNudge;

    [SerializeField] private bool dblJumpUnlocked = false;
    [SerializeField] private bool dashUnlocked = false;
    [SerializeField] private bool glideUnlocked = false;
    private bool isGroundedInternal;

    private Rigidbody2D playerRB;
    private Animator playerAnim;

    private float dashTime;
    private float currJumpForce;
    private float glideGravAmt;
    private float glideDelayTimerCount;
    private float dashCd;

    private float spriteWidth;
    private float collHeight;
    private float jumpTimerCount;
    private int currDirection = 0;

    private bool isColliding;

    private LayerMask groundedFilter;

    private SpriteRenderer playerSprite;
    private Vector2 lastFrameCameraPos;
    #endregion

    #region Start/Update/Enable

    // Start is called before the first frame update
    void Start()
    {
        moveVector = new Vector3(moveAmount, 0f);
        climbVector = new Vector3(0f, ladderClimbSpeed);
        //TODO: calculate based on sprite size
        jumpNudge = new Vector3(0f, 1.0f);

        playerRB = this.GetComponent<Rigidbody2D>();
        playerRB.gravityScale = gravAmt;
        glideGravAmt = gravAmt * glideGravModifier;
        glideDelayTimerCount = glideDelayTimer;

        playerAnim = this.GetComponent<Animator>();

        isDashing = false;
        isGliding = false;
        isClimbing = false;
        startDashCd = false;
        isJumping = false;

        dashTime = startDashTime;
        dashCd = dashCooldown;
        canDash = true;

        num_jumps = 0;

        currJumpForce = jumpForce;

        groundedFilter = LayerMask.GetMask("Level");
        playerSprite = this.GetComponent<SpriteRenderer>();
        BoxCollider2D playerCol = this.GetComponent<BoxCollider2D>();
        collHeight = (float)playerCol.bounds.size.y / 2;
        //note: can add a float offset after dividing by 2 here for more leeway on jumps
        widthOffset = new Vector3(playerCol.bounds.size.x/2, 0, 0);

        jumpTimerCount = jumpTimer;

        ResetAllAnimTriggers("");
    }

    private void FixedUpdate()
    {
        if(!PauseMenu.gamePaused)
        {
            //Horizontal Movement
            currDirection = GetDirFromAxis("Horizontal");
            Move(currDirection);
            lastFrameCameraPos = camera.transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!PauseMenu.gamePaused)
        {
            //Jump
            if (Input.GetButtonDown("Jump"))
            {
                Jump();
            }
            //Jump Button Held
            if (Input.GetButton("Jump"))
            {
                ContinueJump();
            }
            //Release of Jump buttong
            if (Input.GetButtonUp("Jump"))
            {
                isJumping = false;
            }

            //Glide begin
            if (Input.GetButtonDown("Glide"))
            {
                Glide();
            }
            //Glide End
            if (Input.GetButtonUp("Glide"))
            {
                isGliding = false;
            }
            // if we're already gliding, curve our glide fall speed
            if (isGliding)
            {
                playerAnim.SetBool("gliding", true);
                if (glideGravAmt <= gravAmt)
                    glideGravAmt += glideCurveModifier;

                //Lower gravity, mark us as gliding
                playerRB.AddForce(currentWindForce * windGlideModifier);
                playerRB.gravityScale = glideGravAmt;
            }
            else
            {
                playerAnim.SetBool("gliding", false);
            }

            //Dash
            if (Input.GetButtonDown("Dash"))
            {
                Dash(currDirection);
            }

            //Climb
            if (Input.GetAxisRaw("Vertical") != 0)
            {
                Climb(GetDirFromAxis("Vertical"));
            }

            //if we're not dashing/gliding/climbing, turn gravity back on pls
            if (!isDashing && !isGliding && !isClimbing)
            {
                playerRB.gravityScale = gravAmt;
                glideGravAmt = gravAmt * glideGravModifier;
            }

            //If we are, decrease our timer
            if (isDashing)
            {
                playerAnim.SetBool("dashing", true);
                //decrease dash time
                dashTime -= Time.deltaTime;
                if (dashTime <= 0)
                {
                    playerAnim.SetBool("dashing", false);
                    dashTime = startDashTime;
                    playerRB.velocity = Vector2.zero;
                    isDashing = false;
                    startDashCd = true;
                    canDash = false;
                }
            }
            //If we are in the middle of a jump, start our glide timer so we delay when we can glide
            if (startGlideTimer)
            {
                glideDelayTimerCount -= Time.deltaTime;
                if (glideDelayTimerCount <= 0)
                {
                    glideDelayTimerCount = glideDelayTimer;
                    canGlide = true;
                    startGlideTimer = false;
                }
            }

            //Cooldown between dashes
            if (startDashCd)
            {
                dashCd -= Time.deltaTime;
                if (dashCd <= 0)
                {
                    dashCd = dashCooldown;
                    startDashCd = false;
                    canDash = true;
                }
            }

            //internal bool for small efficiency gain
            isGroundedInternal = IsGrounded();
            //if we arent grounded we can look to glide and apply wind force
            if (!isGroundedInternal)
            {
                if(num_jumps != 0)
                {
                    num_jumps = 1;
                }
                //if we aren't gliding set that we should loop the air animation
                if (!isGliding)
                {
                    playerAnim.SetBool("airLoop", true);
                }
                playerRB.AddForce(currentWindForce);
                startGlideTimer = true;

                if (!isGliding && EndAirAnim())
                {
                    ResetAllAnimTriggers("airLoop");
                    playerAnim.SetTrigger("Fall");
                }
                else if (isGliding && EndGlideAnim())
                {
                    ResetAllAnimTriggers("");
                    playerAnim.SetBool("glide", false);
                }
            }
            //If we are set the jumps we have available to our maximum
            else
            {
                playerAnim.SetBool("airLoop", false);
                num_jumps = GetMaxJumps();
            }
        }
    }
    #endregion

    #region Mechanical Methods
    /// <summary>
    /// Retrieves the current direction the player should move from the horizontal axis, and faces the player sprite accordingly
    /// </summary>
    /// <param name="axisName"></param>
    /// <returns>1 for right, -1 for left, 0 for no movement</returns>
    private int GetDirFromAxis(string axisName)
    {
        float axisAmt = Input.GetAxisRaw(axisName);
        if (axisAmt > 0)
        {
            prev_dir = 1;
            playerSprite.flipX = false;
            return 1;
        }
        if (axisAmt < 0)
        {
            prev_dir = -1;
            playerSprite.flipX = true;
            return -1;
        }

        if(prev_dir == 1)
        {
            playerSprite.flipX = false;
        }
        else
        {
            playerSprite.flipX = true;
        }
        return 0;
    }

    /// <summary>
    /// Moves the player in the given direction via vector translation
    /// </summary>
    /// <param name="dir">Direction to move (1 is right, -1 is left, 0 is no movpement)</param>
    private void Move(int dir)
    { 
        if(dir != 0 && isGroundedInternal)
        {
            ResetAllAnimTriggers("Run");
            playerAnim.SetTrigger("Run");
        }
        else if(isGroundedInternal)
        {
            ResetAllAnimTriggers("Idle");
            playerAnim.SetTrigger("Idle");
        }
        if(!isDashing)
        {
            //If we're gliding, modify our movement to be a little faster
            if (isGliding)
                this.transform.position += moveVector * glideMoveModifier * dir;
            //otherwise just move along the vector
            else
                this.transform.position += moveVector * dir;
        }

        float cameraOffset = camera.transform.position.x - lastFrameCameraPos.x;

        if(cameraOffset >= moveAmount || cameraOffset <= -moveAmount)
        {
            backgroundParallax.Speed = parallaxSpeed * dir;
        }
        else
        {
            backgroundParallax.Speed = 0;
        }
    }

    /// <summary>
    /// Translates the player upwards with the designer-defined jump force, marks the player as jumping, and plays the correct animation for jump/double jump
    /// </summary>
    private void Jump()
    {
        //Debug.Break();
        if (num_jumps > 0)
        {
            if (num_jumps == 2)
            {
                ResetAllAnimTriggers("JumpStart");
                playerAnim.SetTrigger("JumpStart");
            }
            else
            {
                ResetAllAnimTriggers("DoubleJumpStart");
                playerAnim.SetTrigger("DoubleJumpStart");
            }

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

    /// <summary>
    /// Continues to translate the player upwards while the jump button is held, until a certain thrshold
    /// </summary>
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

    /// <summary>
    /// Returns the macimum number of jumps the player currently has access to
    /// </summary>
    /// <returns>2 if double jump is unlocked, 1 otherwise</returns>
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

    /// <summary>
    /// Translates the player horizontally further than a normal move command while also slowing descent speed
    /// </summary>
    private void Glide()
    {
        if (glideUnlocked)
        {
            //set up glide if necessary
            if (!IsGrounded() && !isDashing && canGlide)
            {
                ResetAllAnimTriggers("Glide");
                playerAnim.SetTrigger("Glide");
                //if we just started gliding, zero out our velocity so we stop jumping as soon as we start the glide
                if (!isGliding)
                {
                    playerRB.velocity = Vector2.zero;
                }
                isGliding = true;
            }
        }
    }

    /// <summary>
    /// Translates the player forward quickly in the given direction without gravity
    /// </summary>
    /// <param name="dir">The direction to dash</param>
    private void Dash(int dir)
    {
        if (dashUnlocked)
        {
            if (canDash && !isDashing && dir != 0)
            {
                ResetAllAnimTriggers("DashStart");
                playerAnim.SetTrigger("DashStart");
                isDashing = true;
                //set the gravity scale to 0 so we get a straight midair dash if necessary

                playerRB.gravityScale = 0;
                playerRB.velocity = new Vector2(dir, 0) * dashSpeed;
            }
        }
    }

    /// <summary>
    /// Translates the player up a climbable object
    /// </summary>
    /// <param name="dir">The direction to climb, 1 for up, -1 for down</param>
    private void Climb(int dir)
    {
        if(onLadder)
        {
            ResetAllAnimTriggers("Climb");
            playerAnim.SetTrigger("Climb");
            playerRB.velocity = Vector2.zero;
            playerRB.gravityScale = 0;
            isClimbing = true;
            transform.position += climbVector * dir;
        }
    }

    /// <summary>
    /// Checks if a player is grounded by casting 3 rays downwards from the player
    /// Each ray is precisely half the height of the player collider plus a small offset, and are spaced so that a player may
    /// walk nicely on ledges.
    /// </summary>
    /// <returns></returns>
    private bool IsGrounded()
    {
        float heightOffset = 0.1f;
        RaycastHit2D hitCenter = Physics2D.Raycast(transform.position, Vector2.down, collHeight + heightOffset, groundedFilter);
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position - widthOffset, Vector2.down, collHeight + heightOffset, groundedFilter);
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position + widthOffset, Vector2.down, collHeight + heightOffset, groundedFilter);

        //DEBUG stuff for my own sanity. Please do not delete until everything is done
        Debug.DrawRay(transform.position + widthOffset, Vector2.down * collHeight, Color.red);
        Debug.DrawRay(transform.position, Vector2.down * collHeight, Color.red);
        Debug.DrawRay(transform.position - widthOffset, Vector2.down * collHeight, Color.red);
        if (hitCenter.collider != null|| hitLeft.collider != null || hitRight.collider != null)
        {
            return true;
        }
        return false;
    }

    private bool EndAirAnim()
    {
        float currVel = playerRB.velocity.y;
        //Debug.DrawRay(transform.position, Vector2.down * (collHeight + landAnimOffset), Color.red);
        RaycastHit2D hitGround = Physics2D.Raycast(transform.position, Vector2.down, collHeight + landAnimOffset, groundedFilter);
        if (currVel < 0f && hitGround.collider != null)
        {
            return true;
        }
        return false;
    }

    private bool EndGlideAnim()
    {
        //Debug.DrawRay(transform.position, Vector2.down * (collHeight + glideEndAnimOffset), Color.red);
        RaycastHit2D hitGround = Physics2D.Raycast(transform.position, Vector2.down, collHeight + glideEndAnimOffset, groundedFilter);
        if (hitGround.collider != null)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Resets all animation triggers for the player so that the correct animation may be played when necessaey.
    /// </summary>
    private void ResetAllAnimTriggers(string toExclude)
    {
        foreach (string trigger in animTriggers)
        {
            if(trigger != toExclude)
                playerAnim.ResetTrigger(trigger);
        }
        foreach(string animbool in animbools)
        {
            if(animbool != toExclude)
                playerAnim.SetBool(animbool, false);
        }
    }
    #endregion

    #region Collisions
    /// <summary>
    /// Handles Collision with all non-trigger objects, the Game Manager performs the corresponding function
    /// </summary>
    /// <param name="col">the collider encountered by the player</param>
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

    /// <summary>
    /// Handles collisions where the player stays in contact (mostly for boolean checks)
    /// </summary>
    /// <param name="col">the collider encountered by the player</param>
    void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.tag.Contains("Platform"))
        {
            isGliding = false;
            canGlide = false;
        }
    }

    /// <summary>
    /// Handles small mechanical changes based on varying types of platforms
    /// </summary>
    /// <param name="col">the collider encountered by the player</param>
    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Bouncy_Platform"))
            currJumpForce = jumpForce;
        if (col.gameObject.CompareTag("Sticky_Platform"))
            currJumpForce = jumpForce;
    }
    #endregion

    #region Triggers
    /// <summary>
    /// Handles score loss objects, sent to Game Manager
    /// </summary>
    /// <param name="col">the collider encountered by the player</param>
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Score_Loss"))
        {
            OnCollide(4);
        }
        if (col.gameObject.CompareTag("Checkpoint"))
        {
            OnTrigger("Checkpoint", col);
        }
    }

    /// <summary>
    /// Handles any triggers the player stays within for longer periods, specific functionality performed by corresponding
    /// Game Manager call
    /// </summary>
    /// <param name="col">the collider encountered by the player</param>
    void OnTriggerStay2D(Collider2D col)
    {
        if(col.gameObject.CompareTag("Climbable"))
        {
            onLadder = true;
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

    /// <summary>
    /// Handles exiting of triggers
    /// </summary>
    /// <param name="col">the collider encountered by the player</param>
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

    #region Accessors/Public Modifiers
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

    /// <summary>
    /// updates players jump force if necessary
    /// </summary>
    /// <param name="newForce">the force to change jumpforce to</param>
    public void SetJumpForce(float newForce)
    {
        currJumpForce = newForce;
    }

    /// <summary>
    /// Returns the player's standard jump force
    /// </summary>
    /// <returns>a flot of the players standard jump force</returns>
    public float GetDefaultJumpForce()
    {
        return jumpForce;
    }

    /// <summary>
    /// Flags a certain ability as unlocked for use from then onwards
    /// </summary>
    /// <param name="ability">The index number of the ability to unlock</param>
    public void UnlockAbility(int ability)
    {
        switch (ability)
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

    public bool GetGlideUnlocked()
    {
        return glideUnlocked;
    }

    public bool GetDblUnlocked()
    {
        return dblJumpUnlocked;
    }

    public bool GetDashUnlocked()
    {
        return dashUnlocked;
    }
    #endregion
}
