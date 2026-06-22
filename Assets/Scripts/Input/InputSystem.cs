using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSystem : MonoBehaviour
{
    #region bindings
    private static readonly Dictionary<Key, Direction> bindings = new()
    {
        { Key.W, Direction.FORWARD },
        { Key.S, Direction.BACK },
        { Key.A, Direction.LEFT },
        { Key.D, Direction.RIGHT },
        { Key.Q, Direction.TURN_LEFT },
        { Key.E, Direction.TURN_RIGHT },
        { Key.Space, Direction.UP },
        { Key.LeftCtrl, Direction.DOWN }
    };
    #endregion
    
    public Action<Direction> Move;
    public Action<Direction> Up; 
    public Action<Direction> Down; 
    
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
