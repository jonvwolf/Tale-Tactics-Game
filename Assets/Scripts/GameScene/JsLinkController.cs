using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class JsLinkController : MonoBehaviour
{
    /// <summary>
    /// For this to work, create a gameobject (empty 2d) in the scene named `JsLink`
    /// Then attach this monobehaviour to the game object, thats it
    /// (In your outside js get the unity game reference and call it example: 
    /// `_window.HtUnityInstance.SendMessage('JsLink', 'InvokeEvent', 'type test', 'hi from js');`
    /// </summary>
    /// <param name="type"></param>
    /// <param name="data"></param>
    public void InvokeEvent(string data)
    {
        Debug.Log($"Called InvokeEvent Data: {data}");

        // data must be of this format (without quotes)
        // "/*TYPE_GOES_HERE*/{...json...}"

        if (string.IsNullOrWhiteSpace(data) || data[0] != '/' || data[1] != '*')
        {
            Debug.LogError("Wrong format for data");
            return;
        }

        var sb = new StringBuilder(22);
        for (int i = 2; i < data.Length; i++)
        {
            if (data[i] == '*')
            {
                break;
            }
            sb.Append(data[i]);
        }

        Global.JsLinkEventArgs(new Assets.Scripts.Models.JsLinkEventArgs()
        {
            Data = data,
            Type = sb.ToString(),
        });
    }
}
