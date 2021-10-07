using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class PlayerInput : MonoBehaviour
{    
    [Serializable]
    public struct KeyFunction
    {
        public KeyCode key;
        public UnityEvent function;
    }

    public KeyFunction[] keys;


    void Update()
    {
        for (int i = 0; i < keys.Length; i++)
        {
            if (Input.GetKeyDown(keys[0].key)) keys[0].function.Invoke();
        }
    }
}



