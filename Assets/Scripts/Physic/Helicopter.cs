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
    [SerializeField] private float forwardForce;
    [SerializeField] private float sideForce;
    
    [Header("Pitch")]
    [SerializeField] private float forwardTiltForce;
    
    [Header("Roll")]
    [SerializeField] private float turnTiltForce;
    
    [Header("Yaw")]
    [SerializeField] private float torqueCoefficient;
    
    private InputSystem inputSystem;
    private Vector2 tiltForce;
    private Vector2 tilt;
    private bool grounded;
    private bool isEngineAccelerationChanged;
    private bool isRolling;
    private bool isPitching;
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
        ApplyAlignment();
        
        IndicatorsUpdate?.Invoke(engineAcceleration, transform.position.y);
    }

    private void ApplyAcceleration()
    {
        rigidbody.AddRelativeForce(new Vector3(tiltForce.x * sideForce, engineAcceleration, tiltForce.y * forwardForce), ForceMode.Acceleration);
        
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
        tilt.x = Mathf.Lerp(tilt.x, tiltForce.x * turnTiltForce, Time.fixedDeltaTime);
        tilt.y = Mathf.Lerp(tilt.y, tiltForce.y * forwardTiltForce, Time.fixedDeltaTime);
        
        rigidbody.MoveRotation(Quaternion.Euler(tilt.y, transform.localEulerAngles.y, -tilt.x));
    }

    private void ApplyAlignment()
    {
        if (!isPitching)
            tiltForce.y = Mathf.Lerp(tiltForce.y, 0, Time.fixedDeltaTime);

        if (!isRolling)
            tiltForce.x = Mathf.Lerp(tiltForce.x, 0, Time.fixedDeltaTime);
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
        if (moveDirection == MoveDirection.FORWARD || moveDirection == MoveDirection.BACK)
            isPitching = false;
        else if (moveDirection == MoveDirection.UP || moveDirection == MoveDirection.DOWN)
            isEngineAccelerationChanged = false;
        else if (moveDirection == MoveDirection.LEFT || moveDirection == MoveDirection.RIGHT)
            isRolling = false;
    }

    private void KeyDownListener(MoveDirection moveDirection)
    {
        if (moveDirection == MoveDirection.FORWARD || moveDirection == MoveDirection.BACK)
            isPitching = true;
        else if (moveDirection == MoveDirection.UP || moveDirection == MoveDirection.DOWN)
            isEngineAccelerationChanged = true;
        else if (moveDirection == MoveDirection.LEFT || moveDirection == MoveDirection.RIGHT)
            isRolling = true;
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
