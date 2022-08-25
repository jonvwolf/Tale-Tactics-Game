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

    readonly bool IsDebug = Constants.IsDebug;

    // TODO: these dictionaries should be in Global
    readonly Dictionary<long, LoadedImageAssetModel> LoadedImages = new();
    readonly Dictionary<long, LoadedAudioAssetModel> LoadedAudios = new();

    int TotalAssets = 0;
    int LoadedAssets = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (!IsDebug)
        {
            ImageTest.enabled = false;
            ButtonTest.enabled = false;
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
            // set those that are already loaded
            foreach (var item in loadedAssets.LoadedImages)
            {
                LoadedImages.Add(item.Key, item.Value);
                Debug.Log("Reusing image id: " + item.Key);
                LoadedAssets++;
            }
            foreach (var item in loadedAssets.LoadedAudios)
            {
                LoadedAudios.Add(item.Key, item.Value);
                Debug.Log("Reusing audio id: " + item.Key);
                LoadedAssets++;
            }
        }

        StartCoroutine(LoadAssets(model));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator LoadAssets(CurrentGameConfigurationModel model)
    {
        TotalAssets = model.ReadGameConfigurationModel.Images.Count + model.ReadGameConfigurationModel.Audios.Count + model.ReadGameConfigurationModel.Minigames.Count;
        LoadedAssets = 0;

        foreach(var item in model.ReadGameConfigurationModel.Images)
        {
            if (LoadedImages.ContainsKey(item.Id))
                continue;

            yield return new WaitForSeconds(Constants.WaitForSecondsAfterEachLoadAsset);
            yield return LoadAsset(item, null);
        }

        foreach (var item in model.ReadGameConfigurationModel.Audios)
        {
            if (LoadedAudios.ContainsKey(item.Id))
                continue;

            yield return new WaitForSeconds(Constants.WaitForSecondsAfterEachLoadAsset);
            yield return LoadAsset(null, item);
        }
        //todo: aqui me quede -> when loading and error, it should show a retry load assets button and loaded assets should not go to waste
        // . it should actively be adding to the Globals so that they are not lost. But have an option for the user to clear "cache"
        LoadedAssets += model.ReadGameConfigurationModel.Minigames.Count;
        LoadingAssetText.text = $"Loaded asset {LoadedAssets} out of {TotalAssets}...";

        yield return new WaitForSeconds(Constants.WaitForSecondsAfterEachLoadAsset);

        SceneManager.LoadScene(Constants.GameSceneName);
    }

    IEnumerator LoadAsset(ReadImageModel image, ReadAudioModel audio)
    {
        string url;
        LoadingAssetText.text = $"Loading asset {LoadedAssets} out of {TotalAssets}...";

        UnityWebRequest www = default;
        try
        {
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

            yield return www.SendWebRequest();
            if (www.responseCode == 404)
            {
                ErrorText.text = $"File does not exist: {url}";
            }
            else if (www.responseCode >= 500 && www.responseCode <= 599)
            {
                ErrorText.text = "Server error. Returned response code: " + www.responseCode;
                Debug.LogError("Server error: " + www.downloadHandler.text);
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

                        LoadedAudios.Add(audio.Id, new LoadedAudioAssetModel(clip, audio));
                    }
                    else
                    {
                        var texture = DownloadHandlerTexture.GetContent(www);

                        if (IsDebug)
                        {
                            var sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
                            ImageTest.sprite = sprite;
                        }

                        LoadedImages.Add(image.Id, new LoadedImageAssetModel(texture, image));
                    }

                    LoadedAssets++;
                }
                catch (Exception e)
                {
                    ErrorText.text = "Exception happened: " + e.Message + ". Url: " + url;
                    throw;
                }
            }
            else if (www.result != UnityWebRequest.Result.Success)
            {
                ErrorText.text = "Web request was not successful: " + www.error;
            }
            else
            {
                ErrorText.text = "Unkown response code: " + www.responseCode + " Error: " + www.error;
            }
        }
        finally
        {
            if (www != default)
                www.Dispose();
        }
        
    }
}
