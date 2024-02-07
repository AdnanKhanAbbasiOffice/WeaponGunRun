// Publisher: Vörös Gergely
using UnityEngine;
using UnityEngine.Events;

public class ColliderActions : MonoBehaviour {

    #region Public Variables
    [HideInInspector]
    public string ObservedTag;
    [HideInInspector]
    public UnityEvent OnTriggerEnterEvent;
    [HideInInspector]
    public UnityEvent OnTriggerExitEvent;
    [HideInInspector]
    public UnityEvent OnTriggerStayEvent;
    #endregion

    #region Private Variables
    private Collider Player;
    private Collider2D Player2D;
    #endregion

    #region Inspector Specific Variables
    [HideInInspector]
    public bool[] ShowCallBackEvents = new bool[3];
    [HideInInspector]
    public int TagIndex = 0;
    #endregion

    #region Collider Callbacks
    void OnTriggerEnter(Collider other)
    {
        if (ShowCallBackEvents[0] == false) return;
        if (ObservedTag.Equals(other.tag))
        {
            Player = other;
            OnTriggerEnterEvent.Invoke();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (ShowCallBackEvents[1] == false) return;
        if (ObservedTag.Equals(other.tag)) OnTriggerExitEvent.Invoke();
    }

    void OnTriggerStay(Collider other)
    {
        if (ShowCallBackEvents[2] == false) return;
        if (ObservedTag.Equals(other.tag))
        {
            Player = other;
            OnTriggerStayEvent.Invoke();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (ShowCallBackEvents[0] == false) return;
        if (ObservedTag.Equals(other.tag))
        {
            Player2D = other;
            OnTriggerEnterEvent.Invoke();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (ShowCallBackEvents[1] == false) return;
        if (ObservedTag.Equals(other.tag)) OnTriggerExitEvent.Invoke();
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (ShowCallBackEvents[2] == false) return;
        if (ObservedTag.Equals(other.tag))
        {
            Player2D = other;
            OnTriggerStayEvent.Invoke();
        }
    }
    #endregion

    // If I want to destroy everything with ObeservedTag
    public void DestroyPlayer(float time)
    {
        Destroy(Player.gameObject, time);
    }

    public void Destroy2DPlayer(float time)
    {
        Destroy(Player2D.gameObject, time);
    }
}
