using System;
using System.Collections.Generic;
using UnityEngine;

public class Helicopter : MonoBehaviour
{
    public Action<string> Message;
    public Action<float, float> IndicatorsUpdate;
    public Action Destroy;
    
    [Header("Physics")]
    [SerializeField] private Rigidbody rigidbody;

    [Header("Engine")] 
    [SerializeField] private float upForce;
    [SerializeField] private float downForce;
    [SerializeField] private float forwardForce;
    
    [Header("Pitch")]
    [SerializeField] private float forwardTiltForce;
    
    [Header("Roll")]
    [SerializeField] private float turnTiltForce;
    
    [Header("Yaw")]
    [SerializeField] private float torqueCoefficient;
    
    private InputSystem inputSystem;
    private List<MoveDirection> moveDirections;
    private Vector2 tiltForce;
    private Vector2 tilt;
    private bool grounded;
    private bool isEngineAccelerationChanged;
    private float engineAcceleration;
    private float targetAcceleration;
    
    public void Init(InputSystem input)
    {
        moveDirections = new List<MoveDirection>();
        inputSystem = input;
        targetAcceleration = Physics.gravity.magnitude;
        inputSystem.Move += MovementListener;
        inputSystem.Up += KeyUpListener;
        inputSystem.Down += KeyDownListener;
    }
    
    private void FixedUpdate()
    {
        ApplyAcceleration();
        ApplyTilt();
        
        if (moveDirections.Count == 0)
            tiltForce = Vector2.Lerp(tiltForce, Vector2.zero, Time.fixedDeltaTime);
        
        IndicatorsUpdate?.Invoke(engineAcceleration, transform.position.y);
    }

    private void ApplyAcceleration()
    {
        rigidbody.AddRelativeForce(new Vector3(0, engineAcceleration, tiltForce.y * forwardForce), ForceMode.Acceleration);
        
        if (isEngineAccelerationChanged)
            return;
        
        if (grounded)
        {
            engineAcceleration = Mathf.Clamp(engineAcceleration - downForce, 0, engineAcceleration);
            return;
        }
        
        if (engineAcceleration > targetAcceleration)
            engineAcceleration = Mathf.Clamp(engineAcceleration - downForce, targetAcceleration, engineAcceleration);
        else if (engineAcceleration < targetAcceleration)
            engineAcceleration = Mathf.Clamp(engineAcceleration + upForce, engineAcceleration, targetAcceleration);
    }
    
    private void ApplyTilt()
    {
        var pitch = tiltForce.y * forwardTiltForce;
        var roll = -tiltForce.x * turnTiltForce;
        rigidbody.AddRelativeTorque(pitch, 0, roll);
    }
    
    private void MovementListener(MoveDirection moveDirection)
    {
        switch (moveDirection)
        {
            case MoveDirection.UP:
                engineAcceleration += upForce;
                break;
            case MoveDirection.DOWN:
                if (grounded)
                    Message?.Invoke("You are on ground");
                else
                    engineAcceleration = Mathf.Max(0, engineAcceleration - downForce);
                
                break;
            case MoveDirection.FORWARD:
                if (grounded)
                    Message?.Invoke("You are on ground");
                else
                    tiltForce.y = Mathf.Clamp(tiltForce.y + Time.fixedDeltaTime, -1, 1);
                
                break;
            case MoveDirection.BACK:
                if (grounded)
                    Message?.Invoke("You are on ground");
                else
                    tiltForce.y = Mathf.Clamp(tiltForce.y - Time.fixedDeltaTime, -1, 1);
                
                break;
            case MoveDirection.LEFT:
                if (grounded)
                    Message?.Invoke("You are on ground");
                else
                    tiltForce.x = Mathf.Clamp(tiltForce.x - Time.fixedDeltaTime, -1, 1);
                
                break;
            case MoveDirection.RIGHT:
                if (grounded)
                    Message?.Invoke("You are on ground");
                else
                    tiltForce.x = Mathf.Clamp(tiltForce.x + Time.fixedDeltaTime, -1, 1);
                
                break;
            case MoveDirection.TURN_LEFT:
                if (grounded)
                    Message?.Invoke("You are on ground");
                else
                    rigidbody.AddRelativeTorque(0, -torqueCoefficient, 0, ForceMode.Acceleration);    
                
                break;
            case MoveDirection.TURN_RIGHT:
                if (grounded)
                    Message?.Invoke("You are on ground");
                else
                    rigidbody.AddRelativeTorque(0, torqueCoefficient, 0, ForceMode.Acceleration);
                
                break;
        }
    }

    private void KeyUpListener(MoveDirection moveDirection)
    {
        if (moveDirection == MoveDirection.UP || moveDirection == MoveDirection.DOWN)
            isEngineAccelerationChanged = false;
        else
            moveDirections.Remove(moveDirection);
    }

    private void KeyDownListener(MoveDirection moveDirection)
    {
        if (moveDirection == MoveDirection.UP || moveDirection == MoveDirection.DOWN)
            isEngineAccelerationChanged = true;
        else
            moveDirections.Add(moveDirection);
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
            inputSystem.Move -= MovementListener;
            inputSystem.Up -= KeyUpListener;
            inputSystem.Down -= KeyDownListener;
        }
        
        Destroy?.Invoke();
    }
}
