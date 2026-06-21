using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSystem : MonoBehaviour
{
    #region bindings
    private static readonly Dictionary<Key, MoveDirection> bindings = new()
    {
        { Key.W, MoveDirection.FORWARD },
        { Key.S, MoveDirection.BACK },
        { Key.A, MoveDirection.LEFT },
        { Key.D, MoveDirection.RIGHT },
        { Key.Q, MoveDirection.TURN_LEFT },
        { Key.E, MoveDirection.TURN_RIGHT },
        { Key.Space, MoveDirection.UP },
        { Key.LeftCtrl, MoveDirection.DOWN }
    };
    #endregion
    
    public Action<MoveDirection> Move;
    public Action<MoveDirection> Up; 
    public Action<MoveDirection> Down; 
    
    private void FixedUpdate()
    {
        foreach (var (key, direction) in bindings)
        {
            if (Keyboard.current[key].isPressed)
                Move?.Invoke(direction);
        }
    }
    
    private void Update()
    {
        foreach (var (key, direction) in bindings)
        {
            if (Keyboard.current[key].wasPressedThisFrame)
                Down?.Invoke(direction);
            
            if (Keyboard.current[key].wasReleasedThisFrame)
                Up?.Invoke(direction);
        }
    }
}
