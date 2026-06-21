using System;
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
    
    [Header("Pitch")]
    [SerializeField] private float forwardTiltForce;
    
    [Header("Roll")]
    [SerializeField] private float turnTiltForce;
    
    [Header("Yaw")]
    [SerializeField] private float torqueCoefficient;
    
    private InputSystem inputSystem;
    private Vector2 tiltForce;
    private bool grounded;
    private bool isEngineAccelerationChanged;
    private float engineAcceleration;
    private float targetAcceleration;
    
    public void Init(InputSystem input)
    {
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
        
        IndicatorsUpdate?.Invoke(engineAcceleration, transform.position.y);
    }

    private void ApplyAcceleration()
    {
        rigidbody.AddRelativeForce(new Vector3(0, engineAcceleration * rigidbody.mass));
        
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
        var x = tiltForce.x switch
        {
            > 0 => -Time.fixedDeltaTime,
            < 0 => Time.fixedDeltaTime,
            _ => 0
        };
        
        var y = tiltForce.y switch
        {
            > 0 => -Time.fixedDeltaTime,
            < 0 => Time.fixedDeltaTime,
            _ => 0
        };

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
                    y = Time.fixedDeltaTime;
                
                break;
            case MoveDirection.BACK:
                if (grounded)
                    Message?.Invoke("You are on ground");
                else
                    y = -Time.fixedDeltaTime;
                
                break;
            case MoveDirection.LEFT:
                if (grounded)
                    Message?.Invoke("You are on ground");
                else
                    x = -Time.fixedDeltaTime;
                
                break;
            case MoveDirection.RIGHT:
                if (grounded)
                    Message?.Invoke("You are on ground");
                else
                    x = Time.fixedDeltaTime;
                
                break;
            case MoveDirection.TURN_LEFT:
                if (grounded)
                    Message?.Invoke("You are on ground");
                else
                    rigidbody.AddRelativeTorque(0, -torqueCoefficient * rigidbody.mass, 0);    
                
                break;
            case MoveDirection.TURN_RIGHT:
                if (grounded)
                    Message?.Invoke("You are on ground");
                else
                    rigidbody.AddRelativeTorque(0, torqueCoefficient * rigidbody.mass, 0);
                
                break;
        }
        
        tiltForce.x = Mathf.Clamp(tiltForce.x + x, -1, 1);
        tiltForce.y = Mathf.Clamp(tiltForce.y + y, -1, 1);
    }

    private void KeyUpListener(MoveDirection moveDirection)
    {
        if (moveDirection == MoveDirection.UP || moveDirection == MoveDirection.DOWN)
            isEngineAccelerationChanged = false;
    }
    
    private void KeyDownListener(MoveDirection moveDirection)
    {
        if (moveDirection == MoveDirection.UP || moveDirection == MoveDirection.DOWN)
            isEngineAccelerationChanged = true;
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
