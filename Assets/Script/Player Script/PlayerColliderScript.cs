using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColliderScript : MonoBehaviour
{
    public bool pizzaPlus = false;
    public bool pizzaMinus = false;
    public bool flyMultiple = false;
    public bool flyDivision = false;
    public bool pizzaDelveryRight = false;
    public bool pizzaDeliveryLeft = false;
    public bool coinCol = false;
    public bool obstalceCol = false;
    public bool pizzaFinish = false;
    public bool col = false;
    public bool pizzaciCocuk = false;
    public bool finish = false;
    public string value = null;


    public GameObject[] piz = null;

    public Animator playerAnimator;
    public BoxCollider playerCollider;

    private void Awake()
    {
        piz = GameObject.FindGameObjectsWithTag("Pizza");
        playerCollider = GetComponent<BoxCollider>();
        playerAnimator = gameObject.transform.GetChild(3).GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Jump")
        {
            value = gameObject.name;
            col = true;
        }
        if (other.gameObject.layer == 6)
        {
            value = other.gameObject.name;
            pizzaPlus = true;
        }
        else if (other.gameObject.layer == 7)
        {
            pizzaciCocuk = true;
        }
        else if (other.gameObject.layer == 15)
        {
            value = other.gameObject.name;
            pizzaMinus = true;
        }
        else if (other.gameObject.layer == 8)
        {
            value = other.gameObject.name;
            flyMultiple = true;
        }
        else if (other.gameObject.layer == 9)
        {
            value = other.gameObject.name;
            flyDivision = true;
        }
        else if (other.gameObject.layer == 10)
        {
            pizzaDelveryRight = true;
        }
        else if (other.gameObject.layer == 11)
        {
            pizzaDeliveryLeft = true;
        }
        else if (other.gameObject.layer == 12)
        {
            coinCol = true;
        }
        else if (other.gameObject.layer == 13) 
        {
            obstalceCol = true;
        }
        else if (other.gameObject.layer == 14)
        {
            finish = true; 
        }
    }
}
