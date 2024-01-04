using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // Start() variables
    private Rigidbody2D rb;
    private Animator anim;
    private CircleCollider2D coll;
    private ParticleSystem part;

    // FSM
    private enum State { idle, running, jumping, falling, hurt, waiting };
    private State state = State.idle;
    private bool isInvincible = false;
    private bool hasMoved = true;
    private float _timer = 0f;
    private float _idle_duration = 6f;
    private bool isGameOver = false;

    // Inspector variables
    [SerializeField] private LayerMask ground;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float hurtForce = 10f;
    [SerializeField] private AudioSource footstep;
    [SerializeField] private AudioSource bounceSound;
    [SerializeField] private AudioSource spikesSound;
    [SerializeField] private AudioSource powerupSound;
    [SerializeField] private Canvas gameOver;
    private CanvasGroup gameOverCanvasGroup;

    float m_Timer;
    public float fadeDuration = 1f;
    public float displayImageDuration = 1f;

    public bool isJumping { get { return state == State.jumping; } }

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<CircleCollider2D>(); 
        Health();
    }

    private void Health()
    {
        PermanentUI.perm.healthStat.text = PermanentUI.perm.health.ToString();
    }

    // Update is called once per frame
    private void Update()
    {
        if(state != State.hurt) 
        {
            Movement();
        }
        AnimationState();
        anim.SetInteger("state", (int)state); // set animation based on Enumerator state
        PermanentUI.perm.cherryText.text = PermanentUI.perm.cherries.ToString();
        PermanentUI.perm.scoreText.text = PermanentUI.perm.score.ToString();

        TrackIdleTime();
    }

    private void TrackIdleTime() {

        // Keep track of elapsed time for idle animation
        _timer += Time.deltaTime;
        if (_timer >= _idle_duration) {
            _timer = 0f;
            hasMoved = false;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Collectable")
        {
            Cherry cherry = collision.gameObject.GetComponent<Cherry>();
            cherry.Collected();
        }
        if(collision.tag == "Powerup")
        {
            // Remove powerup object
            Destroy(collision.gameObject);
            
            // Limit powerup effect activation period
            StartCoroutine(ResetInvinsiblePowerUp());
            
            // Play feedback sound
            powerupSound.Play();

            if(!isInvincible) 
            {   // Change main music
                AudioSource[] allAudios = Camera.main.gameObject.GetComponents<AudioSource>();
                allAudios[0].Stop(); // stop main audio
                allAudios[1].Play(); // play invincible power up audio
            }

            // Display particle system 
            part = GetComponentInChildren<ParticleSystem>();
            part.Play();

            isInvincible = true;
        }
        if(collision.tag == "Spikes")
        {   

            if (!isInvincible) {
                // Play feedback sound
                spikesSound.Play();

                // Jump up a bit
                rb.velocity = new Vector2(rb.velocity.x, 5f);

                // Player is hurt
                state = State.hurt;
                HandleHealth();
            }

        }
    }

   private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Enemy")
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();

            // If player is jumping on top of enemy, the enemy is defeated
            if(state == State.falling)
            {
                enemy.JumpedOn();
                Jump();

                PermanentUI.perm.score += 100;
            }
            else 
            {   
                // If player is invincible, the enemy is defeated
                if (isInvincible)
                {
                    enemy.JumpedOn();
                    PermanentUI.perm.score += 100;
                }
                else 
                {   
                    // Player gets hurt
                    state = State.hurt;
                    HandleHealth();

                    // Enemy is to my right
                    if(enemy.transform.position.x > transform.position.x)
                    {
                        // I should be damaged and move left
                        rb.velocity = new Vector2(-hurtForce, rb.velocity.y);
                    }
                    else // Enemy is to my left 
                    {
                        // I should be damaged and move right
                        rb.velocity = new Vector2(hurtForce, rb.velocity.y);
                    }
                    
                }
            }


        }

        if(other.gameObject.tag == "BoxPowerup")
        {
            if(state == State.falling)
            {

                BoxPowerup box = other.gameObject.GetComponent<BoxPowerup>();
                box.Collected();
                rb.velocity = new Vector2(rb.velocity.x, 10);
                state = State.jumping;

                StartCoroutine(PowerupDelay());

            }
        }
    
        if(other.gameObject.tag == "BoxPowerupCherry")
        {
            if(state == State.falling)
            {

                BoxPowerup box = other.gameObject.GetComponent<BoxPowerup>();
                box.Collected();
                rb.velocity = new Vector2(rb.velocity.x, 10);
                state = State.jumping;

                PermanentUI.perm.cherries += 10;
            }
        }

        if(other.gameObject.tag == "Bounce")
        {

            // Play feedback sound
            bounceSound.Play();

            // Super jump
            rb.velocity = new Vector2(rb.velocity.x, 45f);
            state = State.jumping;
        
        }
    }


    private void HandleHealth()
    {

        PermanentUI.perm.healthStat.text = PermanentUI.perm.health.ToString();
        if(PermanentUI.perm.health <= 0)
        {   

            // Show game over
            if (!isGameOver)
                this.GameOver();
        } 
        else 
        {
            PermanentUI.perm.health -= 1;
        }
    }
    

    private void GameOver()
    {
        isGameOver = true;

        // stop main audio
        AudioSource[] allAudios = Camera.main.gameObject.GetComponents<AudioSource>();
        allAudios[0].Stop(); 

        // Play game over audio
        AudioSource gameOverAudio = gameOver.GetComponent<AudioSource>();
        gameOverAudio.Play();

        // Freeze time
        Time.timeScale = 0;

        // Display game over canvas overlay
        gameOverCanvasGroup = gameOver.GetComponent<CanvasGroup>();
        gameOverCanvasGroup.alpha = 1;

        StartCoroutine(GameOverReset());
    }

    private void Movement()
    {
        float hDirection = Input.GetAxis("Horizontal");
         
        // Moving left
        if(hDirection < 0)
        {
            rb.velocity = new Vector2(-speed, rb.velocity.y);
            transform.localScale = new Vector2(-1, 1);
            hasMoved = true;
            _timer = 0f;
        } 
        // Moving right
        else if(hDirection > 0)
        {
            rb.velocity = new Vector2(speed, rb.velocity.y);
            transform.localScale = new Vector2(1, 1);
            hasMoved = true;
            _timer = 0f;
        } 

        // Jumping
        if(Input.GetButtonDown("Jump") && coll.IsTouchingLayers(ground))
        {   
            Debug.Log("jumping");
            Jump();
            hasMoved = true;
            _timer = 0f;
        } 
        
    }

    public void Jump() 
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        state = State.jumping;
    }

    private void PlayFootstep() 
    {
        footstep.Play();
    }
    
    private void AnimationState()
    {
        if(state == State.jumping) 
        {
            if(rb.velocity.y < .1f)
            {
                state = State.falling;
            }
        }
        else if(state == State.falling)
        {
            if(coll.IsTouchingLayers(ground))
            {
                state = State.idle;
            }
        }
        else if(state == State.hurt)
        {
            if(Mathf.Abs(rb.velocity.x) < .1f)
            {
                state = State.idle;
            }
        }
        else if(Mathf.Abs(rb.velocity.x) > 2f)
        {
            // Moving
            state = State.running;
        }
        else if (!hasMoved) {
            state = State.waiting;
        }
        else
        {
            state = State.idle;
        }
    }

    private IEnumerator ResetInvinsiblePowerUp()
    {
        yield return new WaitForSeconds(20);

        isInvincible = false;

        /// Change main music back
        AudioSource[] allAudios = Camera.main.gameObject.GetComponents<AudioSource>();
        allAudios[1].Stop(); // stop invincible power up audio
        allAudios[0].Play(); // play main audio again
        
        // Stop invincible power up particle effect
        part.Stop();
    }

    private IEnumerator PowerupDelay()
    {
        yield return new WaitForSeconds(.60f);
        // Limit powerup effect activation period
        StartCoroutine(ResetInvinsiblePowerUp());
        
        // Play feedback sound
        //powerupSound.Play();

        if(!isInvincible) 
        {   // Change main music
            AudioSource[] allAudios = Camera.main.gameObject.GetComponents<AudioSource>();
            allAudios[0].Stop(); // stop main audio
            allAudios[1].Play(); // play invincible power up audio
        }

        // Display particle system 
        part = GetComponentInChildren<ParticleSystem>();
        part.Play();

        isInvincible = true;
    }

    private IEnumerator GameOverReset()
    {
        // Resume time
        float pauseEndTime = Time.realtimeSinceStartup + 6;
        while (Time.realtimeSinceStartup < pauseEndTime)
        {
            yield return 0;
        }
        Time.timeScale = 1;

        // Reset scene, health and cherries
        SceneManager.LoadScene("Menu");
        PermanentUI.perm.health = 5;
        PermanentUI.perm.cherries = 0;
        PermanentUI.perm.score = 0;
    }

}

