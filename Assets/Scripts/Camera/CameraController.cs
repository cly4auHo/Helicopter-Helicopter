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
        transform.position = target.position + target.rotation * offsetPosition;
        transform.rotation = Quaternion.Euler(new Vector3(startRotation.x, target.rotation.eulerAngles.y, startRotation.z));
    }
}
