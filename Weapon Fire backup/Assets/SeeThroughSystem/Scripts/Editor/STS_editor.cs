using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor( typeof(SeeThroughSystem))]
public class STS_editor : Editor {
	
	private SerializedProperty triggerLayers;
	private SerializedProperty obstacleLayers;
	private SerializedProperty backgroundLayers;
	private SerializedProperty replacementShader;
	private SerializedProperty messageReciever;
	private SerializedProperty backgroundCamera;
	private SerializedProperty triggersCamera;
	private SerializedProperty obstaclesCamera;
	private SerializedProperty disabledTriggers;
	private SerializedProperty disabledObstacles;
	private SerializedProperty coloredTriggers;
	private SerializedProperty lightsToDisable;
	
	
	public void OnEnable () 
	{
		triggerLayers = serializedObject.FindProperty("TriggerLayers");
		obstacleLayers = serializedObject.FindProperty("ObstacleLayers");
		backgroundLayers = serializedObject.FindProperty("BackgroundLayers");
		replacementShader = serializedObject.FindProperty("replacementShader");
		messageReciever = serializedObject.FindProperty("messageReciever");
		backgroundCamera = serializedObject.FindProperty("backgroundCamera");
		obstaclesCamera = serializedObject.FindProperty("obstaclesCamera");
		triggersCamera = serializedObject.FindProperty("triggersCamera");
		disabledTriggers = serializedObject.FindProperty("disabledTriggers");
		disabledObstacles = serializedObject.FindProperty("disabledObstacles");
		coloredTriggers = serializedObject.FindProperty("coloredTriggers");
		lightsToDisable = serializedObject.FindProperty("lightsToDisable");

	}
	
	
	public override void OnInspectorGUI()
	{
		bool changed = false;
		serializedObject.Update();
		SeeThroughSystem sts = (SeeThroughSystem)target;
		
		//EditorGUILayout.BeginVertical("box");
		EditorGUILayout.HelpBox("Layers&Objects setup",MessageType.None);
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField(triggerLayers);
		EditorGUILayout.PropertyField(obstacleLayers);
		EditorGUILayout.PropertyField(backgroundLayers);		
		EditorGUILayout.PropertyField(disabledTriggers, true);
		EditorGUILayout.PropertyField(disabledObstacles, true);
		changed = changed | EditorGUI.EndChangeCheck();
		
		EditorGUILayout.Space();
		EditorGUILayout.HelpBox("Colorized triggers setup",MessageType.None);
		if (sts.colorizeTriggers = EditorGUILayout.ToggleLeft("Colorize triggers",sts.colorizeTriggers))
		{
			sts.colorStrength = EditorGUILayout.Slider("Color strength",sts.colorStrength,0,1);
			sts.coloredTriggersDefaultColor = EditorGUILayout.ColorField("Default trigger color",sts.coloredTriggersDefaultColor);
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(coloredTriggers, true);
			changed = changed | EditorGUI.EndChangeCheck();
		}
		
		
		//EditorGUILayout.EndVertical();
		EditorGUILayout.Space();
		//EditorGUILayout.BeginVertical("box");
		EditorGUILayout.HelpBox("Background setup",MessageType.None);
		sts.backgroundRenderType = (SeeThroughSystem.BackgroundRender)EditorGUILayout.EnumPopup("Background type",sts.backgroundRenderType);
		sts.backgroundDownsample = EditorGUILayout.IntSlider("Background downsample",sts.backgroundDownsample,0,4);
		switch (sts.backgroundRenderType)
		{
		case SeeThroughSystem.BackgroundRender.simple:
			sts.tintColor = EditorGUILayout.ColorField("Tint color",sts.tintColor);
			sts.noShadows = EditorGUILayout.ToggleLeft("No shadows",sts.noShadows);
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(lightsToDisable, true);
			changed = changed | EditorGUI.EndChangeCheck();
			break;
		case SeeThroughSystem.BackgroundRender.alpha_hologram:
			sts.tintColor = EditorGUILayout.ColorField("Tint color",sts.tintColor);
			sts.effectColor = EditorGUILayout.ColorField("Hologram color",sts.effectColor);
			sts.hologramColorFactor = EditorGUILayout.Slider("Hologram color factor",sts.hologramColorFactor,0,1f);
			sts.backgroundColor = EditorGUILayout.ColorField("Background color",sts.backgroundColor);
			break;
		case SeeThroughSystem.BackgroundRender.hologram:
			sts.tintColor = EditorGUILayout.ColorField("Tint color",sts.tintColor);
			sts.effectColor = EditorGUILayout.ColorField("Hologram color",sts.effectColor);
			sts.hologramColorFactor = EditorGUILayout.Slider("Hologram color factor",sts.hologramColorFactor,0,1f);
			sts.backgroundColor = EditorGUILayout.ColorField("Background color",sts.backgroundColor);
			break;
		case SeeThroughSystem.BackgroundRender.outline:
			sts.tintColor = EditorGUILayout.ColorField("Tint color",sts.tintColor);
			sts.effectColor = EditorGUILayout.ColorField("Outline color",sts.effectColor);
			sts.backgroundColor = EditorGUILayout.ColorField("Background color",sts.backgroundColor);
			sts.outline = EditorGUILayout.Slider("Outline thickness",sts.outline,0.05f,0.5f);
			break;
		case SeeThroughSystem.BackgroundRender.custom_shader_replacement:
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(replacementShader);
			changed = changed | EditorGUI.EndChangeCheck();
			sts.shaderTags = EditorGUILayout.TextField("Shader tags",sts.shaderTags);
			break;
		}

		EditorGUILayout.Space();

		sts.messageBeforeRender = EditorGUILayout.ToggleLeft("Message before background rendering",sts.messageBeforeRender);
		if (sts.messageBeforeRender)
		{
			EditorGUILayout.BeginVertical("box");
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(messageReciever);
			changed = changed | EditorGUI.EndChangeCheck();
			EditorGUILayout.EndVertical();
		}

		EditorGUILayout.Space();

		EditorGUILayout.HelpBox("Leave empty for auto-generation",MessageType.None);
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField(backgroundCamera);
		changed = changed | EditorGUI.EndChangeCheck();

		EditorGUILayout.Space();

		EditorGUILayout.HelpBox("Transparent area setup",MessageType.None);
		sts.transparency = EditorGUILayout.Slider("Transparency",sts.transparency,0,1);
		sts.sensitivity = EditorGUILayout.Slider("Sensitivity",sts.sensitivity,-0.5f,0.5f);
		sts.maskDownsample = EditorGUILayout.IntSlider("Mask downsample",sts.maskDownsample,0,4);
		sts.hardBlur = 	EditorGUILayout.ToggleLeft("Transparent area range",sts.hardBlur);
		if (sts.hardBlur)
		{
			EditorGUILayout.BeginVertical("box");
			//sts.HBblurSize = EditorGUILayout.Slider("Range size",sts.HBblurSize,0,15);
			sts.HBblurSize = EditorGUILayout.Slider("Range size",sts.HBblurSize,0,50);
			//sts.HBblurIterations = EditorGUILayout.IntSlider("Iterations",sts.HBblurIterations,1,5);
			sts.HBblurIterations = EditorGUILayout.IntSlider("Iterations",sts.HBblurIterations,1,10);
			//sts.HBdownsample = EditorGUILayout.IntSlider("Downsample",sts.HBdownsample,0,5);
			sts.HBdownsample = EditorGUILayout.IntSlider("Downsample",sts.HBdownsample,0,10);
			if (!sts.softBlur)
				sts.blurSmoothing = EditorGUILayout.Slider("Blur smoothness",sts.blurSmoothing,1,15);
			EditorGUILayout.EndVertical();
		}
		sts.softBlur = 	EditorGUILayout.ToggleLeft("Transparent area blur",sts.softBlur);
		if (sts.softBlur)
		{
			EditorGUILayout.BeginVertical("box");
			//sts.SBblurSize = EditorGUILayout.Slider("Blur size",sts.SBblurSize,0,15);
			sts.SBblurSize = EditorGUILayout.Slider("Blur size",sts.SBblurSize,0,50);
			//sts.SBblurIterations = EditorGUILayout.IntSlider("Iterations",sts.SBblurIterations,1,5);
			sts.SBblurIterations = EditorGUILayout.IntSlider("Iterations",sts.SBblurIterations,1,10);
			//sts.SBdownsample = EditorGUILayout.IntSlider("Downsample",sts.SBdownsample,0,5);
			sts.SBdownsample = EditorGUILayout.IntSlider("Downsample",sts.SBdownsample,0,10);
			EditorGUILayout.EndVertical();
		}
		if (sts.hardBlur || sts.softBlur)
			sts.blurSpilling = EditorGUILayout.Slider("Range&Blur spilling",sts.blurSpilling,0,1);

		EditorGUILayout.Space();

		EditorGUILayout.HelpBox("Advanced settings",MessageType.None);

		sts.forceForwardRenderingOnBackground = EditorGUILayout.ToggleLeft("Force Forward rendering on background",sts.forceForwardRenderingOnBackground);
		sts.alphaDiscard = EditorGUILayout.Slider("Alpha discard level",sts.alphaDiscard,0,1);
		sts.useTransparencyMasks = EditorGUILayout.ToggleLeft("Use transparency masks",sts.useTransparencyMasks);			
		sts.preserveDepthTexture = EditorGUILayout.ToggleLeft("Preserve depth texture",sts.preserveDepthTexture);
		sts.showObscuranceMask = EditorGUILayout.ToggleLeft("Show obscurance/color mask",sts.showObscuranceMask);
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField(triggersCamera);
		EditorGUILayout.PropertyField(obstaclesCamera);
		changed = changed | EditorGUI.EndChangeCheck();

		if (changed)
			serializedObject.ApplyModifiedProperties();

		if (GUI.changed)
			EditorUtility.SetDirty (target);
	}
}