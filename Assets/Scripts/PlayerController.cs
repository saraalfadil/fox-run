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
    public SpriteRenderer sprite;
    private AudioSource mainAudio;
    private AudioSource invincibleAudio;
    // FSM
    public PlayerState state = PlayerState.idle;
    private bool isInvincible = false;
    private bool superSpeedEnabled = false;

    // Inspector variables
    [SerializeField] public LayerMask ground;
    [SerializeField] private LayerMask rock;
    [SerializeField] public float hurtForce = 10f;
    [SerializeField] private int defeatEnemyScore = 100;
    [SerializeField] private AudioSource footstep;
    [SerializeField] private AudioSource bounceSound;
    [SerializeField] private AudioSource spikesSound;
    [SerializeField] private AudioSource powerupSound;
    [SerializeField] private GameObject shield;
    [SerializeField] private PlayerMovement movementScript;
    [SerializeField] private PlayerDamage damageScript;
    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<CircleCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Start() {

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
                damageScript.DecreaseHealth();
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
                damageScript.DecreaseHealth();
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

        if(other.gameObject.tag == "Gem" && !damageScript.preventDamage)
        {
            Gem gem = other.gameObject.GetComponent<Gem>();
            gem.Collected();
        }
    }

    private void IncreaseScore()
    {
        PermanentUI.perm.score += defeatEnemyScore;
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
            StartCoroutine(damageScript.ResumeIdleAfterHurt());
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

