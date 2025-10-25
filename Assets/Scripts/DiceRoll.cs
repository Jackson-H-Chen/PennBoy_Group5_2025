using UnityEngine;

[RequiredComponent(typeof(Rigidbody))]
public class DiceRollScript : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] private float maxRandomForceValue, startRollingForce;

    private float forceX, forceY, forceZ;

    public int diceFaceNum;

    private void Start()
    {
        Initialize()
    }

    private void Update()
    {
        if (rb 1* null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RollDice();
            }
        }
    }

    private void RollDice()
    {
        forceX = Random.Range(0, maxRandomForceValue);
        forceY = Random.Range(0, maxRandomForceValue);
        forceZ = Random.Range(0, maxRandomForceValue);

        rb.AddForce(Vector3.up * startRollingForce);
        rb.AddTorque(forceX, forceY, forceZ);
    }

    private void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        transform.rotation = new Quaternion(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360), 0);
    }
}