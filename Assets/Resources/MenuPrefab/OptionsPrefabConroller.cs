using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionsPrefabConroller : MonoBehaviour
{
    public Button BtnGoBack;
    public Button btnClearCache;
    public TMP_Text txtCurrentCache;
    public Button btnExitGame;

    // Start is called before the first frame update
    void Start()
    {
        var model = Global.CurrentGameModel;
        if (model != default)
        {
            txtCurrentCache.text = model.GameCode;
        }
        else
        {
            txtCurrentCache.text = "<none>";
        }

        if (Global.CurrentScene == Constants.GameSceneName)
        {
            btnClearCache.gameObject.SetActive(false);
        }
        else if (Global.CurrentScene == Constants.LoadingSceneName)
        {
            btnClearCache.gameObject.SetActive(false);
        }
        else
        {
            btnExitGame.gameObject.SetActive(false);
        }

        btnClearCache.onClick.AddListener(BtnClearCache_Click);
        btnExitGame.onClick.AddListener(BtnExitGame_Click);
        BtnGoBack.onClick.AddListener(BtnGoBack_Click);
    }

    void BtnGoBack_Click()
    {
        gameObject.SetActive(false);
    }

    void BtnClearCache_Click()
    {
        var model = Global.CurrentGameModel;
        if (model != default)
        {
            model.Dispose();
            Global.CurrentGameModel = default;
            txtCurrentCache.text = "<none>";
        }
    }

    void BtnExitGame_Click()
    {
        // TODO: scenes have to say OK to leave
        SceneManager.LoadScene(Constants.MainSceneName);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
