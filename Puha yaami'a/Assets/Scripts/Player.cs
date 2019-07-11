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
    private int direction;

    [SerializeField]private bool isGrounded, isDashing, isGliding, isClimbing, inWind;
    private bool canGlide = false;
    private bool startGlideTimer = false;
    private bool canDoubleJump = false;

    private Vector2 currentWindForce = Vector2.zero;

    private Vector3 moveVector, climbVector;

    [SerializeField] private bool dblJumpUnlocked = false;
    [SerializeField] private bool dashUnlocked = false;
    [SerializeField] private bool glideUnlocked = false;

    private Rigidbody2D playerRB;

    private float dashTime;
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
        glideGravAmt = gravAmt * glideGravModifier;
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

        if(Input.GetKey(KeyCode.D) && !isDashing)
        {
            //If we're gliding, modify our movement to be a little faster
            if (isGliding)
                this.transform.position += moveVector * glideMoveModifier;
            else
                this.transform.position += moveVector;
            direction = 1;
        }
        else if (Input.GetKey(KeyCode.A) && !isDashing)
        {
            //If we're gliding, modify our movement to be a little faster
            if (isGliding)
                this.transform.position -= moveVector * glideMoveModifier;
            else
                this.transform.position -= moveVector;
            direction = -1;
        }

        //Jump
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(isGrounded)
            {
                playerRB.AddForce(Vector2.up * currJumpForce, ForceMode2D.Impulse);  
                canDoubleJump = true;         
            }
            else
            {
                if(dblJumpUnlocked && canDoubleJump)
                {
                    canDoubleJump = false;
                    playerRB.AddForce(Vector2.up * currJumpForce, ForceMode2D.Impulse);  
                }
            }

        }

        //If I let go of Q, we should stop gliding for this frame
        if (Input.GetKeyUp(KeyCode.Q) && isGliding)
        {
            isGliding = false;
        }

        //Glide, only if not dashing or on the ground
        if(glideUnlocked)
        {
            if (Input.GetKey(KeyCode.Q) && !isGrounded && !isDashing && canGlide)
            {
                //if we just started gliding, zero out our velocity so we stop jumping as soon as we start the glide
                if(!isGliding)
                {
                    playerRB.velocity = Vector2.zero;
                }
                //Lower gravity, mark us as gliding
                playerRB.AddForce(currentWindForce * windGlideModifier);
                playerRB.gravityScale = glideGravAmt;
                isGliding = true;
            }
        }

        //Dash
        if(dashUnlocked)
        {
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

        //if we arent grounded we can look to glide and apply wind force
        if(!isGrounded)
        {
            playerRB.AddForce(currentWindForce);
            startGlideTimer = true;
        }    
    }
        #endregion

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

        if (col.gameObject.CompareTag("Score_Loss"))
        {
            OnCollide(4);
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

    // /// <summary>
    // /// Handles fail state stuff
    // /// </summary>
    // private void OnFail()
    // {
    //     transform.position = new Vector2(10.72f, 1.95f);
    // }

}
