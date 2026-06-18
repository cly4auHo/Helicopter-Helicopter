using UnityEngine;

public class Helicopter : MonoBehaviour
{
    [SerializeField] private Rigidbody rigidbody;

    private bool grounded;
    
    private void FixedUpdate()
    {
        
    }
    
    
    
    private void OnCollisionEnter()
    {
        grounded = true;
    }

    private void OnCollisionExit()
    {
        grounded = false;
    }
}
