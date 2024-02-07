using UnityEngine;
using System;
using System.Collections;
using UnityEditor;

[CustomEditor( typeof(STS_TransMask_For_Renderer))]
public class SIG_TransMaskEditor : Editor {


	private Renderer rend;

	public void OnEnable ()
	{
		STS_TransMask_For_Renderer gmask = (STS_TransMask_For_Renderer)target;
		rend = gmask.gameObject.GetComponent<Renderer>();
	}


	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		STS_TransMask_For_Renderer gmask = (STS_TransMask_For_Renderer)target;
		gmask.rend = rend;


		if (rend == null)
		{
			EditorGUILayout.HelpBox("Glow mask component should be placed on object with Renderer component present.",MessageType.Error);
			return;
		}

		if (gmask.transparencyMasks == null)
		{
			gmask.transparencyMasks = new STS_TransMask_For_Renderer.TextureInfo[rend.sharedMaterials.Length];
		}

		if (gmask.transparencyMasks.Length != rend.sharedMaterials.Length)
		{
			Array.Resize(ref gmask.transparencyMasks, rend.sharedMaterials.Length);
		}

		//foreach(SIG_TransMask.TextureInfo tinfo in gmask.transparencyMasks)
		for (int i = 0; i < gmask.transparencyMasks.Length; i++)
		{
			if (gmask.transparencyMasks[i] == null)
				gmask.transparencyMasks[i] = new STS_TransMask_For_Renderer.TextureInfo();
			gmask.transparencyMasks[i].TransMaskComponent = gmask;
		}


		for (int i = 0; i < gmask.transparencyMasks.Length; i++)
		{
			if (rend.sharedMaterials[i] != null)
			{
				EditorGUILayout.BeginVertical("box");
				EditorGUILayout.HelpBox("Material: " + rend.sharedMaterials[i].name,MessageType.None);		

				gmask.transparencyMasks[i].texture = (Texture)EditorGUILayout.ObjectField("Glow mask texture", gmask.transparencyMasks[i].texture, typeof(Texture), false);

				if (!(gmask.transparencyMasks[i].useMainTextureTilingOffset = EditorGUILayout.ToggleLeft("Use main texture tiling & offset settings",gmask.transparencyMasks[i].useMainTextureTilingOffset)))
				{
					gmask.transparencyMasks[i].tiling = EditorGUILayout.Vector2Field("Tiling",gmask.transparencyMasks[i].tiling);
					gmask.transparencyMasks[i].offset = EditorGUILayout.Vector2Field("Offset",gmask.transparencyMasks[i].offset);
				}
				if (Application.isPlaying)
				{
					EditorGUILayout.ToggleLeft("Affect all instances of material(Read-only in Play mode)",gmask.transparencyMasks[i].affectAllInstancesOfMaterial);
				}
				else
				{
					gmask.transparencyMasks[i].affectAllInstancesOfMaterial = EditorGUILayout.ToggleLeft("Affect all instances of material",gmask.transparencyMasks[i].affectAllInstancesOfMaterial);
				}
				EditorGUILayout.EndVertical();
			}
			else
			{
				if (!(rend is ParticleSystemRenderer))
				{
					EditorGUILayout.HelpBox("One of renderer materials is null",MessageType.Warning);
				}
			}
		}

	}
}
