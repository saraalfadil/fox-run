using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] public float speed = 5f;
    [SerializeField] public float jumpForce = 20f;
    public bool hasMoved = true;
    private float idleTimer = 0f;
    private float idleDuration = 6f;
    public bool isJumping { get { return playerController.state == PlayerState.jumping; } }

    void Update()
    {

        if(playerController.state == PlayerState.hurt || PermanentUI.perm.endLevel) 
            return;

        float hDirection = Input.GetAxis("Horizontal");
        float virection = Input.GetAxis("Vertical");
         
        if(hDirection < 0)
        {
            MovePlayerLeft();
        } 
        else if(hDirection > 0)
        {
            MovePlayerRight();
        } 

        Debug.Log("virection: " + virection);
        
        if(virection < 0) {
            MovePlayerDown();
        }

        // Jumping
        if(Input.GetButtonDown("Jump") && playerController.coll.IsTouchingLayers(playerController.ground))
        {   
            Jump();
        } 

        TrackIdleTime();
    }

    public void Jump(float force = 0f) 
    {   
        force = force != 0f ? force : jumpForce;
        playerController.rb.velocity = new Vector2(playerController.rb.velocity.x, force);
        playerController.state = PlayerState.jumping;
        HasMoved();
    }

    private void MovePlayerLeft() 
    {
        playerController.rb.velocity = new Vector2(-speed, playerController.rb.velocity.y);
        transform.localScale = new Vector2(-1, 1);
        HasMoved();
    }

    private void MovePlayerRight()
    {
        playerController.rb.velocity = new Vector2(speed, playerController.rb.velocity.y);
        transform.localScale = new Vector2(1, 1);
        HasMoved();
    }

    private void MovePlayerDown() 
    {
        //playerController.rb.velocity = new Vector2(playerController.rb.velocity.x, -speed);
        //transform.localScale = new Vector2(-1, 1);
        playerController.state = PlayerState.crouching;
        HasMoved();
    }

    private void HasMoved()
    {
        hasMoved = true;
        idleTimer = 0f; // reset timer
    }
    
    private void TrackIdleTime() {

        // Keep track of elapsed time for idle animation
        idleTimer += Time.deltaTime;
        if (idleTimer >= idleDuration) {
            idleTimer = 0f;
            hasMoved = false;
        }

    }

    public void KnockPlayerBack(Enemy enemy)
    {
        // Enemy is to my right
        if(enemy.transform.position.x > transform.position.x)
        {
            // I should be damaged and move left
            playerController.rb.velocity = new Vector2(-playerController.hurtForce, 5);
        }
        else // Enemy is to my left 
        {
            // I should be damaged and move right
            playerController.rb.velocity = new Vector2(playerController.hurtForce, 5);
        }
    }
}
