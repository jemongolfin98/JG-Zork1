using UnityEngine;
using System;
using TMPro;
using Zork;

public class UnityInputService : MonoBehaviour, IInputService
{
    public TMP_InputField InputField;
    
    public event EventHandler<string> InputReceived;

    public void ProcessInput()
    {
        Assert.IsNotNull(InputField);
        Assert.IsFalse(string.IsNullOrEmpty(InputField.text));
        InputReceived?.Invoke(this, InputField.text);
        InputField.text = string.Empty;
    }
}
