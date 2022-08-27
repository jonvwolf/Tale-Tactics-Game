using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenSettingsBtnController : MonoBehaviour
{
    public Button btnSelfReference;
    // Start is called before the first frame update
    void Start()
    {
        btnSelfReference.onClick.AddListener(() =>
        {
            Debug.Log("hey.. it works... wow...");
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
