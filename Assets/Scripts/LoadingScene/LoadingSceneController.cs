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
    readonly Dictionary<long, ReadImageModel> LoadedImages = new Dictionary<long, ReadImageModel>();
    readonly Dictionary<long, ReadAudioModel> LoadedAudios = new Dictionary<long, ReadAudioModel>();

    int TotalAssets = 0;
    int LoadedAssets = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (!IsDebug)
        {
            ImageTest.enabled = false;
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

            yield return LoadAsset(item, null);
        }

        foreach (var item in model.ReadGameConfigurationModel.Audios)
        {
            if (LoadedAudios.ContainsKey(item.Id))
                continue;

            yield return LoadAsset(null, item);
        }
    }

    IEnumerator LoadAsset(ReadImageModel image, ReadAudioModel audio)
    {
        string url;

        if (image == default)
        {
            url = audio.AbsoluteUrl;
        }
        else
        {
            url = image.AbsoluteUrl;
        }

        LoadingAssetText.text = $"Loading asset {LoadedAssets} out of {TotalAssets}...";

        using var www = UnityWebRequestTexture.GetTexture(url);
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

                    AudioTest.clip = clip;

                    LoadedAudios.Add(audio.Id, audio);
                }
                else
                {
                    var texture = DownloadHandlerTexture.GetContent(www);

                    if (IsDebug)
                    {
                        var sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
                        ImageTest.sprite = sprite;
                    }

                    LoadedImages.Add(image.Id, image);
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
}
