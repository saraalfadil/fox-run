using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private CircleCollider2D coll;
    private ParticleSystem invincibleParticles;
    protected SpriteRenderer sprite;
    private AudioSource mainAudio;
    private AudioSource invincibleAudio;
    // FSM
    private enum State { idle, running, jumping, falling, hurt, waiting, push };
    private State state = State.idle;
    private bool isInvincible = false;
    private bool superSpeedEnabled = false;
    private bool hasMoved = true;
    private float idleTimer = 0f;
    private float idleDuration = 6f;
    private bool preventDamage = false;
    private float preventDamageTimer = 0f;
    private float preventDamageDuration = 3f;
    public bool isJumping { get { return state == State.jumping; } }
    private bool isFlashing = false;

    // Inspector variables
    [SerializeField] private LayerMask ground;
    [SerializeField] private LayerMask rock;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float hurtForce = 10f;
    [SerializeField] private int defeatEnemyScore = 100;
    [SerializeField] private AudioSource footstep;
    [SerializeField] private AudioSource bounceSound;
    [SerializeField] private AudioSource spikesSound;
    [SerializeField] private AudioSource powerupSound;
    [SerializeField] private CherryCollection cherryCollection;
    [SerializeField] private GameObject shield;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<CircleCollider2D>(); 
        sprite = GetComponent<SpriteRenderer>();
        cherryCollection = GetComponent<CherryCollection>();

        AudioSource[] allAudios = Camera.main.gameObject.GetComponents<AudioSource>();
        mainAudio = allAudios[0];
        invincibleAudio = allAudios[1];

    }

    private void GetStats()
    {
        PermanentUI.perm.healthStat.text = PermanentUI.perm.health.ToString();
        PermanentUI.perm.cherryText.text = PermanentUI.perm.cherries.ToString();
        PermanentUI.perm.scoreText.text = PermanentUI.perm.score.ToString();
    }

    private void Update()
    {
        if(state != State.hurt) 
        {
            Movement();
        }
        AnimationState();
        anim.SetInteger("state", (int)state); // set animation based on Enumerator state

        GetStats();

        TrackIdleTime();

        TrackTemporaryInvincibility();

    }

    private void TrackIdleTime() {

        // Keep track of elapsed time for idle animation
        idleTimer += Time.deltaTime;
        if (idleTimer >= idleDuration) {
            idleTimer = 0f;
            hasMoved = false;
        }

    }

    private void TrackTemporaryInvincibility() {

        // Keep track of temporary invincibility
        preventDamageTimer += Time.deltaTime;
        if (preventDamageTimer >= preventDamageDuration) {
            preventDamageTimer = 0f;
            preventDamage = false;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Collectable")
        {
            Cherry cherry = collision.gameObject.GetComponent<Cherry>();
            cherry.Collected();
        }
        if(collision.tag == "Spikes")
        {   

            if (!isInvincible) {
                // Play feedback sound
                spikesSound.Play();

                // Jump up a tiny bit
                Jump(5f);

                // Player is hurt
                DecreaseHealth();
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
                IncreaseScore();
            }
            else 
            {   
                // If player is invincible, the enemy is defeated
                if (isInvincible)
                {
                    enemy.JumpedOn();
                    IncreaseScore();
                }
                else 
                {   
                    // Player gets hurt
                    DecreaseHealth();
                    KnockPlayerBack(enemy);
                    
                }
            }


        }

        if(other.gameObject.tag == "BoxPowerupInvincibility")
        {   
            if(state == State.falling)
            {

                BoxPowerup box = other.gameObject.GetComponent<BoxPowerup>();
                box.Collected();
                Jump();

                StartCoroutine(StartInvinciblePowerup());

            }
        }
    
        if(other.gameObject.tag == "BoxPowerupCherries")
        {   
            if(state == State.falling)
            {

                BoxPowerup box = other.gameObject.GetComponent<BoxPowerup>();
                box.Collected();
                Jump();

                PermanentUI.perm.cherries += 10;
            }
        }

        if(other.gameObject.tag == "BoxPowerupSneakers")
        {   
            if(state == State.falling)
            {

                BoxPowerup box = other.gameObject.GetComponent<BoxPowerup>();
                box.Collected();
                Jump();

                StartCoroutine(StartSneakerPowerup());
            }
        }

        if(other.gameObject.tag == "BoxPowerupShield")
        {   
            if(state == State.falling)
            {

                BoxPowerup box = other.gameObject.GetComponent<BoxPowerup>();
                box.Collected();
                Jump();

                shield.SetActive(true);
            }
        }

        if(other.gameObject.tag == "Bounce")
        {

            // Play feedback sound
            bounceSound.Play();

            // Super jump
            Jump(45f);
        
        }

        if(other.gameObject.tag == "Collectable" && !preventDamage)
        {
            Cherry cherry = other.gameObject.GetComponent<Cherry>();
            cherry.Collected();
        }
    }

    private void IncreaseScore()
    {
        PermanentUI.perm.score += defeatEnemyScore;
    }

    private void DecreaseHealth()
    {

        // Player is temporarily protected from taking damage
        if(preventDamage)
            return;
        
        PermanentUI.perm.healthStat.text = PermanentUI.perm.health.ToString();
        if(PermanentUI.perm.health <= 0)
        {   
            // Show game over
            PermanentUI.perm.GameOver();
        } 
        else 
        {   
            preventDamage = true;
            state = State.hurt;

            // If shield isactive
            if(shield.activeSelf) {

                shield.SetActive(false);
                
            } else {
                // If player doesn't have any cherries, decrease health
                if(PermanentUI.perm.cherries < 1)
                    PermanentUI.perm.health -= 1;

                // Lose collected cherries
                int prevCherryCount = PermanentUI.perm.cherries;
                PermanentUI.perm.cherries = 0;
                Vector3 playerPosition = transform.position;
                cherryCollection.ScatterCherries(playerPosition, prevCherryCount);
            }


        }
        
    }

    private void KnockPlayerBack(Enemy enemy)
    {
        // Enemy is to my right
        if(enemy.transform.position.x > transform.position.x)
        {
            // I should be damaged and move left
            rb.velocity = new Vector2(-hurtForce, 5);
        }
        else // Enemy is to my left 
        {
            // I should be damaged and move right
            rb.velocity = new Vector2(hurtForce, 5);
        }
    }

    private void Movement()
    {
        float hDirection = Input.GetAxis("Horizontal");
         
        // Moving left
        if(hDirection < 0)
        {
            rb.velocity = new Vector2(-speed, rb.velocity.y);
            transform.localScale = new Vector2(-1, 1);
            HasMoved();
        } 
        // Moving right
        else if(hDirection > 0)
        {
            rb.velocity = new Vector2(speed, rb.velocity.y);
            transform.localScale = new Vector2(1, 1);
            HasMoved();
        } 

        // Jumping
        if(Input.GetButtonDown("Jump") && coll.IsTouchingLayers(ground))
        {   
            Jump();
        } 
        
    }

    private void HasMoved()
    {
        hasMoved = true;
        idleTimer = 0f; // reset timer
    }

    public void Jump(float force = 0f) 
    {   
        force = force != 0f ? force : jumpForce;
        rb.velocity = new Vector2(rb.velocity.x, force);
        state = State.jumping;
        HasMoved();
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
            StartCoroutine(ResumeIdleAfterHurt());
        }
        else if(Mathf.Abs(rb.velocity.x) > 2f)
        {
            if(coll.IsTouchingLayers(rock))
            {
                state = State.push;
            }
            else 
            {
                state = State.running;
            }   
        }
        else if(state == State.push)
        {
            if(Mathf.Abs(rb.velocity.x) < 2f)
            {
                state = State.idle;
            }
        }
        else if (!hasMoved) {
            state = State.waiting;
        }
        else
        {
            state = State.idle;
        }
    }

    private IEnumerator ResumeIdleAfterHurt()
    {
        StartPlayerFlashAnimation();
        yield return new WaitForSeconds(.5f);
        if(Mathf.Abs(rb.velocity.x) < .1f)
        {
            state = State.idle;
        }     
    }

    public void StartPlayerFlashAnimation()
    {
        if (!isFlashing)
        {
            StartCoroutine(PlayerFlash());
        }
    }

    private IEnumerator PlayerFlash()
    {
        isFlashing = true;

        float elapsedTime = 0f;
        float flashDuration = 3f;
        float flashSpeed = 5.0f;
        Color originalColor = sprite.color;

        while (elapsedTime < flashDuration)
        {
            float newColor = Mathf.PingPong(elapsedTime * flashSpeed, 1f);
            Color flashColor = new Color(originalColor.r, originalColor.g, originalColor.b, newColor);
            sprite.color = flashColor;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        sprite.color = originalColor;
        isFlashing = false;
        
    }
    private IEnumerator ResetInvinciblePowerUp()
    {
        yield return new WaitForSeconds(20);

        isInvincible = false;

        /// Change main music back
        invincibleAudio.Stop(); // stop invincible power up audio
        mainAudio.Play(); // play main audio again
        
        // Stop invincible power up particle effect
        invincibleParticles.Stop();
    }

    private IEnumerator StartInvinciblePowerup()
    {
        yield return new WaitForSeconds(.60f);
        // Limit powerup effect activation period
        StartCoroutine(ResetInvinciblePowerUp());
        
        if(!isInvincible) 
        {   // Change main music
            mainAudio.Stop(); // stop main audio
            invincibleAudio.Play(); // play invincible power up audio
        }

        // Display particle system 
        invincibleParticles = GetComponentInChildren<ParticleSystem>();
        invincibleParticles.Play();

        isInvincible = true;
    }

    private IEnumerator StartSneakerPowerup()
    {
        if(!superSpeedEnabled) 
        {  
            superSpeedEnabled = true;
            speed = 15f;
            // Increase pitch of music
            mainAudio.pitch = 1.5f;
        }

        yield return new WaitForSeconds(15f);

        // Reset speed
        superSpeedEnabled = false;
        mainAudio.pitch = 1;
        speed = 5f;
    }

}

