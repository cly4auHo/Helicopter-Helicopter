using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameScope : LifetimeScope
{
    [SerializeField] private InputSystem inputSystem;
    [SerializeField] private UIManager uiManager;
    
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterInstance(inputSystem);
        builder.RegisterInstance(uiManager);
    }
}
