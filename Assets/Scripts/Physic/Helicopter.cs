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
    [SerializeField] private float turnTiltForce;
    [SerializeField] private float forwardTiltForce;
    [SerializeField] private float turnForcePercent;
    [SerializeField] private float tiltCoefficient;
    
    private InputSystem inputSystem;
    private Vector2 torqueForce;
    private Dimension direction;
    private Dimension turn;
    private Dimension torque;
    private bool grounded;
    private float engineAcceleration;
    private float hTurn;
    
    public void Init(InputSystem input)
    {
        inputSystem = input;
        inputSystem.Move += MovementListener;
        inputSystem.Up += KeyUp;
        inputSystem.Down += KeyDown;
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
        rigidbody.AddRelativeForce(new Vector3(0, engineAcceleration * rigidbody.mass, 0));
    }
    
    private void ApplyTorque()
    {
        var turn = turnForce * Mathf.Lerp(torqueForce.x, torqueForce.x * (turnTiltForcePercent - Mathf.Abs(torqueForce.y)), Mathf.Max(0, torqueForce.y));
        hTurn = Mathf.Lerp(hTurn, turn, Time.fixedDeltaTime * turnForce);
        
        rigidbody.AddRelativeTorque(0, hTurn * rigidbody.mass, 0);
        rigidbody.AddRelativeForce(new Vector3(0, 0, Mathf.Max(0f, torqueForce.y * forwardForce * rigidbody.mass)));
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
        var torqueForce = 0f;
        
        var x = this.torqueForce.x switch
        {
            > 0 => -Time.fixedDeltaTime,
            < 0 => Time.fixedDeltaTime,
            _ => 0
        };
        
        var y = this.torqueForce.y switch
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
                if (direction == Dimension.FORWARD)
                    y = Time.fixedDeltaTime;
                break;
            case MoveDirection.BACK:
                if (grounded)
                {
                    Message?.Invoke("You are on ground");
                    break;
                }
                
                if (direction == Dimension.BACKWARD)
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
                
                torqueForce = -(turnForcePercent - Mathf.Abs(this.torqueForce.y)) * rigidbody.mass;
                rigidbody.AddRelativeTorque(0, torqueForce, 0);
                break;
            case MoveDirection.TURN_RIGHT:
                if (grounded)
                {
                    Message?.Invoke("You are on ground");
                    break;
                }
                        
                torqueForce = (turnForcePercent - Mathf.Abs(this.torqueForce.y)) * rigidbody.mass;
                rigidbody.AddRelativeTorque(0, torqueForce, 0);
                break;
        }
        
        this.torqueForce.x = Mathf.Clamp(this.torqueForce.x + x, -1, 1);
        this.torqueForce.y = Mathf.Clamp(this.torqueForce.y + y, -1, 1);
    }
    
    private void KeyUp(MoveDirection moveDirection)
    {
        switch (moveDirection)
        {
            case MoveDirection.FORWARD:
                if (direction == Dimension.FORWARD)
                    direction = Dimension.NONE;
                break;
            case MoveDirection.BACK:
                if (direction == Dimension.BACKWARD)
                    direction = Dimension.NONE;
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
    
    private void KeyDown(MoveDirection moveDirection)
    {
        if (moveDirection != MoveDirection.UP && grounded)
        {
            Message?.Invoke("You are on ground");
            return;
        }
        
        switch (moveDirection)
        {
            case MoveDirection.FORWARD:
                if (direction == Dimension.NONE)
                    direction = Dimension.FORWARD;
                else
                    Message?.Invoke("You move backward");
                break;
            case MoveDirection.BACK:
                if (direction == Dimension.NONE)
                    direction = Dimension.BACKWARD;
                else
                    Message?.Invoke("You move forward");
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
            inputSystem.Move -= MovementListener;
            inputSystem.Up -= KeyUp;
            inputSystem.Down -= KeyDown;
        }
        
        Destroy?.Invoke();
    }
}
