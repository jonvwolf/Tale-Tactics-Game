using Assets.Scripts;
using Assets.Scripts.Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneController : MonoBehaviour
{
    public TMP_Text LoadingText;
    public TMP_Text LoadingAssetText;
    public TMP_Text ErrorText;
    public Image ImageTest;
    public AudioSource AudioTest;
    public Button ButtonTest;
    public Button btnRetry;

    readonly bool IsDebug = Constants.IsDebug;

    // TODO: these dictionaries should be in Global
    readonly HashSet<long> LoadedImages = new();
    readonly HashSet<long> LoadedAudios = new();

    int TotalAssets = 0;
    int LoadedAssets = 0;

    bool quit = false;

    bool errorHappened;
    bool canExitBecauseError;
    // Start is called before the first frame update
    void Start()
    {
        Global.CurrentScene = Constants.LoadingSceneName;
        btnRetry.enabled = false;
        btnRetry.gameObject.SetActive(false);

        if (!IsDebug)
        {
            ImageTest.enabled = false;
            ButtonTest.enabled = false;
            ImageTest.gameObject.SetActive(false);
            ButtonTest.gameObject.SetActive(false);
        }
        else
        {
            ButtonTest.onClick.AddListener(() =>
            {
                AudioTest.Play();
            });
        }

        var model = Global.GetGameConfiguration();
        if (model == default)
        {
            SceneManager.LoadScene(Constants.MainSceneName);
            return;
        }

        LoadingText.text = $"Loading for {model.GameCode}...";

        if (LoadedAudios.Count != 0 || LoadedImages.Count != 0)
        {
            throw new Exception("This should not happen");
        }
        
        var loadedAssets = Global.CurrentGameModel;

        if (loadedAssets != null)
        {
            if (model.GameCode != loadedAssets.GameCode)
            {
                // different game code, delete cache
                loadedAssets.Dispose();
                Global.CurrentGameModel = default;
                loadedAssets = default;
            }
        }

        if (loadedAssets != null)
        {
            // set those that are already loaded
            foreach (var item in loadedAssets.LoadedImages)
            {
                LoadedImages.Add(item.Key);
                Debug.Log("Reusing image id: " + item.Key);
            }
            foreach (var item in loadedAssets.LoadedAudios)
            {
                LoadedAudios.Add(item.Key);
                Debug.Log("Reusing audio id: " + item.Key);
            }
        }
        else
        {
            // create a new game
            Global.CurrentGameModel = new CurrentGameModel(model.GameCode);
        }

        // TODO: these user settings code setup is COPIED CODE
        var settings = Global.GetCurrentUserSettings();
        Global.ApplyUserSettings(settings);

        Global.OnUserSettingsChanged += OnUserSettingsChanged;
        Global.OnExitGame += OnExitGame;

        btnRetry.onClick.AddListener(() =>
        {
            StartCoroutine(LoadAssets(model));
        });

        StartCoroutine(LoadAssets(model));
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
        if (canExitBecauseError)
        {
            // send signal it is okay
            Global.OkExitGame();
        }
    }

    void OnUserSettingsChanged(object sender, UserSettingsEventArgs args)
    {
        Global.ApplyUserSettings(args);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator LoadAssets(CurrentGameConfigurationModel model)
    {
        btnRetry.enabled = false;
        btnRetry.gameObject.SetActive(false);

        canExitBecauseError = false;
        errorHappened = false;
        TotalAssets = model.ReadGameConfigurationModel.Images.Count + model.ReadGameConfigurationModel.Audios.Count + model.ReadGameConfigurationModel.Minigames.Count;
        LoadedAssets = 0;

        foreach (var item in model.ReadGameConfigurationModel.Images)
        {
            if (quit)
            {
                break;
            }
            if (errorHappened)
            {
                break;
            }
            if (LoadedImages.Contains(item.Id))
            {
                LoadedAssets++;
                continue;
            }

            yield return new WaitForSeconds(Constants.WaitForSecondsAfterEachLoadAsset);
            yield return LoadAsset(item, null);
        }

        foreach (var item in model.ReadGameConfigurationModel.Audios)
        {
            if (quit)
            {
                break;
            }
            if (errorHappened)
            {
                break;
            }
            if (LoadedAudios.Contains(item.Id))
            {
                LoadedAssets++;
                continue;
            }

            yield return new WaitForSeconds(Constants.WaitForSecondsAfterEachLoadAsset);
            yield return LoadAsset(null, item);
        }

        LoadedAssets += model.ReadGameConfigurationModel.Minigames.Count;
        LoadingAssetText.text = $"Loaded asset {LoadedAssets} out of {TotalAssets}...";

        yield return new WaitForSeconds(Constants.WaitForSecondsAfterEachLoadAsset);

        if (quit)
        {
            Debug.Log("OK to exit...");
            Global.OkExitGame();
            yield break;
        }

        if (errorHappened)
        {
            yield break;
        }

        // TODO: if user is unlucky and clicks exit game just after previous check, it will not exit game but just change scene
        SceneManager.LoadScene(Constants.GameSceneName);
    }

    IEnumerator LoadAsset(ReadImageModel image, ReadAudioModel audio)
    {
        var requestSuccess = false;
        string url;
        LoadingAssetText.text = $"Loading asset {LoadedAssets} out of {TotalAssets}...";

        UnityWebRequest www = default;
        try
        {
            if (quit)
            {
                yield break;
            }

            if (image == default)
            {
                url = audio.AbsoluteUrl;
                www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG);
            }
            else
            {
                url = image.AbsoluteUrl;
                www = UnityWebRequestTexture.GetTexture(url);
            }

            Debug.Log("Calling url: " + url);
            yield return www.SendWebRequest();
            if (www.responseCode == 404)
            {
                ErrorText.text = $"File does not exist: {url}";
                btnRetry.enabled = true;
                btnRetry.gameObject.SetActive(true);
                errorHappened = true;
            }
            else if (www.responseCode >= 500 && www.responseCode <= 599)
            {
                ErrorText.text = "Server error. Returned response code: " + www.responseCode;
                Debug.LogError("Server error: " + www.downloadHandler.text);
                btnRetry.enabled = true;
                btnRetry.gameObject.SetActive(true);
                errorHappened = true;
            }
            else if (www.responseCode == 200)
            {
                try
                {
                    if (image == default)
                    {
                        var clip = DownloadHandlerAudioClip.GetContent(www);

                        if (IsDebug)
                        {
                            AudioTest.clip = clip;
                        }

                        LoadedAudios.Add(audio.Id);
                        Global.CurrentGameModel.AddAudio(audio.Id, new LoadedAudioAssetModel(clip, audio));
                    }
                    else
                    {
                        var texture = DownloadHandlerTexture.GetContent(www);

                        if (IsDebug)
                        {
                            var sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
                            ImageTest.sprite = sprite;
                        }

                        LoadedImages.Add(image.Id);
                        Global.CurrentGameModel.AddImage(image.Id, new LoadedImageAssetModel(texture, image));
                    }

                    LoadedAssets++;
                    requestSuccess = true;
                }
                catch (Exception e)
                {
                    ErrorText.text = "Exception happened: " + e.Message + ". Url: " + url;
                    btnRetry.enabled = true;
                    btnRetry.gameObject.SetActive(true);
                    errorHappened = true;
                    Debug.LogError("Exception happened: " + e.ToString());
                }
            }
            else if (www.result != UnityWebRequest.Result.Success)
            {
                ErrorText.text = "Web request was not successful: " + www.error;
                btnRetry.enabled = true;
                btnRetry.gameObject.SetActive(true);
                errorHappened = true;
            }
            else
            {
                ErrorText.text = "Unkown response code: " + www.responseCode + " Error: " + www.error;
                btnRetry.enabled = true;
                btnRetry.gameObject.SetActive(true);
                errorHappened = true;
            }
        }
        finally
        {
            // This only happens when this throws an exception
            // Because in all scenarios, if something is wrong (no exception)
            // errorHappened will be true
            if (!requestSuccess && !errorHappened)
            {
                // TODO: can't know the exception and outer try/finally doesn't work. NICE UNITY
                ErrorText.text = $"There was an error trying to load an asset, try again or exit the game and join again";
                btnRetry.enabled = true;
                btnRetry.gameObject.SetActive(true);
                errorHappened = true;
                canExitBecauseError = true;
            }
            if (www != default)
                www.Dispose();
        }
        
    }
}
