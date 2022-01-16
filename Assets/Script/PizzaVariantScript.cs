using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PizzaVariantScript : MonoBehaviour
{
    public int count = 0;
    private GameManager gameManager;
    private ParticleSystem particle = null;

    private void Awake()
    {
        particle = GetComponent<ParticleSystem>();
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        count = 0;
    }
    private void Update()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
  
        if (other.gameObject.layer == 7)
        {
            gameManager.postion.gameObject.transform.position += gameManager.postion.transform.forward * gameObject.transform.localScale.x + new Vector3(0.0f, 0.0f, -0.8f);
            gameManager.pizza.RemoveAt(0);
            Destroy(gameObject);
        }
        if (other.gameObject.layer == 16)
        {
            particle.Play();
            if (count == 0)
            {
                Debug.Log(1);
                transform.GetChild(3).gameObject.SetActive(true);
                count++;
            }
            else if (count == 1)
            {
                gameObject.transform.GetChild(4).gameObject.SetActive(true);
            }
        }

    }
}
