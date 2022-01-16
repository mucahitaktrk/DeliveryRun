using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PanelScript : MonoBehaviour
{
    public string value = "";
    public PanelType panelType = null;
    public TextMeshPro valueText = null;

    private void Awake()
    {
        gameObject.name = panelType.panelValue;
        if (gameObject.layer == 6)
        {
            valueText.text = "+" + gameObject.name;
        }
        else if (gameObject.layer == 7)
        {
            valueText.text = "-" + gameObject.name;
        }
        else if (gameObject.layer == 8)
        {
            valueText.text = "x" + gameObject.name;
        }
        else if (gameObject.layer == 9)
        {
            valueText.text = "/" + gameObject.name;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Destroy(gameObject);
        }
    }
}
