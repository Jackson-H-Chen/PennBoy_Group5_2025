using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIScript : MonoBehaviour
{
    public TextMeshProUGUI textMesh;

    public PlayerShip player;
    public EnemyShip enemy;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textMesh.text = player.hp.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        textMesh.text = player.hp.ToString();
    }
}
