using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneController : MonoBehaviour
{
    public AudioSource sndWaiting;
    // Start is called before the first frame update
    void Start()
    {
        Global.CurrentScene = Constants.GameSceneName;

        var gameModel = Global.GetGameConfiguration();
        var game = Global.CurrentGameModel;
        if (gameModel == default || game == default)
        {
            SceneManager.LoadScene(Constants.MainSceneName);
            return;
        }

        StartCoroutine(AudioHelper.FadeIn(sndWaiting, 10f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
