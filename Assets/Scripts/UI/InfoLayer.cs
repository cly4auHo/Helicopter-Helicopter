using TMPro;
using UnityEngine;

public class InfoLayer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI speedIndicator;
    [SerializeField] private TextMeshProUGUI heightIndicator;
    
    public void UpdateInfo(in float speed, in float height)
    {
        speedIndicator.text = $"{speed:F2}";
        heightIndicator.text = $"{height:F2}";
    }
}
