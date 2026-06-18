using System.Collections;
using TMPro;
using UnityEngine;

public class WidgetLayer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI container;
    [SerializeField] private float timer;
    
    private Coroutine coroutine;
    
    public void Warning(string message)
    {
        if (coroutine != null)
            StopCoroutine(coroutine);

        container.text = message;
        coroutine = StartCoroutine(Warning());
    }

    private IEnumerator Warning()
    {
        yield return new WaitForSeconds(timer);
        
        container.text = null;
    }
}
