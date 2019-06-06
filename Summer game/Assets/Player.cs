using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveAmount;
    [SerializeField] private float jumpForce;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float gravAmt;

    [SerializeField]private float dashTime;
    [SerializeField]private float startDashTime;

    private GameObject movingPlatform;
    private Vector3 mpOffset;

    //-1 is left, 1 is right, 0 is not moving
    private int direction;

    [SerializeField]private bool isGrounded, isDashing;

    private Vector3 moveVector;

    private Rigidbody2D playerRB;

    private float currJumpForce;

    // Start is called before the first frame update
    void Start()
    {
        moveVector = new Vector3(moveAmount, 0f);
        playerRB = this.GetComponent<Rigidbody2D>();
        playerRB.gravityScale = gravAmt;

        isGrounded = true;
        isDashing = false;

        dashTime = startDashTime;

        direction = 0;

        currJumpForce = jumpForce;

        movingPlatform = null;
    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetKey(KeyCode.D))
        {
            this.transform.position += moveVector;
            direction = 1;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            this.transform.position -= moveVector;
            direction = -1;
        }

        if(Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            playerRB.AddForce(Vector2.up * currJumpForce, ForceMode2D.Impulse);
        }

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

        //if we're not dashing, turn gravity back on pls
        if (isDashing == false)
        {
            playerRB.gravityScale = gravAmt;
        }
        else
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
       
    }

    void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.tag.Contains("Platform"))
            isGrounded = true;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Bouncy_Platform"))
            currJumpForce *= 1.5f;
        if (col.gameObject.CompareTag("Sticky_Platform"))
            currJumpForce *= 0.5f;
        if (col.gameObject.CompareTag("Fail_Platform"))
            OnFail();
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

    /// <summary>
    /// Handles fail state stuff
    /// </summary>
    private void OnFail()
    {
        transform.position = new Vector2(10.72f, 1.95f);
    }

}
