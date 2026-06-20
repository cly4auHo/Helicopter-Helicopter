using UnityEngine;
using VContainer;

public class GameManager : MonoBehaviour
{
    [Inject] private InputSystem inputSystem;
    [Inject] private UIManager uiManager;
    
    [SerializeField] private Helicopter helicopterPrefab;
    [SerializeField] private CameraController cameraControllerPrefab;
    
    private Helicopter helicopter;
    private CameraController cameraController;
    
    private void Start()
    {
        helicopter = Instantiate(helicopterPrefab);
        helicopter.Init(inputSystem);
        cameraController = Instantiate(cameraControllerPrefab);
        cameraController.Init(helicopter.transform);
        
        helicopter.Message += OnHelicopterMessage;
        helicopter.IndicatorsUpdate += OnHelicopterUpdateIndicators;
        helicopter.Destroy += OnHelicopterDestroy;
    }
    
    private void OnHelicopterMessage(string message)
    {
        uiManager.Warning(message);
    }
    
    private void OnHelicopterUpdateIndicators(float acceleration, float height)
    {
        uiManager.UpdateInfo(acceleration, height);
    }
    
    private void OnHelicopterDestroy()
    {
        if (!helicopter)
            return;
        
        helicopter.Message -= OnHelicopterMessage;
        helicopter.IndicatorsUpdate -= OnHelicopterUpdateIndicators;
        helicopter.Destroy -= OnHelicopterDestroy;
    }
}
