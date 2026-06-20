using System;
using UnityEngine;

public class Helicopter : MonoBehaviour
{
    public Action<string> Message;
    public Action<float, float> IndicatorsUpdate;
    public Action Destroy;
    
    [SerializeField] private Rigidbody rigidbody;
    
    [Header("Engine")]
    [SerializeField] private float heightLimit;
    [SerializeField] private float upForce;
    [SerializeField] private float downForce;
    
    [Header("Movement")]
    [SerializeField] private float turnTiltForcePercent;
    [SerializeField] private float forwardForce;
    [SerializeField] private float turnForce;
    
    [Header("Tilt")]
    [SerializeField] private float turnTiltForce;
    [SerializeField] private float forwardTiltForce;
    [SerializeField] private float turnForcePercent;
    
    private InputSystem inputSystem;
    private Vector2 yForce;
    private Vector2 hTilt;
    private bool grounded;
    private float engineAcceleration;
    private float hTurn;
    
    public void Init(InputSystem input)
    {
        inputSystem = input;
        inputSystem.Move += Movement;
        inputSystem.Up += KeyUp;
        inputSystem.Down += KeyDown;
    }
    
    private void FixedUpdate()
    {
        rigidbody.AddRelativeForce(new Vector3(0, engineAcceleration * rigidbody.mass));
        ApplyTorque();
        ApplyTilt();
        IndicatorsUpdate?.Invoke(engineAcceleration, transform.position.y);
    }
    
    private void ApplyTorque()
    {
        var turn = turnForce * Mathf.Lerp(yForce.x, yForce.x * (turnTiltForcePercent - Mathf.Abs(yForce.y)), Mathf.Max(0, yForce.y));
        hTurn = Mathf.Lerp(hTurn, turn, Time.fixedDeltaTime * turnForce);
        rigidbody.AddRelativeTorque(0, hTurn * rigidbody.mass, 0);
        rigidbody.AddRelativeForce(Vector3.forward * Mathf.Max(0f, yForce.y * forwardForce * rigidbody.mass));
    }

    private void ApplyTilt()
    {
        hTilt.x = Mathf.Lerp(hTilt.x, yForce.x * turnTiltForce, Time.deltaTime);
        hTilt.y = Mathf.Lerp(hTilt.y, yForce.y * forwardTiltForce, Time.deltaTime);
        transform.localRotation = Quaternion.Euler(hTilt.y, transform.localEulerAngles.y, -hTilt.x);
    }
    
    private void Movement(MoveDirection direction)
    {
        var y = 0f;
        var x = 0f;
        var torqueForce = 0f;
        
        if (yForce.y > 0)
            y = - Time.fixedDeltaTime;
        else if (yForce.y < 0)
            y = Time.fixedDeltaTime;
        
        if (yForce.x > 0)
            x = -Time.fixedDeltaTime;
        else if (yForce.x < 0)
            x = Time.fixedDeltaTime;
        
        switch (direction)
        {
            case MoveDirection.UP:
                engineAcceleration += upForce;
                break;
            case MoveDirection.DOWN:
                if (grounded)
                {
                    Message?.Invoke("You are on ground");
                    break;
                }
                
                engineAcceleration = Mathf.Max(0, engineAcceleration - downForce);
                break;
            case MoveDirection.FORWARD:
                if (grounded)
                {
                    Message?.Invoke("You are on ground");
                    break;
                }
                
                y = Time.fixedDeltaTime;
                break;
            case MoveDirection.BACK:
                if (grounded)
                {
                    Message?.Invoke("You are on ground");
                    break;
                }
                
                y = -Time.fixedDeltaTime;
                break;
            case MoveDirection.LEFT:
                if (grounded)
                {
                    Message?.Invoke("You are on ground");
                    break;
                }
                
                x = -Time.fixedDeltaTime;
                break;
            case MoveDirection.RIGHT:
                if (grounded)
                {
                    Message?.Invoke("You are on ground");
                    break;
                }
                
                x = Time.fixedDeltaTime;
                break;
            case MoveDirection.TURN_LEFT:
                if (grounded)
                {
                    Message?.Invoke("You are on ground");
                    break;
                }
                
                torqueForce = -(turnForcePercent - Mathf.Abs(yForce.y)) * rigidbody.mass;
                rigidbody.AddRelativeTorque(0, torqueForce, 0);
                break;
            case MoveDirection.TURN_RIGHT:
                if (grounded)
                {
                    Message?.Invoke("You are on ground");
                    break;
                }
                        
                torqueForce = (turnForcePercent - Mathf.Abs(yForce.y)) * rigidbody.mass;
                rigidbody.AddRelativeTorque(0, torqueForce, 0);
                break;
        }
        
        yForce.x = Mathf.Clamp(yForce.x + x, -1, 1);
        yForce.y = Mathf.Clamp(yForce.y + y, -1, 1);
    }
    
    private void KeyUp(MoveDirection direction)
    {
        switch (direction)
        {
            case MoveDirection.FORWARD:
                break;
            case MoveDirection.BACK:
                break;
            case MoveDirection.LEFT:
                break;
            case MoveDirection.RIGHT:
                break;
            case MoveDirection.TURN_LEFT:
                break;
            case MoveDirection.TURN_RIGHT:
                break;
        }
    }
    
    private void KeyDown(MoveDirection direction)
    {
        switch (direction)
        {
            case MoveDirection.FORWARD:
                break;
            case MoveDirection.BACK:
                break;
            case MoveDirection.LEFT:
                break;
            case MoveDirection.RIGHT:
                break;
            case MoveDirection.TURN_LEFT:
                break;
            case MoveDirection.TURN_RIGHT:
                break;
        }
    }
    
    private void OnCollisionEnter()
    {
        grounded = true;
    }

    private void OnCollisionExit()
    {
        grounded = false;
    }

    private void OnDestroy()
    {
        if (inputSystem)
        {
            inputSystem.Move -= Movement;
            inputSystem.Up -= KeyUp;
            inputSystem.Down -= KeyDown;
        }
        
        Destroy?.Invoke();
    }
}
