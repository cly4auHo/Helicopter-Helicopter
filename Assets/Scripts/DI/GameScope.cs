using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameScope : LifetimeScope
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private InputSystem inputSystem;
    [SerializeField] private UIManager uiManager;
    
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterInstance(gameManager);
        builder.RegisterInstance(inputSystem);
        builder.RegisterInstance(uiManager);
    }
}
