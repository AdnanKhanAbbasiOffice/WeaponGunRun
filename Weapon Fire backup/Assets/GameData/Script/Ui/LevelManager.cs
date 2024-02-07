using UnityEngine;
[CreateAssetMenu(fileName = "New Level", menuName = "Levels Info")]
public class LevelManager : ScriptableObject
{
    [SerializeField] internal GameObject levelPreb;
    [SerializeField] internal int totalTarget = 0;
    [SerializeField] internal bool IsTutorialLevel = false;
    [SerializeField] internal bool IsLevelBeforeTutorial = false;



}




