using Assets.Scripts;
using Assets.Scripts.Hub;
using Assets.Scripts.Models;
using Assets.Scripts.ServerModels;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSceneController : MonoBehaviour
{
    public AudioSource sndWaiting;
    public AudioMixer audioMixer;

    public Canvas cnvError;
    public TMP_Text txtError;
    public Button btnCloseErrorCanvas;
    public Button btnReconnect;
    public TMP_Text txtWaitingText;

    public AudioSource soundEffects;

    GameCodeModel gameCodeModel;
    HtHubConnection hub;

    CurrentGameModel currentGameModel;

    bool quit;
    bool alreadySentQuit;
    // Start is called before the first frame update
    async void Start()
    {
        Global.CurrentScene = Constants.GameSceneName;

        var gameModel = Global.GetGameConfiguration();
        currentGameModel = Global.CurrentGameModel;
        if (gameModel == default || currentGameModel == default)
        {
            SceneManager.LoadScene(Constants.MainSceneName);
            return;
        }

        gameCodeModel = new GameCodeModel(currentGameModel.GameCode);

        var settings = Global.GetCurrentUserSettings();
        Global.ApplyUserSettings(settings);

        Global.OnUserSettingsChanged += OnUserSettingsChanged;
        Global.OnExitGame += OnExitGame;

        StartCoroutine(AudioHelper.FadeIn(sndWaiting, 10f));

        btnCloseErrorCanvas.onClick.AddListener(() =>
        {
            if (!btnReconnect.gameObject.activeInHierarchy)
            {
                cnvError.gameObject.SetActive(false);
            }
        });

        btnReconnect.onClick.AddListener(async () =>
        {
            btnReconnect.gameObject.SetActive(false);
            // only when "Disconnected" comes up, it will automatically disconnect
            if(await hub.ConnectAsync())
            {
                txtWaitingText.text = "Connected to hub. Waiting for Horror Master...";
            }
        });

        hub = new HtHubConnection(gameCodeModel);
        hub.OnConnectionStatusChanged += Hub_OnConnectionStatusChanged;
        hub.OnHmCommand += Hub_OnHmCommand;
        hub.OnHmPredefinedCommand += Hub_OnHmPredefinedCommand;

        txtWaitingText.text = "Connecting to hub...";
        if(await hub.ConnectAsync())
        {
            txtWaitingText.text = "Connected to hub. Waiting for Horror Master...";
        }
        else
        {
            txtWaitingText.text = "Error connecting to hub...";
        }
        
    }

    private void Hub_OnHmPredefinedCommand(object sender, HmCommandPredefinedModel e)
    {
        txtWaitingText.text = e.ToString();
    }

    private void Hub_OnHmCommand(object sender, HmCommandModel e)
    {
        txtWaitingText.text = e.ToString();

        if (e.AudioIds != default && e.AudioIds.Count > 0)
        {
            foreach (var id in e.AudioIds)
            {
                var audio = currentGameModel.GetAudio(id);
                if (audio != default)
                {
                    if (!audio.Model.IsBgm)
                    {
                        soundEffects.PlayOneShot(audio.AudioClip);
                        Debug.Log("Playing sound effect: " + audio.Model.Name);
                    }
                }
            }
        }
    }

    private void Hub_OnConnectionStatusChanged(object sender, HubConnectionStatusEventArgs e)
    {
        cnvError.gameObject.SetActive(true);
        btnReconnect.gameObject.SetActive(false);

        string ex = string.Empty;
        if (e.Exception != default)
            ex = $"({e.Exception.Message})";

        if (e.IsReconnecting)
        {
            txtError.text = "Lost connection to hub. Reconnecting..." + ex;
        }
        else if (e.Disconnected)
        {
            // Have to call Connect again
            txtError.text = "Disconnected from hub." + ex;
            btnReconnect.gameObject.SetActive(true);
        }
        else if (e.Reconnected)
        {
            txtError.text = "Connection restored" + ex;
        }
        else if (e.InvokeFailed)
        {
            txtError.text = "Failed to call hub invoke" + ex;
        }
        else if (e.FailedToConnect)
        {
            // Have to call Connect again
            txtError.text = "Not able to connect to hub" + ex;
            btnReconnect.gameObject.SetActive(true);
        }
        else
        {
            txtError.text = "This is a bug from Hub_OnConnectionStatusChanged";
        }
    }

    async void OnDestroy()
    {
        if (hub != default)
        {
            hub.OnConnectionStatusChanged -= Hub_OnConnectionStatusChanged;
            hub.OnHmCommand -= Hub_OnHmCommand;
            hub.OnHmPredefinedCommand -= Hub_OnHmPredefinedCommand;

            // unsubscribe because this will call `OnConnectionStatusChanged` with connection closed
            await hub.DisposeAsync();
        }

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
    async void Update()
    {
        if (quit)
        {
            if (!alreadySentQuit)
            {
                alreadySentQuit = true;
                hub.OnConnectionStatusChanged -= Hub_OnConnectionStatusChanged;
                hub.OnHmCommand -= Hub_OnHmCommand;
                hub.OnHmPredefinedCommand -= Hub_OnHmPredefinedCommand;
                await hub.StopAsync();
                Global.OkExitGame();
            }
            return;
        }
    }

}
