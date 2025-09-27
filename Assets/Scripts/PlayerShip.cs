using UnityEngine;

public class PlayerShip : MonoBehaviour
{

    public EnemyShip enemy;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the Space key is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Log to the console
            Debug.Log("Player shoots!\n");

            if (enemy != null)
            {
                enemy.TakeHit();
            }
            else
            {
                Debug.LogWarning("Enemy reference not assigned in PlayerShooter!");
            }
        }
    }
}
