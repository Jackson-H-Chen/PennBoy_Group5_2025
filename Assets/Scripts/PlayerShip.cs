using UnityEngine;

public class PlayerShip : MonoBehaviour
{
    public int hp = 500;

    public EnemyShip enemy;
    public int turnFlag = 1;

    public int cannonDamage = 100;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //turnFlag = 1; // Game starts as the player's turn
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Player turn flag: " + turnFlag);
        if (turnFlag == 1 && hp > 0) {
            // Check if the Space key is pressed
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // Log to the console
                Debug.Log("Player shoots!\n");

                if (enemy != null)
                {
                    enemy.TakeHit();
                    turnFlag = 0;
                }
                else
                {
                    Debug.LogWarning("Enemy reference not assigned in PlayerShooter!");
                }
            }
        }
    }

    public void TakeHit() {
        hp -= enemy.cannonDamage;
        Debug.Log("You suffered a hit to your hull! You have " + hp + " remaining.");

        if (hp <= 0)
        {
            Destroy(gameObject); // Remove player from the scene
            Debug.Log(
                "You have fallen to a technological force greater than yours. " +
                "You listen to the cacophony of twisting metal and shattering glass, " +
                "shrieking into an indifferent and eternal cosmic night." +
                "And then, the finality of silence.\n\n\n" +
                "CONTINUE THE CYCLE?"
            ); // TODO: Prompt for user input to restart the game
        } else {
            turnFlag = 1;
        }
    }
}
