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
    
    [Header("Torque")]
    [SerializeField] private float turnTiltForcePercent;
    [SerializeField] private float forwardForce;
    [SerializeField] private float turnForce;
    
    [Header("Tilt")]
    [SerializeField] private float sideForce;
    [SerializeField] private float turnTiltForce;
    [SerializeField] private float forwardTiltForce;
    [SerializeField] private float turnForcePercent;
    [SerializeField] private float tiltCoefficient;
    
    private InputSystem inputSystem;
    private Vector2 torqueForce;
    private bool grounded;
    private bool isEngineAccelerationChanged;
    private float engineAcceleration;
    private float targetAcceleration;
    private float turnRate;
    
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
        ApplyTorque();
        ApplyTilt();
        
        IndicatorsUpdate?.Invoke(engineAcceleration, transform.position.y);
    }

    private void ApplyAcceleration()
    {
        rigidbody.AddRelativeForce(new Vector3(0, engineAcceleration * rigidbody.mass));
        
        if (grounded || isEngineAccelerationChanged)
            return;
        
        if (engineAcceleration > targetAcceleration)
            engineAcceleration = Mathf.Clamp(engineAcceleration - downForce, targetAcceleration, engineAcceleration);
        else if (engineAcceleration < targetAcceleration)
            engineAcceleration = Mathf.Clamp(engineAcceleration + upForce, engineAcceleration, targetAcceleration);
    }
    
    private void ApplyTorque()
    {
        var turn = turnForce * Mathf.Lerp(torqueForce.x, torqueForce.x * (turnTiltForcePercent - Mathf.Abs(torqueForce.y)), Mathf.Max(0, torqueForce.y));
        turnRate = Mathf.Lerp(turnRate, turn, Time.fixedDeltaTime * turnForce);
        rigidbody.AddRelativeTorque(0, turnRate * rigidbody.mass, 0);
        rigidbody.AddRelativeForce(new Vector3(torqueForce.x * sideForce * rigidbody.mass, 0, torqueForce.y * forwardForce * rigidbody.mass));
    }

    private void ApplyTilt()
    {
        var roll = -torqueForce.x * turnTiltForce;
        var pitch = torqueForce.y * forwardTiltForce;
        var targetRotation = Quaternion.Euler(pitch, rigidbody.rotation.eulerAngles.y, roll);

        rigidbody.MoveRotation(Quaternion.Slerp(rigidbody.rotation, targetRotation, Time.fixedDeltaTime * tiltCoefficient));
    }
    
    private void MovementListener(MoveDirection moveDirection)
    {
        var force = 0f;
        
        var x = torqueForce.x switch
        {
            > 0 => -Time.fixedDeltaTime,
            < 0 => Time.fixedDeltaTime,
            _ => 0
        };
        
        var y = torqueForce.y switch
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
                
                force = Mathf.Abs(torqueForce.y) - turnForcePercent;
                rigidbody.AddRelativeTorque(0, force * rigidbody.mass, 0);
                break;
            case MoveDirection.TURN_RIGHT:
                if (grounded)
                {
                    Message?.Invoke("You are on ground");
                    break;
                }
                
                force = turnForcePercent - Mathf.Abs(torqueForce.y);
                rigidbody.AddRelativeTorque(0, force * rigidbody.mass, 0);
                break;
        }
        
        torqueForce.x = Mathf.Clamp(torqueForce.x + x, -1, 1);
        torqueForce.y = Mathf.Clamp(torqueForce.y + y, -1, 1);
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
