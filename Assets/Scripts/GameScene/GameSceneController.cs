using Assets.Scripts;
using Assets.Scripts.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GameSceneController : MonoBehaviour
{
    public AudioSource sndWaiting;
    public AudioMixer audioMixer;

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
        var settings = Global.GetCurrentUserSettings();
        Global.ApplyUserSettings(settings);

        Global.OnUserSettingsChanged += OnUserSettingsChanged;

        StartCoroutine(AudioHelper.FadeIn(sndWaiting, 10f));

        var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:53353/ChatHub")
                .Build();
    }
    void OnDestroy()
    {
        Global.OnUserSettingsChanged -= OnUserSettingsChanged;
        Debug.Log("MenuController:OnDestroy");
    }

    void OnUserSettingsChanged(object sender, UserSettingsEventArgs args)
    {
        Global.ApplyUserSettings(args, audioMixer);
    }
    // Update is called once per frame
    void Update()
    {
        
    }

}
