using UnityEngine;

// The projectile generated by the Gunner (stapler) enemies.
public class Staple : MonoBehaviour
{
    private float deathTimer = 5f;
    private int damage = 1;
    private bool die = false;

    private void Start()
    {
        // TODO: broken for story mode, fix
        //damage = (int)GameObject.FindGameObjectWithTag("WaveManager").GetComponent<WaveManager>().gunnerStats[1];
        damage = 5;
          
    }

    private void Update() 
    {
        // Timer to delete old bullets
        if (deathTimer > 0f)
        {
            deathTimer -= Time.deltaTime;
        } else {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D colObj)
    {
        switch (colObj.gameObject.tag)
        {
            case "Wall":
                die = true;
                break;
                
            case "Player":
                PlayerStats playerStats = colObj.gameObject.GetComponent<PlayerStats>();
                
                if (playerStats != null)
                {
                    playerStats.health -= damage;
                } else {
                    colObj.gameObject.GetComponent<PlayerStatsStory>().health -= damage;
                }
                
                die = true;
                break;

            default:
                break;
        }

        if (die)
        {
            Destroy(this.gameObject);
        }
    }
}