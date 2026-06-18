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

    public void UpdateInfo(in float speed, in float height)
    {
        infoLayer.UpdateInfo(speed, height);
    }
    
    public void Warning(string message)
    {
        widgetLayer.Warning(message);
    }
}
