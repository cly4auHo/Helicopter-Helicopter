using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Vector3 offsetPosition;
    [SerializeField] private Vector3 startRotation;

    private Transform target;
    
    public void Init(Transform aim)
    {
        target = aim;
        transform.localEulerAngles = startRotation;
    }

    private void LateUpdate()
    {
        transform.position = target.position + offsetPosition;
    }
}
