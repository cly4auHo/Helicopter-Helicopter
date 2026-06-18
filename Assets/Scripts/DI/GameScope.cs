using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameScope : LifetimeScope
{
    [SerializeField] private InputSystem inputSystem;
    
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterInstance(inputSystem);
    }
}
