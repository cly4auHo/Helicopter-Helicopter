using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private InfoLayer infoLayerPrefab;
    [SerializeField] private WidgetLayer widgetLayerPrefab;

    private InfoLayer infoLayer;
    private WidgetLayer widgetLayer;
    
    private void Awake()
    {
        infoLayer = Instantiate(infoLayerPrefab);
        widgetLayer = Instantiate(widgetLayerPrefab);
    }

    public void UpdateInfo(in float acceleration, in float height)
    {
        infoLayer.UpdateInfo(acceleration, height);
    }
    
    public void Warning(string message)
    {
        widgetLayer.Warning(message);
    }
}
