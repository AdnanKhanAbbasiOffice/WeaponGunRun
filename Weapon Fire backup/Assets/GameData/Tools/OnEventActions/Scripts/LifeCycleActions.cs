// Publisher: Vörös Gergely

using UnityEngine;
using UnityEngine.Events;

public class LifeCycleActions : MonoBehaviour {

    #region Public Variables
    [HideInInspector]
    public UnityEvent OnStartEvent;
    [HideInInspector]
    public UnityEvent OnAwakeEvent;
    [HideInInspector]
    public UnityEvent OnEnableEvent;
    [HideInInspector]
    public UnityEvent OnDisableEvent;
    #endregion

    #region Inspector Specific Variables
    [HideInInspector]
    public bool[] ShowLifeCycleEvents = new bool[4];
    #endregion

    void Start() {
        if (ShowLifeCycleEvents[0] == false) return;
        OnStartEvent.Invoke();
    }

    void Awake() {
        if (ShowLifeCycleEvents[1] == false) return;
        OnAwakeEvent.Invoke();
    }

    void OnEnable() {
        if (ShowLifeCycleEvents[2] == false) return;
        OnEnableEvent.Invoke();
    }

    void OnDisable() {
        if (ShowLifeCycleEvents[3] == false) return;
        OnDisableEvent.Invoke();
    }
}
