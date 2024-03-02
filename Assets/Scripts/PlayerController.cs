using System;
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
    public PlayerState state;
    private bool isInvincible = false;
    private bool isFalling = false;
    private bool superSpeedEnabled = false;

    // Inspector variables
    [SerializeField] public LayerMask ground;
    [SerializeField] private LayerMask rock;
    [SerializeField] private int defeatEnemyScore = 100;
    [SerializeField] private AudioSource footstep;
    [SerializeField] private AudioSource bounceSound;
    [SerializeField] private AudioSource spikesSound;
    [SerializeField] private AudioSource powerupSound;
    [SerializeField] private GameObject shield;
    [SerializeField] public PlayerMovement movementScript;
    [SerializeField] private PlayerHealth healthScript;    
	[SerializeField] private PlayerInput inputScript;
    public bool isTouchingGround { get { return coll.IsTouchingLayers(ground); } }
	public bool isHurt { get { return state == PlayerState.hurt; } }
	public static event Action<int> OnGemCollected;
	public static event Action<int> OnLifeCollected;
	public static event Action<int> OnEnemyDefeated;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<CircleCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
		state = PlayerState.idle;
    }

    private void Start()
    {
        AudioSource[] allAudios = Camera.main.gameObject.GetComponents<AudioSource>();
        mainAudio = allAudios[0];
        invincibleAudio = allAudios[1];
    }

    private void Update()
    {
        AnimationState();
    }

    private void AnimationState()
    {
        anim.SetInteger("state", (int)state);

        if (state == PlayerState.jumping)
        {
            if (rb.velocity.y < .1f)
            {
                state = PlayerState.falling;
                isFalling = true;
            }
        }
        else if (state == PlayerState.falling)
        {
            if (isTouchingGround)
            {
                state = PlayerState.idle;
                isFalling = false;
            }
        }
        else if (state == PlayerState.hurt)
        {
            StartCoroutine(healthScript.ResumeIdleAfterHurt());
        }
        else if (Mathf.Abs(rb.velocity.x) > 2f)
        {
            if (coll.IsTouchingLayers(rock))
            {
                state = PlayerState.push;
            }
            else
            {
                state = PlayerState.running;
            }
        }
        else if (state == PlayerState.push)
        {
            if (Mathf.Abs(rb.velocity.x) < 2f)
            {
                state = PlayerState.idle;
            }
        }
        else if (!movementScript.hasMoved)
        {
            state = PlayerState.waiting;
        }
        else if (state == PlayerState.crouching)
        {
            state = PlayerState.crouching;
        }
        else
        {
            state = PlayerState.idle;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Gem")
        {
            CollectGem(collision.gameObject);
        }
        if(collision.tag == "Spikes")
        {
            HandleSpikesCollison();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            HandleEnemyCollision(other.gameObject);
        }

        if (other.gameObject.tag == "BoxPowerupInvincibility")
        {
            if (isFalling)
            {
                CollectPowerup(other.gameObject);

                StartCoroutine(StartInvinciblePowerup());
            }
        }

        if (other.gameObject.tag == "BoxPowerupGems")
        {
            if (isFalling)
            {
                CollectPowerup(other.gameObject);
				OnGemCollected?.Invoke(10);
            }
        }

        if (other.gameObject.tag == "BoxPowerupSneakers")
        {
            if (isFalling)
            {
                CollectPowerup(other.gameObject);

                StartSneakerPowerup();
            }
        }

        if (other.gameObject.tag == "BoxPowerupShield")
        {
            if (isFalling)
            {
                CollectPowerup(other.gameObject);

                shield.SetActive(true);
            }
        }

        if (other.gameObject.tag == "BoxPowerupLife")
        {
            if (isFalling)
            {
                CollectPowerup(other.gameObject);
				OnLifeCollected?.Invoke(1);
            }
        }

        if (other.gameObject.tag == "Bounce")
        {
            HandleBounce();
        }

        if (other.gameObject.tag == "Gem" && !healthScript.preventDamage)
        {
            CollectGem(other.gameObject);
        }
    }


    private void CollectGem(GameObject gemObject)
    {
        if (gemObject != null)
        {
            Gem gem = gemObject.GetComponent<Gem>();
            gem.Collected();
			OnGemCollected?.Invoke(1);
        }
    }

    private void CollectPowerup(GameObject powerupObject)
    {
        if (powerupObject != null)
        {
            BoxPowerup box = powerupObject.GetComponent<BoxPowerup>();
            box.Collected();
            movementScript.Jump();
        }

    }

    private void HandleSpikesCollison()
    {
        if (!isInvincible) {
            // Play feedback sound
            spikesSound.Play();

            // Jump up a tiny bit
            movementScript.Jump(5f);

            // Player is hurt
            healthScript.DecreaseHealth();
        }
    }

    private void HandleBounce()
    {
        // Play feedback sound
        bounceSound.Play();

        // Super jump
        movementScript.Jump(45f);
    }

    private void HandleEnemyCollision(GameObject enemyObject)
    {
        if (isInvincible || isFalling)
        {
            DefeatEnemy(enemyObject);
        }
        else
        {
            PlayerHurt(enemyObject);
        }
    }

    private void PlayerHurt(GameObject enemyObject)
    {
        // Player gets hurt
        healthScript.DecreaseHealth();
        movementScript.KnockPlayerBack(enemyObject);
    }

    private void DefeatEnemy(GameObject enemyObject)
    {
        Enemy enemy = enemyObject.GetComponent<Enemy>();

        // jump up
        if (isFalling)
            movementScript.Jump();

        OnEnemyDefeated?.Invoke(defeatEnemyScore);

        // the enemy is defeated
        enemy.JumpedOn();

    }

    private void PlayFootstep()
    {
        footstep.Play();
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

        if (!isInvincible)
        {   // Change main music
            mainAudio.Stop(); // stop main audio
            invincibleAudio.Play(); // play invincible power up audio

            // Display particle system
            invincibleParticles = GetComponentInChildren<ParticleSystem>();
            invincibleParticles.Play();

            isInvincible = true;
        }

    }

    private void StartSneakerPowerup()
    {
        StartCoroutine(ResetSneakerPowerup());

        if (!superSpeedEnabled)
        {
            superSpeedEnabled = true;
            movementScript.speed = 15f;
            // Increase pitch of music
            mainAudio.pitch = 1.5f;
        }
    }

    private IEnumerator ResetSneakerPowerup()
    {
        yield return new WaitForSeconds(15f);

        // Reset speed
        superSpeedEnabled = false;
        mainAudio.pitch = 1;
        movementScript.speed = 5f;
    }

}
