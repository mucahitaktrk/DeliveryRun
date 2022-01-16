using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PizzaScript : MonoBehaviour
{

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Pizza")
        {
            rb.AddForce(new Vector2(0, 5));
        }
    }
}
