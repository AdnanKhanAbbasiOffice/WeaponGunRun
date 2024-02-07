using UnityEngine;
using System;
using System.Collections;
using UnityEditor;

[CustomEditor( typeof(STS_TransMask_Global))]
public class STS_TransMaskGlobalEditor : Editor {


	private Renderer rend;


	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		STS_TransMask_Global gmask = (STS_TransMask_Global)target;

		if (gmask.TransMasks == null)
		{
			gmask.TransMasks = new STS_TransMask_Global.TextureInfo[0];
		}


		for (int i = 0; i < gmask.TransMasks.Length; i++)
		{
			EditorGUILayout.BeginVertical("box");

			EditorGUILayout.HelpBox("Array index: " + i.ToString(),MessageType.None);

			gmask.TransMasks[i].material = (Material)EditorGUILayout.ObjectField("Material", gmask.TransMasks[i].material, typeof(Material), false);

			gmask.TransMasks[i].texture = (Texture)EditorGUILayout.ObjectField("Glow mask texture", gmask.TransMasks[i].texture, typeof(Texture), false);

			if (!(gmask.TransMasks[i].useMainTextureTilingOffset = EditorGUILayout.ToggleLeft("Use main texture tiling & offset settings",gmask.TransMasks[i].useMainTextureTilingOffset)))
			{
				gmask.TransMasks[i].tiling = EditorGUILayout.Vector2Field("Tiling",gmask.TransMasks[i].tiling);
				gmask.TransMasks[i].offset = EditorGUILayout.Vector2Field("Offset",gmask.TransMasks[i].offset);
			}
			if(GUILayout.Button("Delete material slot"))
			{
				if (gmask.TransMasks[i].material == null || EditorUtility.DisplayDialog("Really delete material slot?",
														                                 "All glow parameters for material '"+ gmask.TransMasks[i].material.name +"' will be lost. Proceed?",
														                                 "Delete", "Cancel"))
				{
					for (int x = i+1; x < gmask.TransMasks.Length; x++)
						gmask.TransMasks[x-1] = gmask.TransMasks[x];			
					Array.Resize(ref gmask.TransMasks,gmask.TransMasks.Length - 1);
				}
			}

			EditorGUILayout.EndVertical();
		}

		if(GUILayout.Button("Add material slot"))
		{
			Array.Resize(ref gmask.TransMasks,gmask.TransMasks.Length + 1);
			gmask.TransMasks[gmask.TransMasks.Length-1] = new STS_TransMask_Global.TextureInfo();
			gmask.TransMasks[gmask.TransMasks.Length-1].TransMaskComponent = gmask;
		}

	}
}
