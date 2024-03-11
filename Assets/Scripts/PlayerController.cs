using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public static event Action<int> OnGemCollected;
	public static event Action<int> OnLifeCollected;
	public static event Action<Enemy> OnEnemyDefeated;
    public Rigidbody2D rb;
    public Animator anim;
    public CircleCollider2D coll;
    private ParticleSystem invincibleParticles;
    public SpriteRenderer sprite;
    private AudioSource mainAudio;
    private AudioSource invincibleAudio;
    public PlayerState state;
	public StateMachine playerStateMachine;
	public bool isTouchingGround { get { return coll.IsTouchingLayers(ground); } }
	private bool isInvincible = false;
    public bool isFalling = false;
	public bool isHurt = false;
    private bool superSpeedEnabled = false;
    [SerializeField] public LayerMask ground;
    [SerializeField] private LayerMask rock;
    [SerializeField] private int defeatEnemyScore = 100;
    [SerializeField] private AudioSource footstep;
    [SerializeField] private AudioSource bounceSound;
    [SerializeField] private AudioSource spikesSound;
    [SerializeField] private AudioSource powerupSound;
    [SerializeField] private GameObject shield;
    [SerializeField] public PlayerMovement movementScript;
    [SerializeField] public PlayerHealth healthScript;    
	[SerializeField] private PlayerInput inputScript;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<CircleCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
		playerStateMachine = new StateMachine(this);
    }

    private void Start()
    {
        AudioSource[] allAudios = Camera.main.gameObject.GetComponents<AudioSource>();
        mainAudio = allAudios[0];
        invincibleAudio = allAudios[1];

		playerStateMachine.Initialize(playerStateMachine.idleState);
    }

    private void Update()
    {
		playerStateMachine.Update();
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
        if (isInvincible) 
			return;

		// Play feedback sound
		spikesSound.Play();

		// Jump up a tiny bit
		movementScript.Jump(5f);

		// Player is hurt
		healthScript.DecreaseHealth();
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

        if (isFalling)
		{
			movementScript.Jump();
		}
            
		Enemy enemy = enemyObject.GetComponent<Enemy>();

        OnEnemyDefeated?.Invoke(enemy);

    }

    private void PlayFootstep()
    {
        footstep.Play();
    }

    private IEnumerator StartInvinciblePowerup()
    {
        yield return new WaitForSeconds(.60f);

        // Limit powerup effect activation period
        StartCoroutine(ResetInvinciblePowerUp());

        BeInvincible();

    }

	private void BeInvincible()
	{
		if (isInvincible)
			return;

        // Change main music
		mainAudio.Stop(); // stop main audio
		invincibleAudio.Play(); // play invincible power up audio

		// Display particle system
		invincibleParticles = GetComponentInChildren<ParticleSystem>();
		invincibleParticles.Play();

		isInvincible = true;
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

    private void StartSneakerPowerup()
    {
        StartCoroutine(ResetSneakerPowerup());

        EnableSuperSpeed();
    }

	private void EnableSuperSpeed()
	{
        if (superSpeedEnabled)
			return;

		superSpeedEnabled = true;
		movementScript.speed = 15f;
		// Increase pitch of music
		mainAudio.pitch = 1.5f;
	}

    private IEnumerator ResetSneakerPowerup()
    {
        yield return new WaitForSeconds(15f);

        ResetSpeed();
    }

	private void ResetSpeed()
	{
        // Reset speed
        superSpeedEnabled = false;
        mainAudio.pitch = 1;
        movementScript.speed = 5f;
	}

}
