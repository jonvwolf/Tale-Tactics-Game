using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GlobalMenu : MonoBehaviour
{
    GameObject prefabOpenSettings;
    // Start is called before the first frame update
    void Start()
    {
        //prefabOpenSettings = Resources.Load("MenuPrefab/Open_settings_btn");
        //if(prefabOpenSettings == default)
        //{
        //    throw new Exception("prefab open settings does not exist");
        //}

        //Instantiate(prefabOpenSettings, Vector3.zero, Quaternion.identity);

        //prefabOpenSettings.transform.SetParent(CanvasToAttachTo.transform);

        var canvas = GameObject.FindGameObjectWithTag("maincanvas");
        if(canvas == default)
        {
            throw new Exception("canvas (maincanvas) is null");
        }
        // TODO: is this memory corruption?
        prefabOpenSettings = (GameObject)Instantiate(Resources.Load("MenuPrefab/Open_settings_btn", typeof(GameObject)));

        prefabOpenSettings.transform.SetParent(canvas.transform, false);

        Global.OnCanvasChangedForOptionsBtn += Global_OnCanvasChangedForOptionsBtn;

        Debug.Log("Open settings btn OK");
    }

    private void Global_OnCanvasChangedForOptionsBtn(object sender, Canvas e)
    {
        Destroy(prefabOpenSettings);
        prefabOpenSettings = (GameObject)Instantiate(Resources.Load("MenuPrefab/Open_settings_btn", typeof(GameObject)));

        prefabOpenSettings.transform.SetParent(e.transform, false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnDestroy()
    {
        Global.OnCanvasChangedForOptionsBtn -= Global_OnCanvasChangedForOptionsBtn;
        Destroy(prefabOpenSettings);
    }
}
