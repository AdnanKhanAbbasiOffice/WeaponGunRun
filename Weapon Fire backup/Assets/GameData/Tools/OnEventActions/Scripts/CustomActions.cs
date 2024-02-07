// Publisher: Vörös Gergely

using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CustomActions : MonoBehaviour
{
  //  [Tooltip("Delay Time in Seconds")]
    public float DelayTime;

    public UnityEvent OnCustomEvent;

    public void InvokeCustomEvent()
    {
        StartCoroutine(Invoke());
    }

    private IEnumerator Invoke()
    {
        yield return new WaitForSeconds(DelayTime);
        OnCustomEvent.Invoke();
    }
}
