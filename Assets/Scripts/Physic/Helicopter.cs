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
    private Vector2 heighForce;
    private Vector2 hTilt;
    private bool grounded;
    private float engineForce;
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
        ApplyLift();
        ApplyTilt();
        IndicatorsUpdate?.Invoke(engineForce, transform.position.y);
    }
    
    private void ApplyLift()
    {
        var force = Mathf.Lerp(0, engineForce, 1 - Mathf.Clamp01(transform.position.y / heightLimit));
        rigidbody.AddRelativeForce(new Vector3(0, force * rigidbody.mass));
        
        var turn = turnForce * Mathf.Lerp(heighForce.x, heighForce.x * (turnTiltForcePercent - Mathf.Abs(heighForce.y)), Mathf.Max(0, heighForce.y));
        hTurn = Mathf.Lerp(hTurn, turn, Time.fixedDeltaTime * turnForce);
        rigidbody.AddRelativeTorque(0f, hTurn * rigidbody.mass, 0);
        rigidbody.AddRelativeForce(Vector3.forward * Mathf.Max(0f, heighForce.y * forwardForce * rigidbody.mass));
    }

    private void ApplyTilt()
    {
        hTilt.x = Mathf.Lerp(hTilt.x, heighForce.x * turnTiltForce, Time.deltaTime);
        hTilt.y = Mathf.Lerp(hTilt.y, heighForce.y * forwardTiltForce, Time.deltaTime);
        transform.localRotation = Quaternion.Euler(hTilt.y, transform.localEulerAngles.y, -hTilt.x);
    }
    
    private void Movement(MoveDirection direction)
    {
        var y = 0f;
        var x = 0f;
        
        if (heighForce.y > 0)
            y = - Time.fixedDeltaTime;
        else if (heighForce.y < 0)
            y = Time.fixedDeltaTime;
        
        if (heighForce.x > 0)
            x = -Time.fixedDeltaTime;
        else if (heighForce.x < 0)
            x = Time.fixedDeltaTime;
        
        switch (direction)
        {
            case MoveDirection.UP:
                engineForce += upForce;
                break;
            case MoveDirection.DOWN:
                engineForce = Mathf.Min(0, engineForce - downForce);
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
                
                var force = -(turnForcePercent - Mathf.Abs(heighForce.y)) * rigidbody.mass;
                rigidbody.AddRelativeTorque(0, force, 0);
                break;
            case MoveDirection.TURN_RIGHT:
                if (grounded)
                {
                    Message?.Invoke("You are on ground");
                    break;
                }
                        
                var yForce = (turnForcePercent - Mathf.Abs(heighForce.y)) * rigidbody.mass;
                rigidbody.AddRelativeTorque(0, yForce, 0);
                break;
        }
        
        heighForce.x = Mathf.Clamp(heighForce.x + x, -1, 1);
        heighForce.y = Mathf.Clamp(heighForce.y + y, -1, 1);
    }
    
    private void KeyUp(MoveDirection direction)
    {
        switch (direction)
        {
            case MoveDirection.UP:
                break;
            case MoveDirection.DOWN:
                break;
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
            case MoveDirection.UP:
                break;
            case MoveDirection.DOWN:
                break;
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
