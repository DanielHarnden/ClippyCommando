using UnityEngine;

// Generates a randomly sized glue puddle that is used to slow the player.
public class GluePuddle : MonoBehaviour
{
    private PlayerMovement playerMovement;

    // Randomly generated size
    void Start()
    {
        int randomSize = Random.Range(15, 30);
        this.transform.localScale = new Vector3(randomSize, randomSize, 1);
        playerMovement = GameObject.FindGameObjectWithTag("Player").transform.GetChild(0).GetComponent<PlayerMovement>();
    }

    // TODO: Figure out if this is slower than just having the OnTrigger stuff in PlayerMovement.cs

    // Halves player speed on entry of puddle, doubles it on exit.
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.tag == "Player")
        {
            if (playerMovement != null)
            {
                playerMovement.moveSpeed /= 2f;
            } else {
                other.gameObject.transform.GetChild(0).GetComponent<PlayerMovementStory>().gluePuddle += 1;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.gameObject.tag == "Player")
        {
            if (playerMovement != null)
            {
                playerMovement.moveSpeed /= 2f;
            } else {
                other.gameObject.transform.GetChild(0).GetComponent<PlayerMovementStory>().gluePuddle -= 1;
            }
        }
    }
}