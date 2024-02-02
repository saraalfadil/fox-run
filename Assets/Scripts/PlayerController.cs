using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState { idle, running, jumping, falling, hurt, waiting, push, crouching };

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;
    private Animator anim;
    public CircleCollider2D coll;
    private ParticleSystem invincibleParticles;
    protected SpriteRenderer sprite;
    private AudioSource mainAudio;
    private AudioSource invincibleAudio;
    // FSM
    public PlayerState state = PlayerState.idle;
    private bool isInvincible = false;
    private bool superSpeedEnabled = false;
    private bool preventDamage = false;
    private float preventDamageTimer = 0f;
    private float preventDamageDuration = 3f;
    private bool isFlashing = false;

    // Inspector variables
    [SerializeField] public LayerMask ground;
    [SerializeField] private LayerMask rock;
    [SerializeField] public float hurtForce = 10f;
    [SerializeField] private int defeatEnemyScore = 100;
    [SerializeField] private AudioSource footstep;
    [SerializeField] private AudioSource bounceSound;
    [SerializeField] private AudioSource spikesSound;
    [SerializeField] private AudioSource powerupSound;
    [SerializeField] private GemCollection gemCollection;
    [SerializeField] private GameObject shield;
    [SerializeField] private PlayerMovement movementScript;
    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<CircleCollider2D>(); 
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Start() {
        gemCollection = GetComponent<GemCollection>();

        AudioSource[] allAudios = Camera.main.gameObject.GetComponents<AudioSource>();
        mainAudio = allAudios[0];
        invincibleAudio = allAudios[1];

    }

    private void GetStats()
    {
        PermanentUI.perm.healthStat.text = PermanentUI.perm.health.ToString();
        PermanentUI.perm.gemText.text = PermanentUI.perm.gems.ToString();
        PermanentUI.perm.scoreText.text = PermanentUI.perm.score.ToString();
    }

    private void Update()
    {
        AnimationState();
        anim.SetInteger("state", (int)state); // set animation based on Enumerator state

        GetStats();

        TrackTemporaryInvincibility();

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
        if(collision.tag == "Gem")
        {
            Gem gem = collision.gameObject.GetComponent<Gem>();
            gem.Collected();
        }
        if(collision.tag == "Spikes")
        {   

            if (!isInvincible) {
                // Play feedback sound
                spikesSound.Play();

                // Jump up a tiny bit
                movementScript.Jump(5f);

                // Player is hurt
                DecreaseHealth();
            }

        }
    }

    private void DefeatEnemy(Enemy enemy)
    {
        // jump up
        if(state == PlayerState.falling)    
            movementScript.Jump();

        IncreaseScore();

        // the enemy is defeated
        enemy.JumpedOn();

    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Enemy")
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();

            // If player is jumping on top of enemy, or if player is invincible
            if(isInvincible || state == PlayerState.falling)
            {   
                DefeatEnemy(enemy);
            }
            else 
            {   
                 
                // Player gets hurt
                DecreaseHealth();
                movementScript.KnockPlayerBack(enemy);
                
            }

        }

        if(other.gameObject.tag == "BoxPowerupInvincibility")
        {   
            if(state == PlayerState.falling)
            {

                BoxPowerup box = other.gameObject.GetComponent<BoxPowerup>();
                box.Collected();
                movementScript.Jump();

                StartCoroutine(StartInvinciblePowerup());

            }
        }
    
        if(other.gameObject.tag == "BoxPowerupGems")
        {   
            if(state == PlayerState.falling)
            {

                BoxPowerup box = other.gameObject.GetComponent<BoxPowerup>();
                box.Collected();
                movementScript.Jump();

                PermanentUI.perm.gems += 10;
            }
        }

        if(other.gameObject.tag == "BoxPowerupSneakers")
        {   
            if(state == PlayerState.falling)
            {

                BoxPowerup box = other.gameObject.GetComponent<BoxPowerup>();
                box.Collected();
                movementScript.Jump();

                StartCoroutine(StartSneakerPowerup());
            }
        }

        if(other.gameObject.tag == "BoxPowerupShield")
        {   
            if(state == PlayerState.falling)
            {

                BoxPowerup box = other.gameObject.GetComponent<BoxPowerup>();
                box.Collected();
                movementScript.Jump();

                shield.SetActive(true);
            }
        }

        if(other.gameObject.tag == "BoxPowerupLife")
        {   
            if(state == PlayerState.falling)
            {

                BoxPowerup box = other.gameObject.GetComponent<BoxPowerup>();
                box.Collected();
                movementScript.Jump();

                PermanentUI.perm.health += 1;
            }
        }

        if(other.gameObject.tag == "Bounce")
        {

            // Play feedback sound
            bounceSound.Play();

            // Super jump
            movementScript.Jump(45f);
        
        }

        if(other.gameObject.tag == "Gem" && !preventDamage)
        {
            Gem gem = other.gameObject.GetComponent<Gem>();
            gem.Collected();
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

            TakeDamage();

        }
        
    }

    private void TakeDamage() {

        if(preventDamage)
            return;
        
        preventDamage = true;
        state = PlayerState.hurt;

        // If shield is enabled
        // player doesn't take any damage
        if(shield.activeSelf) {

            // Disable shield
            shield.SetActive(false);
            
        } else {
            // If player doesn't have any gems, decrease health
            if(PermanentUI.perm.gems < 1)
                PermanentUI.perm.health -= 1;

            // Lose collected gems
            int collectedGems = PermanentUI.perm.gems;
            gemCollection.LoseGems(collectedGems);

            // Reset gems to 0 
            PermanentUI.perm.gems = 0;
        }

    }

    private void PlayFootstep() 
    {
        footstep.Play();
    }
    
    private void AnimationState()
    {

        if(state == PlayerState.jumping) 
        {
            if(rb.velocity.y < .1f)
            {
                state = PlayerState.falling;
            }
        }
        else if(state == PlayerState.falling)
        {
            if(coll.IsTouchingLayers(ground))
            {
                state = PlayerState.idle;
            }
        }
        else if(state == PlayerState.hurt)
        {
            StartCoroutine(ResumeIdleAfterHurt());
        }
        else if(Mathf.Abs(rb.velocity.x) > 2f)
        {
            if(coll.IsTouchingLayers(rock))
            {
                state = PlayerState.push;
            }
            else 
            {
                state = PlayerState.running;
            }   
        }
        else if(state == PlayerState.push)
        {
            if(Mathf.Abs(rb.velocity.x) < 2f)
            {
                state = PlayerState.idle;
            }
        }
        else if (!movementScript.hasMoved) {
            state = PlayerState.waiting;
        }
        else if (state == PlayerState.crouching) {
            state = PlayerState.crouching;
        }
        else
        {
            state = PlayerState.idle;
        }
    }

    private IEnumerator ResumeIdleAfterHurt()
    {
        StartPlayerFlashAnimation();
        yield return new WaitForSeconds(.5f);
        if(Mathf.Abs(rb.velocity.x) < .1f)
        {
            state = PlayerState.idle;
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
            movementScript.speed = 15f;
            // Increase pitch of music
            mainAudio.pitch = 1.5f;
        }

        yield return new WaitForSeconds(15f);

        // Reset speed
        superSpeedEnabled = false;
        mainAudio.pitch = 1;
        movementScript.speed = 5f;
    }

}

