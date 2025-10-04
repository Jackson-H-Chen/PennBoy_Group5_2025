using UnityEngine;

public class EnemyShip : MonoBehaviour
{
    public int hp = 500;

    public PlayerShip player;
    public int turnFlag = 0;

    public int cannonDamage = 100;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //turnFlag = 0; // Game starts as the player's turn
    }

    // Update is called once per frame
    void Update()
    {
        if (turnFlag == 1 && hp > 0) {
            Debug.Log("Enemy shoots!\n");

            // Enemy hit rate determined by RNG
            // TODO: May incorporate formula with player's speed stat
            // Higher speed = lower chance to get hit by the enemy ship
            int diceRoll = Random.Range(1, 21);
            if (diceRoll > 10) {
                player.TakeHit();
            }

            turnFlag = 0;
        }
    }

    // Method to be called when the enemy gets hit
    public void TakeHit()
    {
        hp -= player.cannonDamage;
        Debug.Log("Enemy hit! Remaining health: " + hp);

        if (hp <= 0)
        {
            Debug.Log("Enemy destroyed. They will soon be forgotten, swallowed by the abyss.");
            Destroy(gameObject); // Remove enemy from the scene
        } else {
            turnFlag = 1;
        }
    }
}
