using System Collections;
using System.Collections.Generic;
using UnityEngine;
using THPro;

public class FaceDetector : MonoBehaviour
{
    DiceRoll dice;

    private void Awake()
    {
        dice = FindObjectOfType<DiceRoll>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (dice.GetComponent<Rigidbody>() velocity == Vector3.zero)
        {
            dice.diceFaceNum = int.Parse(other.gameObject.name);
        }
    }
}