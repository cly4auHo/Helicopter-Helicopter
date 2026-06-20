using TMPro;
using UnityEngine;

public class InfoLayer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI accelerationIndicator;
    [SerializeField] private TextMeshProUGUI heightIndicator;
    
    public void UpdateInfo(in float acceleration, in float height)
    {
        accelerationIndicator.text = $"{acceleration:F2}";
        heightIndicator.text = $"{height:F2}";
    }
}
