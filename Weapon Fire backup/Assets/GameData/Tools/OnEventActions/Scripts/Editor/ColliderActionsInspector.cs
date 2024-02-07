// Publisher: Vörös Gergely

using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(ColliderActions))]
public class ColliderActionsInspector : Editor
{

    private string[] CallBacks = { "On Collider Enter", "On Collider Exit", "On Collider Stay" };
    private int CallBackIndex = 0;

    
    private ColliderActions _actions;
    public ColliderActions actions
    {
        get
        {
            return _actions = _actions ?? (target as ColliderActions);
        }
    }

    private int Index => actions.TagIndex;

    private string[] Tags => InternalEditorUtility.tags;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(10);
        GUIBody();
        GUILayout.Space(10);
        
    }

    public void GUIBody()
    {
        // Select your Objects tag. Only object with this tah will Invoke Actions.
        GUILayout.BeginHorizontal();
            GUILayout.Label("Select Player Tag", GUILayout.Width(180));

            EditorGUI.BeginChangeCheck();
            actions.TagIndex = EditorGUILayout.Popup(Index, Tags);
            if (EditorGUI.EndChangeCheck())
                actions.ObservedTag = Tags[Index];
        GUILayout.EndHorizontal();

        // Select your Callback to add.
        GUILayout.BeginHorizontal();
            GUILayout.Label("Select Your Collider Callbacks ", GUILayout.Width(180));
            CallBackIndex = EditorGUILayout.Popup(CallBackIndex, CallBacks);
            if (GUILayout.Button("Add", GUILayout.Height(14)))
            {
                actions.ShowCallBackEvents[CallBackIndex] = true;
            }
        GUILayout.EndHorizontal();


        int length = actions.ShowCallBackEvents.Length;
        for (int i = 0; i < length; i++)
        {
            if (!actions.ShowCallBackEvents[i]) continue;
            GUILayout.Space(10);
            GUILayout.BeginVertical(GUI.skin.box);
                GUILayout.BeginHorizontal();
                    GUI.backgroundColor = Color.green;
                    if (GUILayout.Button("Hide"))
                    {
                        actions.ShowCallBackEvents[i] = false;
                    }
                    GUI.backgroundColor = Color.white;
                GUILayout.EndHorizontal();
            
                switch (i)
                {
                    case 0:
                        ShowProperty("OnTriggerEnterEvent");
                        break;
                    case 1:
                        ShowProperty("OnTriggerExitEvent");
                        break;
                    case 2:
                        ShowProperty("OnTriggerStayEvent");
                        break;
                }
            GUILayout.EndVertical();
        }

    }

    private void ShowProperty(string name)
    {
        SerializedProperty prop = serializedObject.FindProperty(name);
        EditorGUILayout.PropertyField(prop);
        serializedObject.ApplyModifiedProperties();
    }



}
