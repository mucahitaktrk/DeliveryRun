using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{

    [SerializeField] private GameObject playerObject = null;
    public List<GameObject> pizza = null;


    public GameObject[] piz = null;

    [SerializeField] private GameObject pizzaBox = null;
    [SerializeField] public GameObject postion = null;
    [SerializeField] private GameObject pizzaPos = null;

    public GameObject[] finishObjects = null;
    public GameObject pizzaDel;

    public GameObject deneme;

    private float _move = -1.2f;
    private float _lastFrameFingerPostionX;
    private float _moveFactorX;
    public float panelValue = 5;

    private int coins = 0;
    private int coin = 0;

    [SerializeField] private float boundrey = 0;

    
    private Rigidbody playerRigidbody;
    [SerializeField] private Rigidbody pizzaRigidbody;
    [SerializeField] private List<Rigidbody> pizzaList;
    private PlayerColliderScript playerColliderScript;
    [SerializeField] private PizzaVariantScript[] pizzaVariantScript;

    [SerializeField] private GameObject[] UI = null;
    [SerializeField] private Text gameCoin = null;
    [SerializeField] private Text nextCoin = null;
    [SerializeField] private Text AdCoin = null;
    [SerializeField] private GameObject[] levels = null;
    private int level = 0;

    private void Awake()
    {
        UI[0].SetActive(false);
        UI[1].SetActive(false);
       // piz = GameObject.FindGameObjectsWithTag("Pizza");
        level = PlayerPrefs.GetInt("Level");
        pizzaDel = GameObject.FindGameObjectWithTag("Pizza");
        playerObject = GameObject.FindGameObjectWithTag("Player");
        finishObjects = GameObject.FindGameObjectsWithTag("Jump");
        playerColliderScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerColliderScript>();
        playerRigidbody = playerObject.GetComponent<Rigidbody>();
        coin = piz.Length;
        for (int i = 0; i < piz.Length; i++)
        {
            piz[i].SetActive(false);
        }
    }

    private void Update()
    {
        

            InputSystem();
            MoverSystem();
         

        Speed();

        Level();

        if (coin < 9)
        {
            Fail();
        }
        if (coin > 0)
        {
            Coin();
        }
        Finish();
        //PizzaMinus();
        PizzaDelivery();
        coins = PlayerPrefs.GetInt("Coins");
        gameCoin.text = 100.ToString();
    }

    private void InputSystem()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _lastFrameFingerPostionX = Input.mousePosition.x;
        }
        else if (Input.GetMouseButton(0))
        {
            _moveFactorX = Input.mousePosition.x - _lastFrameFingerPostionX;
            _lastFrameFingerPostionX = Input.mousePosition.x;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _moveFactorX = 0;
        }

        float xBondrey = Mathf.Clamp(value: playerObject.transform.position.x, min: -boundrey, max: boundrey);
        playerObject.transform.position = new Vector3(xBondrey, playerObject.transform.position.y, playerObject.transform.position.z);
        
    }

    private void MoverSystem()
    {
        float swaerSystem = Time.fixedDeltaTime * _move * 0.8f * _moveFactorX;
        playerObject.transform.Translate(swaerSystem, 0, 0);
    }

    private void Speed()
    {
        playerRigidbody.velocity = new Vector3(playerObject.transform.position.x, playerObject.transform.position.y, 15f);
    }

    private void PizzaInstantiate()
    {
        /*
        postion.gameObject.transform.position += postion.transform.up * pizzaBox.transform.localScale.y - new Vector3(0, 0.55f, 0);
        pizza.Add(Instantiate(pizzaBox, postion.gameObject.transform.position,
        playerObject.transform.rotation, playerObject.transform.GetChild(0)));
        */
        postion.gameObject.transform.position += postion.transform.forward * deneme.transform.localScale.x + new Vector3(0.0f, 0.0f, +2.5f);
        pizza.Add(Instantiate(deneme, postion.gameObject.transform.position,
        deneme.transform.rotation, playerObject.transform.GetChild(3)));
        for (int i = 0; i < pizza.Count; i++)
        {
            pizzaRigidbody = pizza[i].GetComponent<Rigidbody>();
        }
        pizzaList.Add(pizzaRigidbody);
    }

    private void PizInstantiate()
    {
        postion.gameObject.transform.position += postion.transform.forward * deneme.transform.localScale.x + new Vector3(0.0f, 0.0f, +2.5f);
        pizza.Add(Instantiate(deneme, postion.gameObject.transform.position,
        deneme.transform.rotation, playerObject.transform.GetChild(3)));
        for (int i = 0; i < pizza.Count; i++)
        {
            pizzaRigidbody = pizza[i].GetComponent<Rigidbody>();
        }
        pizzaList.Add(pizzaRigidbody);
    }

    private void PizzaMinus()
    {
        if (playerColliderScript.pizzaMinus)
        {

            Destroy(pizza[0],0.2f);
            pizza.RemoveAt(0);
            pizzaList.RemoveAt(0);
            playerColliderScript.pizzaMinus = false;
        }
    }
    
    private void PizzaDelivery()
    {
        if (playerColliderScript.pizzaDeliveryLeft)
        {
            pizzaRigidbody.AddForce(new Vector3(-200, 60, 0));
            Destroy(pizzaDel, 0.5f);

            //Instantiate(pizzaBox, postion.gameObject.transform.position, Quaternion.identity, playerObject.transform.GetChild(0));
            //pizzaRigidbody = pizzaDel.GetComponent<Rigidbody>();
            playerColliderScript.pizzaDeliveryLeft = false;
        }
        else if (playerColliderScript.pizzaDelveryRight)
        {
            pizzaRigidbody.AddForce(new Vector3(200, 60, 0));
            
            //Instantiate(pizzaBox, postion.gameObject.transform.position, Quaternion.identity, playerObject.transform.GetChild(0));
            //pizzaRigidbody = pizzaDel.GetComponent<Rigidbody>();
            playerColliderScript.pizzaDelveryRight = false;
        }
    }
    
    private void Fail()
    {
        if (playerColliderScript.obstalceCol)
        {
            pizzaVariantScript[coin].count = 0;
            piz[coin].transform.GetChild(3).gameObject.SetActive(false);
            piz[coin].transform.GetChild(4).gameObject.SetActive(false);
            piz[coin].SetActive(false);
            coin++;
            
            playerColliderScript.obstalceCol = false;
            /*
            playerRigidbody.velocity = Vector3.zero;
            playerColliderScript.playerCollider.isTrigger = true;
            playerColliderScript.playerAnimator.SetBool("Fail", true);
            UI[1].SetActive(true);
            nextCoin.text = coin.ToString();
            AdCoin.text = coin.ToString();
            /*
            for (int i = 0; i < pizzaList.Count; i++)
            {
                float posX = Random.Range(-10f, 10f);
                float posY = Random.Range(2f, 5f);
                pizzaList[i].AddForce(new Vector3(posX, posY, posX));
            }
            */
        }
    }

    private void Coin()
    {
        if (playerColliderScript.coinCol)
        {

            //PizzaInstantiate();
            coin--;
            piz[coin].SetActive(true);
            //PlayerPrefs.SetInt("Coin", coin);
            playerColliderScript.coinCol = false;
        }
    }

    private void Finish()
    {
        if (playerColliderScript.finish)
        {
            playerRigidbody.velocity = Vector3.zero;
            playerColliderScript.playerCollider.isTrigger = true;
            playerColliderScript.playerAnimator.SetBool("Victory", true);
            UI[0].SetActive(true);
            nextCoin.text = coin.ToString();
            AdCoin.text =  (coin * 3).ToString();
            /*
            for (int i = 0; i < pizzaList.Count; i++)
            {
                float posX = Random.Range(-10f, 10f);
                float posY = Random.Range(2f, 5f);
                pizzaList[i].AddForce(new Vector3(posX, posY, posX));
            }
            */
        }
    }

    private void Level()
    {
        if (level == 0)
        {
            levels[0].SetActive(true);
        }
        else if (level >= 1)
        {
            levels[level - 1].SetActive(false);
            levels[level].SetActive(true);
        }
    }

    public void NextButton()
    {
        level++;
        coins += 100 ;
        PlayerPrefs.GetInt("Coins", coins);
        PlayerPrefs.SetInt("Level", level);
        SceneManager.LoadScene(0);
    }
    public void NextAdButton()
    {
        level++;
        coins += 300 ;
        PlayerPrefs.GetInt("Coins", coins);
        PlayerPrefs.SetInt("Level", level);
        SceneManager.LoadScene(0);
    }
    public void Lose()
    {
        SceneManager.LoadScene(0);
    }

}
