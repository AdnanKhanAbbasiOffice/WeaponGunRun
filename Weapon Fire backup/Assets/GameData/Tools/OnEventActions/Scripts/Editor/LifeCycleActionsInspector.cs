// Publisher: Vörös Gergely

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LifeCycleActions))]
public class LifeCycleActionsInspector : Editor
{

    private string[] Events = { "On Start Event", "On Awake Event", "On Enable Event", "On Disable Event" };
    private int EventIndex = 0;

    private LifeCycleActions _actions;
    public LifeCycleActions actions
    {
        get
        {
            return _actions = _actions ?? (target as LifeCycleActions);
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(10);
        GUIBody();
        GUILayout.Space(10);
    }

    public void GUIBody()
    {
        // Select your Callback to add.
        GUILayout.BeginHorizontal();
        GUILayout.Label("Select LifeCycle Events ", GUILayout.Width(180));
        EventIndex = EditorGUILayout.Popup(EventIndex, Events);
        if (GUILayout.Button("Add", GUILayout.Height(14)))
        {
            actions.ShowLifeCycleEvents[EventIndex] = true;
        }
        GUILayout.EndHorizontal();


        int length = actions.ShowLifeCycleEvents.Length;
        for (int i = 0; i < length; i++)
        {
            if (!actions.ShowLifeCycleEvents[i]) continue;
            GUILayout.Space(10);
            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.BeginHorizontal();
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("Hide"))
            {
                actions.ShowLifeCycleEvents[i] = false;
            }
            GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();

            switch (i)
            {
                case 0:
                    ShowProperty("OnStartEvent");
                    break;
                case 1:
                    ShowProperty("OnAwakeEvent");
                    break;
                case 2:
                    ShowProperty("OnEnableEvent");
                    break;
                case 3:
                    ShowProperty("OnDisableEvent");
                    break;
            }
            GUILayout.EndVertical();
        }

    }

    private void ShowProperty(string name)
    {
        SerializedObject ser = new SerializedObject(actions);
        SerializedProperty prop = ser.FindProperty(name);
        EditorGUILayout.PropertyField(prop);
        ser.ApplyModifiedProperties();
    }



}

