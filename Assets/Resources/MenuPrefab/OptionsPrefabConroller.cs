using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsPrefabConroller : MonoBehaviour
{
    public Button BtnGoBack;

    // Start is called before the first frame update
    void Start()
    {
        BtnGoBack.onClick.AddListener(BtnGoBack_Click);
    }

    void BtnGoBack_Click()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
