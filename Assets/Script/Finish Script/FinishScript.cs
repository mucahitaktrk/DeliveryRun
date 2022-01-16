using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FinishScript : MonoBehaviour
{
    public bool col = false;
    public string value = null;
    public FinishType finishType = null;
    public TextMeshPro finishValueText = null;
    
    private void Awake()
    {
        gameObject.name = finishType.finishValue;
        finishValueText.text = "x" + finishType.finishValue;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 10)
        {
            col = true;
            Destroy(gameObject);
        }
        
    }
}
