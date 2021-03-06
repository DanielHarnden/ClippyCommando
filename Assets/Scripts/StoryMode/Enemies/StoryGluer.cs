using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using Pathfinding;

public class StoryGluer : MonoBehaviour
{
    // Movement and Player Locating
    public AIPath aiPath;               // A* Pathfinding for Unity https://arongranberg.com/astar/
    private PlayerStatsStory playerStats;

    private Transform playerTransform;  // Used in AimRuler() to determine the player's position
    private Vector2 playerPos;          
    private Vector2 gluerPos;       
    private float distanceBetween;
    
    private Rigidbody2D gluerRigidbody; // Rigidbody used for movement
    public bool playerSeen;

    // Stats
    public float health;                  
    public float damage;
    public float moveSpeed;
    private bool dead;

    public GameObject gluePuddle;

    // UI and Audiovisual Flair
    private Text healthText;
    private AudioSource gluerAudio;
    private ParticleSystem gluerParticles;
    public GameObject[] bits;
    public AudioClip pain;
    public AudioClip screech;
    public AudioClip splooge;
    


    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStatsStory>();

        gluerRigidbody = this.gameObject.GetComponent<Rigidbody2D>();
        healthText = this.gameObject.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Text>();
        gluerAudio = this.gameObject.GetComponent<AudioSource>();
        gluerParticles = this.transform.GetChild(1).GetComponent<ParticleSystem>();

        aiPath.maxSpeed = moveSpeed;
    }



    void FixedUpdate() 
    {
        if (playerSeen)
        {
            aiPath.enabled = true;

            playerPos = playerTransform.position;
            gluerPos = this.transform.position;

            distanceBetween = Vector2.Distance(gluerPos, playerPos);

            aiPath.maxSpeed = -((2 * (Mathf.Pow(distanceBetween - 6, 2) / 10)) - 10 - moveSpeed);

            if (aiPath.maxSpeed < 2)
            {
                aiPath.maxSpeed = 2;
            }

            AimGluer();

            if (aiPath.maxSpeed >= 5f && !gluerAudio.isPlaying && !dead)
            {
                gluerAudio.clip = screech;
                gluerAudio.Play();
            }
            
        } else {
            aiPath.enabled = false;
        }
            
        // Kills if health is depleted
        if (health <= 0)
        {
            KillGluer(false);
        } else {
            healthText.text = health.ToString();
        }
    }



    // Ruler "looks" towards player
    void AimGluer()
    {
        // Compares player and enemy positions, stores in playerPos
        playerPos.x = playerPos.x - gluerPos.x;
        playerPos.y = playerPos.y - gluerPos.y;

        // Determines the angle needed to rotate
        float angle = Mathf.Atan2(playerPos.y, playerPos.x) * Mathf.Rad2Deg;

        // Angle - 90 in Vector3 due to sprite being vertical and rotate towards pointing right, not up
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(new Vector3(0, 0, angle - 90)), Time.deltaTime * 750f);
    }



    public void KillGluer(bool suicided)
    {
        if (!dead)
        {
            if (gluerAudio.isPlaying)
            {
                gluerAudio.Stop();
            }

            dead = true;
            gameObject.tag = "Untagged";
            this.GetComponent<SpriteRenderer>().enabled = false;
            
            foreach(Transform child in transform)
            {
                if (child.GetComponent<ParticleSystem>() == null)
                {
                    GameObject.Destroy(child.gameObject);
                }
            }
            
            aiPath.enabled = false;
            this.GetComponent<BoxCollider2D>().enabled = false;
            playerStats.enemiesLeft -= 1;

            if (suicided)
            {
                Instantiate(gluePuddle, this.transform.position,   this.transform.rotation);
                gluerAudio.clip = splooge;
                gluerAudio.Play();
            } else {
                for (int i = 0; i < bits.Length; i++)
                {
                    Instantiate(bits[i], this.transform.position + new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0f), this.transform.rotation);
                }
            }

            
        } else {
            if (!gluerAudio.isPlaying && !gluerParticles.isPlaying)
            {
                Destroy(this.gameObject);
            }
        }
    }
    
    public void Damaged()
    {
        gluerAudio.clip = pain;
        gluerAudio.Play();
        gluerParticles.Play();
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.tag == "Player")
        {
            if (!playerSeen)
            {
                if (!gluerAudio.isPlaying)
                {
                    gluerAudio.clip = screech;
                    gluerAudio.Play();
                }
                playerSeen = true;

                GameObject[] rulers = GameObject.FindGameObjectsWithTag("StoryRuler");
                GameObject[] gluers = GameObject.FindGameObjectsWithTag("StoryGluer");
                GameObject[] gunners = GameObject.FindGameObjectsWithTag("StoryGunner");

                foreach(GameObject ally in rulers)
                {
                    ally.GetComponentInParent<StoryRuler>().AlertFriends();
                    ally.GetComponentInParent<StoryRuler>().playerSeen = true;
                }

                foreach(GameObject ally in gluers)
                {
                    ally.GetComponentInParent<StoryGluer>().playerSeen = true;
                }

                foreach(GameObject ally in gunners)
                {
                    ally.GetComponentInParent<StoryGunner>().playerSeen = true;
                }
            }
        }
    }
}
