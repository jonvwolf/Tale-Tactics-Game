using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class SignalRWrapper : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void Hello();

    [DllImport("__Internal")]
    private static extern int AddNumbers(int x, int y);

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Started");
        Hello();
        Debug.Log("Result is: " + AddNumbers(1, 3));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
