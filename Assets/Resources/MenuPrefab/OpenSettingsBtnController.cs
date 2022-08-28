using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenSettingsBtnController : MonoBehaviour
{
    public Button btnSelfReference;
    GameObject prefabCanvas1;
    // Start is called before the first frame update
    void Start()
    {
        if (prefabCanvas1 != default)
        {
            throw new System.Exception("prefabCanvas1 is not null");
        }
        // TODO: I don't really know why this works...
        // TODO: does this generate memory leaks?
        prefabCanvas1 = (GameObject)Instantiate(Resources.Load("MenuPrefab/OptionsPrefab", typeof(GameObject)));
        prefabCanvas1.SetActive(false);

        btnSelfReference.onClick.AddListener(() =>
        {
            prefabCanvas1.SetActive(true);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
