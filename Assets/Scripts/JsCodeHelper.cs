#if UNITY_WEBGL
using Assets.Scripts.Hub;
using Assets.Scripts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class JsCodeHelper : IJsCodeHelper
    {
        [DllImport("__Internal")]
        private static extern void GetCodeJs();

        Action<string> ReturnCodeAction;
        bool disposedValue;

        public JsCodeHelper(Action<string> action)
        {
            ReturnCodeAction = action;
            Global.OnJsLinkEvent += Global_OnJsLinkEvent;
        }

        private void Global_OnJsLinkEvent(object sender, JsLinkEventArgs e)
        {
            const string OnCodeType = "OnCode";
            if (string.IsNullOrWhiteSpace(e.Data) || string.IsNullOrWhiteSpace(e.Type))
            {
                Debug.LogError("Type or Data is empty");
                return;
            }
            if (e.Type == OnCodeType)
            {
                Debug.Log($"Called {OnCodeType}");
                var codeModel = HtHubConnectionWebGl.CheckJsLinkData<JsCode>(e.Data);
                if (codeModel != default)
                {
                    if (string.IsNullOrWhiteSpace(codeModel.Code))
                    {
                        Debug.LogError("Code is null or whitespace...");
                        return;
                    }
                    ReturnCodeAction(codeModel.Code);
                }
            }
        }

        public void GetCode()
        {
            // This will make JS raise the JS link event
            GetCodeJs();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Global.OnJsLinkEvent -= Global_OnJsLinkEvent;
                    ReturnCodeAction = default;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
#endif