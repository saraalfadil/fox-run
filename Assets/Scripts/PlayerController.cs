using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private CircleCollider2D coll;
    private ParticleSystem part;
    // FSM
    private enum State { idle, running, jumping, falling, hurt, waiting };
    private State state = State.idle;
    private bool isInvincible = false;
    private bool hasMoved = true;
    private float idleTimer = 0f;
    private float idleDuration = 6f;
    private bool noDamage = false;
    private float noDamageTimer = 0f;
    private float noDamageDuration = 5f;
    private bool dropCherries = false;
    private float cherriesTimer = 0f;
    private float cherriesDuration = 1f;
    protected SpriteRenderer sprite;

    // Inspector variables
    [SerializeField] private LayerMask ground;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float hurtForce = 10f;
    [SerializeField] private int defeatEnemyScore = 100;
    [SerializeField] private AudioSource footstep;
    [SerializeField] private AudioSource bounceSound;
    [SerializeField] private AudioSource spikesSound;
    [SerializeField] private AudioSource powerupSound;
    public GameObject collectable;
    private List<GameObject> collectables = new List<GameObject>();
    public bool isJumping { get { return state == State.jumping; } }
    private bool isFlashing = false;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<CircleCollider2D>(); 
        sprite = GetComponent<SpriteRenderer>();
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

        UpdateLostCherriesAnimation();

    }

    private void TrackIdleTime() {

        // Keep track of elapsed time for idle animation
        idleTimer += Time.deltaTime;
        if (idleTimer >= idleDuration) {
            idleTimer = 0f;
            hasMoved = false;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Collectable")
        {
            Cherry cherry = collision.gameObject.GetComponent<Cherry>();
            cherry.Collected();
            collectables.Remove(collision.gameObject);
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

                PermanentUI.perm.score += defeatEnemyScore;
            }
            else 
            {   
                // If player is invincible, the enemy is defeated
                if (isInvincible)
                {
                    enemy.JumpedOn();
                    PermanentUI.perm.score += defeatEnemyScore;
                }
                else 
                {   
                    // Player gets hurt
                    DecreaseHealth();

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
                Jump();

                StartCoroutine(PowerupDelay());

            }
        }
    
        if(other.gameObject.tag == "BoxPowerupCherry")
        {   
            if(state == State.falling)
            {

                BoxPowerup box = other.gameObject.GetComponent<BoxPowerup>();
                box.Collected();
                Jump();

                PermanentUI.perm.cherries += 10;
            }
        }

        if(other.gameObject.tag == "Bounce")
        {

            // Play feedback sound
            bounceSound.Play();

            // Super jump
            Jump(45f);
        
        }

        if(other.gameObject.tag == "Collectable")
        {
            Cherry cherry = other.gameObject.GetComponent<Cherry>();
            cherry.Collected();
            collectables.Remove(other.gameObject);
        }
    }


    private void DecreaseHealth()
    {

        if (noDamage)
            return;
        
        PermanentUI.perm.healthStat.text = PermanentUI.perm.health.ToString();
        if(PermanentUI.perm.health <= 0)
        {   
            // Show game over
            PermanentUI.perm.GameOver();
        } 
        else 
        {
            state = State.hurt;
            PermanentUI.perm.health -= 1;
            PermanentUI.perm.cherries = 0;
            LoseCherries();
            
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
            StartCoroutine(ResumeIdleAfterHurt);
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

    private IEnumerator ResumeIdleAfterHurt()
    {
        yield return new WaitForSeconds(.5f);
        if(Mathf.Abs(rb.velocity.x) < .1f)
        {
            state = State.idle;
        }
        StartFlashAnimation();
    }

    public void StartFlashAnimation()
    {
        if (!isFlashing)
        {
            StartCoroutine(Flash());
        }
    }

    private IEnumerator Flash()
    {
        isFlashing = true;

        float elapsedTime = 0f;
        float flashDuration = 2.0f;
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

    private void LoseCherries()
    {   

        noDamage = true;
        int numberOfCherries = 6;
        
        for (int i = 0; i < numberOfCherries; i++)
        {
            // semicircle (180 degrees)
            float angle = i * Mathf.PI / (numberOfCherries - 1);
            
            float radius = 2f;
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            
            Vector3 pos = transform.position + new Vector3(x, y, 0);

            collectables.Add((GameObject)Instantiate(collectable, pos, Quaternion.identity));
        }
        

    }
    
    private void UpdateLostCherriesAnimation() {

        // Keep track of temporary invincibility
        noDamageTimer += Time.deltaTime;
        if (noDamageTimer >= noDamageDuration) {
            noDamageTimer = 0f;
            noDamage = false;
        }
        // Keep track of when cherries are suspended
        cherriesTimer += Time.deltaTime;
        if (cherriesTimer >= cherriesDuration) {
            cherriesTimer = 0f;
            dropCherries = true;
        }

        // Increase the radius of the circle gradually
        float radiusIncreaseRate = 0.5f;
        for (int i = collectables.Count - 1; i >= 0; i--)
        {     

            if (collectables[i] == null || collectables[i].GetComponent<Cherry>() == null)
            {
                collectables.RemoveAt(i);
                continue;
            }

            float angle = i * Mathf.PI * 2 / collectables.Count;
            float x = Mathf.Cos(angle) * (.01f + radiusIncreaseRate * Time.deltaTime);
            float y = Mathf.Sin(angle) * (.01f + radiusIncreaseRate * Time.deltaTime);

            Cherry cherry = collectables[i].GetComponent<Cherry>();

            // Continuously update position
            Vector3 newPos = cherry.transform.position + new Vector3(x, y, 0);
            cherry.transform.position = newPos;

            // Drop cherries to ground
            if(dropCherries) 
                cherry.ChangeCollider();

        }
    }

}

