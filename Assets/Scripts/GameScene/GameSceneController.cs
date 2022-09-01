using Assets.Scripts;
using Assets.Scripts.Models;
using Assets.Scripts.ServerModels;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GameSceneController : MonoBehaviour
{
    public AudioSource sndWaiting;
    public AudioMixer audioMixer;

    GameCodeModel gameCodeModel;

    bool quit;
    bool okToExit;
    // Start is called before the first frame update
    async void Start()
    {
        Global.CurrentScene = Constants.GameSceneName;

        var gameModel = Global.GetGameConfiguration();
        var game = Global.CurrentGameModel;
        if (gameModel == default || game == default)
        {
            SceneManager.LoadScene(Constants.MainSceneName);
            return;
        }

        gameCodeModel = new GameCodeModel(game.GameCode);

        var settings = Global.GetCurrentUserSettings();
        Global.ApplyUserSettings(settings);

        Global.OnUserSettingsChanged += OnUserSettingsChanged;
        Global.OnExitGame += OnExitGame;

        StartCoroutine(AudioHelper.FadeIn(sndWaiting, 10f));

        var connection = new HubConnectionBuilder()
                .WithUrl(Constants.HubUrl)
                .Build();

        //await connection.StartAsync();
        await connection.InvokeAsync("JoinGameAsPlayer", gameModel);
    }
    void OnDestroy()
    {
        Global.OnUserSettingsChanged -= OnUserSettingsChanged;
        Global.OnExitGame -= OnExitGame;
        Debug.Log("MenuController:OnDestroy");
    }

    void OnExitGame(object sender, EventArgs args)
    {
        quit = true;
    }

    void OnUserSettingsChanged(object sender, UserSettingsEventArgs args)
    {
        Global.ApplyUserSettings(args, audioMixer);
    }
    // Update is called once per frame
    void Update()
    {
        if (quit)
        {
            // TODO
            okToExit = true;

            // okToExit is decided locally for when it is ok to signal
            if (okToExit)
            {
                Global.OkExitGame();
            }
        }
    }

}
