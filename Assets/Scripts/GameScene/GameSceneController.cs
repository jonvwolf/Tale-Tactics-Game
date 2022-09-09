using Assets.Scripts;
using Assets.Scripts.Hub;
using Assets.Scripts.Models;
using Assets.Scripts.ServerModels;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSceneController : MonoBehaviour
{
    public CanvasGroup cgGameCanvas;

    public AudioSource sndWaiting;
    public AudioSource sndBgm2;
    Coroutine crFadeIn;
    Coroutine crFadeOut;
    bool isBgm1Playing = true;
    long idAudioBgmPlaying = 0;

    bool receivedFirstHmCommand;

    public AudioMixer audioMixer;

    public Canvas cnvWaiting;
    public Canvas cnvGame;
    public GameObject panelImage;
    bool isImage1Showing = true;
    Coroutine crImageFadeIn = default;
    Coroutine crImageFadeOut = default;
    long idImageShowing = 0;
    public Image imgImage1;
    public Image imgImage2;

    public GameObject panelTimer;
    public TMP_Text txtTimer;
    public Coroutine crTimer;
    public AudioSource sndTimerTick;
    public AudioSource sndTimerEnded;

    public GameObject panelText;
    /// <summary>
    /// This is the text to show from incoming HmCommand
    /// </summary>
    public TMP_Text txtText;

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

        crFadeIn = StartCoroutine(AudioHelper.FadeIn(sndWaiting, Constants.AudioFadeInTime));

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
        FirstReceviedHmCommand();

        // This is to stop the waiting audio if its the first command
        Handle_OnHmCommand_Audio(new HmCommandModel());

        if (e.StopSoundEffects.HasValue && e.StopSoundEffects.Value)
        {
            //TODO: welp.. can't stop play playoneshots

            if (crTimer != default)
                StopCoroutine(crTimer);

            if (sndTimerTick.isPlaying)
                sndTimerTick.Stop();
            if (sndTimerEnded.isPlaying)
                sndTimerEnded.Stop();

            panelTimer.SetActive(false);
        }

        if (e.ClearScreen.HasValue && e.ClearScreen.Value)
        {
            if (crImageFadeIn != default)
                StopCoroutine(crImageFadeIn);
            if (crImageFadeOut != default)
                StopCoroutine(crImageFadeOut);
            idImageShowing = 0;

            // ! because they are reversed
            if (!isImage1Showing)
            {
                if (imgImage2.color.a > 0)
                {
                    var img2color = imgImage2.color;
                    img2color.a = 0;
                    imgImage2.color = img2color;
                }

                if (imgImage1.color.a > 0)
                    crImageFadeOut = StartCoroutine(AudioHelper.FadeOutImage(imgImage1, Constants.ImageFadeOutTime));
            }
            else
            {
                if (imgImage1.color.a > 0)
                {
                    var img1color = imgImage1.color;
                    img1color.a = 0;
                    imgImage1.color = img1color;
                }

                if (imgImage2.color.a > 0)
                    crImageFadeOut = StartCoroutine(AudioHelper.FadeOutImage(imgImage2, Constants.ImageFadeOutTime));
            }

            txtText.text = string.Empty;
            panelText.SetActive(false);
        }

        if (e.StopBgm.HasValue && e.StopBgm.Value)
        {
            idAudioBgmPlaying = 0;
            if (crFadeIn != default)
                StopCoroutine(crFadeIn);
            if (crFadeOut != default)
                StopCoroutine(crFadeOut);

            if (isBgm1Playing)
            {
                if (sndBgm2.isPlaying)
                    sndBgm2.Stop();
                if (sndWaiting.isPlaying)
                    crFadeOut = StartCoroutine(AudioHelper.FadeOut(sndWaiting, Constants.AudioFadeOutTime));
            }
            else
            {
                if (sndWaiting.isPlaying)
                    sndWaiting.Stop();
                if (sndBgm2.isPlaying)
                    crFadeOut = StartCoroutine(AudioHelper.FadeOut(sndBgm2, Constants.AudioFadeOutTime));
            }

        }

        // this has to be here
        receivedFirstHmCommand = true;
    }

    private void FirstReceviedHmCommand()
    {
        if (!receivedFirstHmCommand)
        {
            StartCoroutine(AudioHelper.FadeOutCanvasGroup(cgGameCanvas, Constants.ImageFadeOutTime, cnvWaiting));

            Global.CanvasChangedForOptionsBtn(cnvGame);
            cnvGame.gameObject.SetActive(true);
            // panel image will be always active
            panelImage.SetActive(true);
        }
    }

    private void Hub_OnHmCommand(object sender, HmCommandModel e)
    {
        FirstReceviedHmCommand();

        Handle_OnHmCommand_Timer(e);
        Handle_OnHmCommand_Audio(e);
        Handle_OnHmCommand_Image(e);
        Handle_OnHmCommand_Text(e);

        // this has to be here
        receivedFirstHmCommand = true;
    }
    void Handle_OnHmCommand_Audio(HmCommandModel e)
    {
        bool gotBgm = false;
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
                    else if (!gotBgm)
                    {
                        gotBgm = true;

                        if (idAudioBgmPlaying == id)
                        {
                            continue;
                        }
                        idAudioBgmPlaying = id;

                        if (isBgm1Playing)
                        {
                            isBgm1Playing = false;
                            if (crFadeIn != default)
                                StopCoroutine(crFadeIn);
                            if (crFadeOut != default)
                                StopCoroutine(crFadeOut);

                            if (sndWaiting.isPlaying)
                                crFadeOut = StartCoroutine(AudioHelper.FadeOut(sndWaiting, Constants.AudioFadeOutTime));

                            sndBgm2.clip = audio.AudioClip;
                            crFadeIn = StartCoroutine(AudioHelper.FadeIn(sndBgm2, Constants.AudioFadeInTime));
                        }
                        else
                        {
                            isBgm1Playing = true;
                            if (crFadeIn != default)
                                StopCoroutine(crFadeIn);
                            if (crFadeOut != default)
                                StopCoroutine(crFadeOut);

                            if (sndBgm2.isPlaying)
                                crFadeOut = StartCoroutine(AudioHelper.FadeOut(sndBgm2, Constants.AudioFadeOutTime));

                            sndWaiting.clip = audio.AudioClip;
                            crFadeIn = StartCoroutine(AudioHelper.FadeIn(sndWaiting, Constants.AudioFadeInTime));
                        }
                    }
                }
            }
        }
        else
        {
            if (!receivedFirstHmCommand)
            {
                if (crFadeIn != default)
                    StopCoroutine(crFadeIn);

                // this is to fade out the waiting background sound when the first command doesn't have an audio
                crFadeOut = StartCoroutine(AudioHelper.FadeOut(sndWaiting, Constants.AudioFadeOutTime));
            }
        }
    }
    void Handle_OnHmCommand_Image(HmCommandModel e)
    {
        if (e.ImageId.HasValue && e.ImageId.Value > 0)
        {
            var image = currentGameModel.GetImage(e.ImageId.Value);
            if (image != default)
            {
                if(idImageShowing != e.ImageId)
                {
                    idImageShowing = e.ImageId.Value;
                    if (isImage1Showing)
                    {
                        isImage1Showing = false;
                        if (crImageFadeIn != default)
                            StopCoroutine(crImageFadeIn);
                        if (crImageFadeOut != default)
                            StopCoroutine(crImageFadeOut);

                        if (imgImage2.color.a > 0)
                            crImageFadeOut = StartCoroutine(AudioHelper.FadeOutImage(imgImage2, Constants.ImageFadeOutTime));

                        imgImage1.sprite = image.Sprite;
                        crImageFadeIn = StartCoroutine(AudioHelper.FadeInImage(imgImage1, Constants.ImageFadeInTime));
                    }
                    else
                    {
                        isImage1Showing = true;

                        if (crImageFadeIn != default)
                            StopCoroutine(crImageFadeIn);

                        if (crImageFadeOut != default)
                            StopCoroutine(crImageFadeOut);

                        if (imgImage1.color.a > 0)
                            crImageFadeOut = StartCoroutine(AudioHelper.FadeOutImage(imgImage1, Constants.ImageFadeOutTime));

                        imgImage2.sprite = image.Sprite;
                        crImageFadeIn = StartCoroutine(AudioHelper.FadeInImage(imgImage2, Constants.ImageFadeInTime));
                    }
                }
            }
        }
    }
    void Handle_OnHmCommand_Timer(HmCommandModel e)
    {
        if (e.Timer.HasValue && e.Timer.Value > 0)
        {
            panelTimer.SetActive(true);
            if (crTimer != default)
                StopCoroutine(crTimer);

            crTimer = StartCoroutine(StartTimer(e.Timer.Value));
        }
    }

    IEnumerator StartTimer(long timer)
    {
        sndTimerTick.Play();
        txtTimer.text = $"{timer}s";
        while (timer > 0)
        {
            yield return new WaitForSeconds(1);
            timer--;
            txtTimer.text = $"{timer}s";
        }
        sndTimerTick.Stop();

        sndTimerEnded.Play();
        while (timer > -4)
        {
            timer--;
            yield return new WaitForSeconds(1);
        }
        panelTimer.SetActive(false);
    }

    void Handle_OnHmCommand_Text(HmCommandModel e)
    {
        if (!string.IsNullOrEmpty(e.Text))
        {
            panelText.SetActive(true);
            txtText.text = e.Text;
        }
        else
        {
            if (e.ImageId.HasValue)
            {
                txtText.text = string.Empty;
                panelText.SetActive(false);
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
                await hub.PlayerSendLog(new TextLogModel()
                {
                    From = "A player",
                    Message = "Has left the game"
                });
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
