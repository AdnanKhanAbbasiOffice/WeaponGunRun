using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "FalconPopupForceUpdateLanguage", menuName = "ScriptableObjects/FalconPopupForceUpdateLanguage", order = 1)]
public class FalconPopupForceUpdateLanguage : ScriptableObject
{
    public List<LocalizeInfo> localizeInfos;
}
[Serializable]
public class LocalizeInfo
{
    public string Language;
    public string Cancel;
    public string Update;
    public string Title;
}