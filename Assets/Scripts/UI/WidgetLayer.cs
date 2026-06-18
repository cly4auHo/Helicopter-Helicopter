using System.Collections;
using TMPro;
using UnityEngine;

public class WidgetLayer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI message;
    [SerializeField] private float timer;
    
    private Coroutine coroutine;
    
    public void Warning(in MoveDirection direction)
    {
        if (coroutine != null)
            StopCoroutine(coroutine);

        message.text = $"You can`t move {direction}";
        coroutine = StartCoroutine(Warning());
    }

    private IEnumerator Warning()
    {
        yield return new WaitForSeconds(timer);
        
        message.text = null;
    }
}
