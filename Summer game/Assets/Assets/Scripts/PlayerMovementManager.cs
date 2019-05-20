using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementManager : MonoBehaviour
{
    /// <summary>
    /// Controls anything to do with the player moving
    /// </summary>

    public PlayerInput playerInput;

    public LayerMask playerMask;

    [SerializeField]private float moveForce;
    [SerializeField]private float jumpForce;
    [SerializeField]private float dashForce;
    [SerializeField]private float knockbackForce;

    private bool canDoubleJump = false;

    private float dashTimer = 5f;

    //This will be synced to the animation later
    private float placeholderDashDuration = 1f;

    private Vector2 currentPosition = new Vector2();
    private Vector2 moveVelocity;

    public bool isGrounded = true;
    public bool dashUsed = false;
    public bool isDashing = false;

    private Transform tagGround;

    private Rigidbody2D playerRigidbody;

    // Use this for initialization
    void Start ()
    {
        //Populating Variables
        playerRigidbody = this.transform.parent.GetComponent<Rigidbody2D>();
        tagGround = GameObject.Find(playerRigidbody.gameObject.name + "/tag_ground").transform;
    }

    private void FixedUpdate()
    {
        /**
         * Checks if player is grounded based on a linecast from the player to
         * the object tagGround, ignoreing the player object in the linecast.
         * Any object that needs to be ignored can be added via PLayerMask in
         * the unity editor
        **/
        isGrounded = Physics2D.Linecast(currentPosition, tagGround.position, playerMask);

        //Adds an extra downward force to make jumping less floaty
        if(!isGrounded && !isDashing)
        {
            playerRigidbody.AddForce(Vector2.down * 2.0f, ForceMode2D.Impulse);
        }

        //Calls move regularly 
        Move(Input.GetAxis("Horizontal"));
    }

    // Update is called once per frame
    void Update ()
    {
        //Again, all of this is placeholder since I don't have animations to work with right now
        if(isDashing)
        {
            placeholderDashDuration -= Time.deltaTime;
        }
        if(placeholderDashDuration < 0)
        {
            isDashing = false;
            placeholderDashDuration = 1f;
        }
        //End placeholder section

        //Dash cooldown timer
        if(dashUsed)
        {
            dashTimer -= Time.deltaTime;
        }
        if (dashTimer <= 0)
        {
            dashUsed = false;
            dashTimer = 5f;
        }

        //Grabs the current position of player
        currentPosition = GetComponentInParent<Transform>().transform.position;
	}

    #region Movement Methods

    /// <summary>
    /// Moves player based on @horizInput (for keyboard either moving or stationary, 
    /// for controller can move at varying speed). Called regularly/
    /// </summary>
    /// <param name="horizInput"></param>
    public void Move(float horizInput)
    {
        if(!isDashing)
        {
            moveVelocity = playerRigidbody.velocity;
            moveVelocity.x = horizInput * moveForce;

            playerRigidbody.velocity = moveVelocity;
        }
    }

    /// <summary>
    /// Player jumps, also handles double jmping
    /// </summary>
    public void Jump()
    {
        if (isGrounded)
        {
            playerRigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
            canDoubleJump = true;
        }
        else
        {
            if (canDoubleJump)
            {
                canDoubleJump = false;
                playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, 0);
                playerRigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
        }
    }

    /// <summary>
    /// Moves player rapidly forwards in the direction they were facing
    /// @TODO: Sync this with animations, very reliant on them
    /// </summary>
    /// <param name="horizInput"></param>
    public void Dash(float horizInput)
    {
        if(!dashUsed && playerRigidbody.velocity.magnitude != 0)
        {
            isDashing = true;
            Debug.Log("Dash");
            horizInput = Mathf.Round(horizInput);
            if(horizInput < 0)
            {
                if(!isGrounded)
                {
                    
                }
                else
                {
                    playerRigidbody.AddForce(Vector2.left * dashForce, ForceMode2D.Impulse);
                }
            }
            else
            {
                playerRigidbody.AddForce(Vector2.right * dashForce, ForceMode2D.Impulse);
            }

            dashUsed = true;
        }
    }

    /// <summary>
    /// Knocks the player back away from the thing they hit using @direction.
    /// @TODO: WIP and may become depricated
    /// </summary>
    /// <param name="direction"></param>
    public void DamageKnockback(Vector2 direction)
    {
        Debug.Log("Called");
        playerRigidbody.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
    }

#endregion
}
