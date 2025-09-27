using UnityEngine;

public class EnemyShip : MonoBehaviour
{

    public int hp = 500;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Method to be called when the enemy gets hit
    public void TakeHit()
    {
        hp -= 100;
        Debug.Log("Enemy hit! Remaining health: " + hp);

        if (hp <= 0)
        {
            Debug.Log("Enemy destroyed!");
            Destroy(gameObject); // Remove enemy from the scene
        }
    }
}
