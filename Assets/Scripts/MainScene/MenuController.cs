using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public Button btnStartGame;
    public TMP_InputField txtGameCode;

    // Start is called before the first frame update
    void Start()
    {
        btnStartGame.onClick.AddListener(EnterGame_Click);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnterGame_Click()
    {
        Debug.Log(txtGameCode.text);
    }
}
