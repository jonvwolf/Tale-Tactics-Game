using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GlobalMenu : MonoBehaviour
{
    public GameObject prefabOpenSettings;
    public Canvas CanvasToAttachTo;
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


        prefabOpenSettings = (GameObject)Instantiate(Resources.Load("MenuPrefab/Open_settings_btn", typeof(GameObject)));

        prefabOpenSettings.transform.SetParent(CanvasToAttachTo.transform);
        prefabOpenSettings.transform.localScale = Vector3.one;
        prefabOpenSettings.transform.localRotation = Quaternion.Euler(Vector3.zero);
        prefabOpenSettings.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, 0, 0);

        Debug.Log("Open settings btn OK");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
