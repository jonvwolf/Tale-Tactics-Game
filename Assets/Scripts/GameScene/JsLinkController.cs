using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsLinkController : MonoBehaviour
{
    public void InvokeEvent(string type, string data)
    {
        Debug.Log($"Called InvokeEvent: {type} Data: {data}");
        Global.JsLinkEventArgs(new Assets.Scripts.Models.JsLinkEventArgs()
        {
            Data = data,
            Type = type
        });
    }
}
