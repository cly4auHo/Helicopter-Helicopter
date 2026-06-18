using UnityEngine;
using VContainer;

public class GameManager : MonoBehaviour
{
    [Inject] private InputSystem inputSystem;
    [Inject] private UIManager uiManager;
    
    [SerializeField] private Helicopter helicopterPrefab;

    private Helicopter helicopter;
    
    private void Start()
    {
        helicopter = Instantiate(helicopterPrefab);
    }
}
