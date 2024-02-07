using UnityEngine;
using System.Collections;
using System;

[AddComponentMenu("Image Effects/See-Through System/See-Through System")]
public class SeeThroughSystem : MonoBehaviour {
	
	public LayerMask TriggerLayers;
	public LayerMask ObstacleLayers;
	public LayerMask BackgroundLayers;
	public int maskDownsample = 1;
	public int backgroundDownsample = 0;
	public Renderer[] disabledTriggers;
	public Renderer[] disabledObstacles;
	public bool colorizeTriggers;
	public float colorStrength;
	public Color coloredTriggersDefaultColor = Color.white;
	public ColoredObject[] coloredTriggers;
	public bool preserveDepthTexture;
	//public bool checkRenderTypes = true;
	public bool useTransparencyMasks = false;
	public float alphaDiscard = 0.01f;
	public float transparency = 1;
	public float sensitivity = 0;
	public float blurSpilling = 0;
	public bool hardBlur = true; 
	public int HBdownsample = 1;
	public float HBblurSize = 5;
	public int HBblurIterations = 1;
	public float blurSmoothing = 5;
	public bool softBlur = false;
	public int SBdownsample = 1;
	public float SBblurSize = 4;
	public int SBblurIterations = 1;
	public Color tintColor = Color.gray;
	public BackgroundRender backgroundRenderType = BackgroundRender.simple;
	public bool noShadows = false;
	public Light[] lightsToDisable;
	public float outline = 0.1f;
	public float hologramColorFactor = 0.5f;
	public Color effectColor = Color.blue;
	public Color backgroundColor = Color.black;
	public Shader replacementShader;
	public string shaderTags;
	public bool messageBeforeRender;
	public GameObject messageReciever;
	public string messageMethod = "STS_BeforeBackRender";
	public bool forceForwardRenderingOnBackground;
	public bool showObscuranceMask = false;
	public bool showColorMask = false;
	public Camera triggersCamera;
	public Camera obstaclesCamera;
	
	public Camera backgroundCamera;
	
	private Camera objectsCamera;
	private Camera mainCam;
	
	
	public enum BackgroundRender
	{
		simple = 0,
		outline = 1,
		hologram = 2,
		alpha_hologram = 3,
		custom_shader_replacement = 100
	}

	[System.Serializable]
	public class ColoredObject
	{
		public Color color;
		public Renderer[] triggers;
	}
	
	
	private int downsample;
	private float blurSize;
	private int blurIterations;
	
	private RenderTexture backgroundTexture;
	private RenderTexture objectsTexture;
	private RenderTexture obstaclesTexture;
	private RenderTexture obstaclesZBuffer;
	private RenderTexture originalZBuffer;

	private Shader whiteMaskShader;
	private Shader redMaskShader;
	private Shader redMaskShaderRT;
	private Shader redMaskShaderRTTM;
	private Shader depthMaskShaderRT;
	private Shader alphaMaskShader;
	private Shader alphaMaskShaderRT;
	private Shader alphaMaskShader_ortho;
	private Shader alphaMaskShaderRT_ortho;
	private Shader alphaMaskShaderRTTM;
	private Shader outlineShader;
	private Shader hologramShader;
	private Shader alphaHologramShader;
	
	private Material curMaterial;
	private Material curMaskMaterial;
	private Material curOrigMaterial;
	private Material curSatMaterial;
	private Material curCleanAlphaMaterial;
	private Material curBlurMaterial;
	private Material curMergeMasksMaterial;
	private Material[][] origRendMats;
	private Material[] rendMats;
	
	private Vector2 newResolution;
	private Vector2 oldResolution;
	
	private bool oldColorizeTriggers;
	
	const int CheckMaskRes = 8;
	
	

	Material satMaterial 
	{
		get 
		{
			if (curSatMaterial == null) 
			{
				curSatMaterial = new Material(Shader.Find("Hidden/STS_saturate_mask"));
				curSatMaterial.hideFlags = HideFlags.DontSave;
			}
			return curSatMaterial;
		} 
	} 
	
	Material mergeMasksMaterial 
	{
		get 
		{
			if (curMergeMasksMaterial == null) 
			{
				curMergeMasksMaterial = new Material(Shader.Find("Hidden/STS_MergeMasks"));
				curMergeMasksMaterial.hideFlags = HideFlags.DontSave;
			}
			return curMergeMasksMaterial;
		} 
	} 

	Material cleanAlphaMaterial
	{
		get 
		{
			if (curCleanAlphaMaterial == null) 
			{
				curCleanAlphaMaterial = new Material(Shader.Find("Hidden/STS_clean_alpha"));
				curCleanAlphaMaterial.hideFlags = HideFlags.DontSave;
			}
			return curCleanAlphaMaterial;
		} 
	} 
	
	Material blurMaterial 
	{
		get 
		{
			if (curBlurMaterial == null) 
			{
				curBlurMaterial = new Material(Shader.Find("Hidden/STS_FastBlur"));
				curBlurMaterial.hideFlags = HideFlags.DontSave;
			}
			return curBlurMaterial;
		} 
	} 
	
	Material material
	{
		get
		{
			if(curMaterial == null || oldColorizeTriggers != colorizeTriggers)
			{
				if(curMaterial != null)
				{
					DestroyImmediate(curMaterial);
				}
				if (colorizeTriggers)
					curMaterial = new Material(Shader.Find("Hidden/STS_compose"));
				else
					curMaterial = new Material(Shader.Find("Hidden/STS_compose_no_color"));
				curMaterial.hideFlags = HideFlags.HideAndDontSave;	
			}
			return curMaterial;
		}
	}
	
	Material maskMaterial
	{
		get
		{
			if(curMaskMaterial == null)
			{
				if(curMaskMaterial != null)
				{
					DestroyImmediate(curMaskMaterial);
				}
				curMaskMaterial = new Material(Shader.Find("Hidden/STS_trigger_mask_pass"));
				curMaskMaterial.hideFlags = HideFlags.HideAndDontSave;	
			}
			return curMaskMaterial;
		}
	}
	
	
	
	Material origMaterial
	{
		get
		{
			if(curOrigMaterial == null)
			{
				if(curOrigMaterial != null)
				{
					DestroyImmediate(curOrigMaterial);
				}
				curOrigMaterial = new Material(Shader.Find("Hidden/STS_save_orig_depth"));
				curOrigMaterial.hideFlags = HideFlags.HideAndDontSave;	
			}
			return curOrigMaterial;
		}
	}
	
	public void SetScissorRect( Camera cam, Rect r )
	{		
		if ( r.x < 0 )
		{
			r.width += r.x;
			r.x = 0;
		}
		
		if ( r.y < 0 )
		{
			r.height += r.y;
			r.y = 0;
		}
		
		r.width = Mathf.Min( 1 - r.x, r.width );
		r.height = Mathf.Min( 1 - r.y, r.height );			
		cam.rect = new Rect (0,0,1,1);
		cam.ResetProjectionMatrix ();
		Matrix4x4 m = cam.projectionMatrix;
		cam.rect = r;
		//Matrix4x4 m1 = Matrix4x4.TRS( new Vector3( r.x, r.y, 0 ), Quaternion.identity, new Vector3( r.width, r.height, 1 ) );
		Matrix4x4 m2 = Matrix4x4.TRS (new Vector3 ( ( 1/r.width - 1), ( 1/r.height - 1 ), 0), Quaternion.identity, new Vector3 (1/r.width, 1/r.height, 1));
		Matrix4x4 m3 = Matrix4x4.TRS( new Vector3( -r.x  * 2 / r.width, -r.y * 2 / r.height, 0 ), Quaternion.identity, Vector3.one );
		cam.projectionMatrix = m3 * m2 * m; 
	} 
	
	
	public void DisableTrigger(Renderer rend)
	{
		Array.Resize(ref disabledTriggers,disabledTriggers.Length+1);
		disabledTriggers[disabledTriggers.Length-1] = rend;
	}
	
	public void DisableObstacle(Renderer rend)
	{
		Array.Resize(ref disabledObstacles,disabledObstacles.Length+1);
		disabledTriggers[disabledObstacles.Length-1] = rend;
	}
	
	public void EnableTrigger(Renderer rend)
	{
		bool found = false;
		for (int i = 0; i < disabledTriggers.Length-1; i++)
		{
			if (disabledTriggers[i] == rend)
				found = true;
			if (found)
				disabledTriggers[i] = disabledTriggers[i+1];
		}
		Array.Resize(ref disabledTriggers,disabledTriggers.Length+1);
	}
	
	public void EnableObstacle(Renderer rend)
	{
		bool found = false;
		for (int i = 0; i < disabledObstacles.Length-1; i++)
		{
			if (disabledObstacles[i] == rend)
				found = true;
			if (found)
				disabledObstacles[i] = disabledObstacles[i+1];
		}
		Array.Resize(ref disabledObstacles,disabledObstacles.Length+1);
	}
	
	public void ColorizeTriggerObject(Renderer triggerRend, Color color, bool newArrayField = false)
	{
		DecolorizeTriggerObject(triggerRend);
		
		if (!newArrayField)
		{
			for (int i = 0; i < coloredTriggers.Length-1; i++)
			{
				if (coloredTriggers[i].color == color)
				{
					ColorizeTriggerObject(triggerRend,i);
					return;
				}
			}
		}
		
		Array.Resize(ref coloredTriggers, coloredTriggers.Length+1);
		coloredTriggers[coloredTriggers.Length-1] = new ColoredObject();
		coloredTriggers[coloredTriggers.Length-1].color = color;
		coloredTriggers[coloredTriggers.Length-1].triggers = new Renderer[1];
		coloredTriggers[coloredTriggers.Length-1].triggers[0] = triggerRend;
	}
	
	public void ColorizeTriggerObject(Renderer triggerRend, int coloredTriggersInd)
	{
		DecolorizeTriggerObject(triggerRend);
		Array.Resize(ref coloredTriggers[coloredTriggersInd].triggers, coloredTriggers[coloredTriggersInd].triggers.Length + 1);
		coloredTriggers[coloredTriggersInd].triggers[coloredTriggers[coloredTriggersInd].triggers.Length-1] = triggerRend;
	}
	
	public void ColorizeTriggerObject(GameObject trigger, Color color, bool newArrayField = false)
	{
		Component[] rends;
		rends = trigger.GetComponentsInChildren<Renderer>();
		if (!newArrayField)
		{
			for (int i = 0; i < coloredTriggers.Length-1; i++)
			{
				if (coloredTriggers[i].color == color)
				{
					foreach(Renderer rend in rends)
						ColorizeTriggerObject(rend,i);
					return;
				}
			}
		}
		Array.Resize(ref coloredTriggers, coloredTriggers.Length+1);
		coloredTriggers[coloredTriggers.Length-1] = new ColoredObject();
		coloredTriggers[coloredTriggers.Length-1].color = color;
		coloredTriggers[coloredTriggers.Length-1].triggers = new Renderer[rends.Length];
		for (int i = 0; i < rends.Length; i++)
		{
			DecolorizeTriggerObject((Renderer)rends[i]);
			coloredTriggers[coloredTriggers.Length-1].triggers[i] = (Renderer)rends[i];
		}
	}
	
	public void DecolorizeTriggerObject(Renderer triggerRend)
	{
		bool found = false;
		foreach(ColoredObject colObj in coloredTriggers)
		{
			for (int i = 0; i < colObj.triggers.Length; i++)
			{
				if (colObj.triggers[i] == triggerRend)
					found = true;
				if (found && i != colObj.triggers.Length-1)
					colObj.triggers[i] = colObj.triggers[i+1];
			}
			if (found)
			{
				Array.Resize(ref colObj.triggers, colObj.triggers.Length-1);
				return;
			}
		}
	}
	
	
	
	private void Blur (Material satMat, RenderTexture source, RenderTexture destination)
	{
		float widthMod = 1.0f / (1.0f * (1<<downsample));
		blurMaterial.SetVector("_Parameter", new Vector4(blurSize * widthMod, -blurSize * widthMod, 0.0f, 0.0f)); 
		source.filterMode = FilterMode.Bilinear;
		int rtW = source.width >> downsample;
		int rtH = source.height >> downsample;
		RenderTexture rt = RenderTexture.GetTemporary (rtW, rtH, 0, source.format);
		rt.filterMode = FilterMode.Bilinear; 
		Graphics.Blit (source, rt, blurMaterial, 0); 
		for(int i = 0; i < blurIterations; i++) 
		{
			float iterationOffs  = (i*1.0f);
			blurMaterial.SetVector ("_Parameter", new Vector4 (blurSize * widthMod + iterationOffs, -blurSize * widthMod - iterationOffs, 0.0f, 0.0f));
			
			// vertical blur
			RenderTexture rt2  = RenderTexture.GetTemporary (rtW, rtH, 0, source.format);
			rt2.filterMode = FilterMode.Bilinear;
			Graphics.Blit (rt, rt2, blurMaterial, 1);
			RenderTexture.ReleaseTemporary (rt);
			rt = rt2;
			
			// horizontal blur
			rt2 = RenderTexture.GetTemporary (rtW, rtH, 0, source.format);
			rt2.filterMode = FilterMode.Bilinear;
			Graphics.Blit (rt, rt2, blurMaterial, 2);
			RenderTexture.ReleaseTemporary (rt);
			rt = rt2;
		}
		Graphics.Blit (rt, destination,satMat);		
		RenderTexture.ReleaseTemporary (rt);
	}
	
	
	private void GetDepthTransMask(RenderTexture source, RenderTexture dest)
	{
		RenderTexture depthTransBuffer = RenderTexture.GetTemporary(source.width,source.height,0,source.format);
		//Graphics.Blit(source,depthTransBuffer,maskMaterial);
		Graphics.Blit(source,depthTransBuffer,cleanAlphaMaterial);
		
		RenderTexture saturatedMask = RenderTexture.GetTemporary(source.width,source.height,0,source.format);
		if (hardBlur)
		{
			if (softBlur)
				satMaterial.SetFloat("_power",256);
			else
				satMaterial.SetFloat("_power",blurSmoothing);
			blurIterations = HBblurIterations;
			blurSize = HBblurSize;
			downsample = HBdownsample;
			if (softBlur)
			{
				Blur (satMaterial, depthTransBuffer,saturatedMask);
			}
			else
			{
				Blur (satMaterial, depthTransBuffer,dest);
			}		
		}
		if (softBlur)
		{
			satMaterial.SetFloat("_power",1);
			blurIterations = SBblurIterations;
			blurSize = SBblurSize;
			downsample = SBdownsample;
			if (hardBlur)
			{
				Blur (satMaterial, saturatedMask,dest);
			}
			else
			{
				Blur (satMaterial, depthTransBuffer,dest);
			}
		}
		satMaterial.SetFloat("_transparency",transparency);
		if (!hardBlur && !softBlur)
			Graphics.Blit(depthTransBuffer,dest,satMaterial);
		RenderTexture.ReleaseTemporary(depthTransBuffer);
		RenderTexture.ReleaseTemporary(saturatedMask);
	}

	
	private Color InvertColor(Color col)
	{
		return new Color(1 - col.r,1 - col.g,1 - col.b);
	}


	
	void OnRenderImage(RenderTexture src, RenderTexture dest) 
	{
		if (transparency > 0)
		{
			Shader.SetGlobalFloat("_alphaDiscard",alphaDiscard);

			Shader.SetGlobalFloat("_STSObstDepthShift",sensitivity);
			
			newResolution = new Vector2((int)mainCam.pixelWidth,(int)mainCam.pixelHeight);


			if (oldResolution != newResolution)
			{
				if (originalZBuffer != null)
					originalZBuffer.Release();
				if (preserveDepthTexture)
				{
					originalZBuffer =  new RenderTexture((int)mainCam.pixelWidth, (int)mainCam.pixelHeight,0,RenderTextureFormat.Depth);
				}
			}
			
			//Saving original depth buffer;
			if (preserveDepthTexture)
			{
				if (originalZBuffer == null)
				{
					originalZBuffer =  new RenderTexture((int)mainCam.pixelWidth, (int)mainCam.pixelHeight,0,RenderTextureFormat.Depth);
				}
				else
				{
					originalZBuffer.DiscardContents();
				}
				Graphics.Blit(obstaclesTexture,originalZBuffer,origMaterial);
			}
			
			//Disabling renderers
			foreach (Renderer rend in disabledTriggers)
				if (rend != null)
					rend.enabled = false;
			foreach (Renderer rend in disabledObstacles)
				if (rend != null)
					rend.enabled = false;

			obstaclesTexture = RenderTexture.GetTemporary(src.width >> maskDownsample,src.height >> maskDownsample,16,src.format);


			//Rendering obstacle mask
			if (obstaclesCamera == null)
			{
				objectsCamera.CopyFrom(mainCam);
			}
			else
			{
				objectsCamera.transform.position = obstaclesCamera.transform.position;
				objectsCamera.transform.rotation = obstaclesCamera.transform.rotation;
				objectsCamera.CopyFrom(obstaclesCamera);
			}
			objectsCamera.renderingPath = RenderingPath.VertexLit;
			objectsCamera.targetTexture = obstaclesTexture;


			objectsCamera.backgroundColor = Color.black;
			objectsCamera.clearFlags = CameraClearFlags.Color;

			objectsCamera.depthTextureMode = DepthTextureMode.Depth;

			objectsCamera.cullingMask = ObstacleLayers.value;
			objectsCamera.backgroundColor = new Color(0,0,0,0);

			if (!useTransparencyMasks)
				objectsCamera.RenderWithShader(redMaskShaderRT,"RenderType");
			else
				objectsCamera.RenderWithShader(redMaskShaderRTTM,"RenderType");

			obstaclesZBuffer = RenderTexture.GetTemporary(src.width >> maskDownsample, src.height >> maskDownsample,16,RenderTextureFormat.Depth);
			objectsCamera.targetTexture = obstaclesZBuffer;

			objectsCamera.RenderWithShader(depthMaskShaderRT,"RenderType");
			Shader.SetGlobalTexture("_CameraDepthTexture",obstaclesZBuffer);


			objectsTexture = RenderTexture.GetTemporary(src.width >> maskDownsample,src.height >> maskDownsample,16,src.format);

			objectsCamera.clearFlags = CameraClearFlags.SolidColor;
			objectsCamera.targetTexture = objectsTexture;

			objectsCamera.renderingPath = RenderingPath.VertexLit;
			objectsCamera.depthTextureMode = DepthTextureMode.None;
			objectsCamera.backgroundColor = new Color(0,0,0,0);
			
			objectsCamera.cullingMask = TriggerLayers.value;

			//Setting colors if colorize triggers turned on
			if (colorizeTriggers && coloredTriggers.Length > 0)
			{
				Shader.SetGlobalColor("_STScolor",InvertColor(coloredTriggersDefaultColor));
				for (int i = 0; i < coloredTriggers.Length; i++)
				{
					Color col = InvertColor(coloredTriggers[i].color);
					float maxC = Mathf.Max(Mathf.Max(col.r,col.g), col.b);
					col = col / maxC;
					for (int x = 0; x < coloredTriggers[i].triggers.Length; x++)
					{
						foreach(Material mat in coloredTriggers[i].triggers[x].materials)
						{
							mat.SetColor("_STScolor",col * colorStrength);
						}
					}
				}
			}
			else
			{
				Shader.SetGlobalColor("_STScolor",Color.white);
			}

			if (objectsCamera.orthographic)
				objectsCamera.RenderWithShader(alphaMaskShaderRT_ortho,"RenderType");
			else
				objectsCamera.RenderWithShader(alphaMaskShaderRT,"RenderType");			

            
			//Show debug mask if toggled on
			if (showObscuranceMask)
			{
				Graphics.Blit(objectsTexture,dest);
				
				RenderTexture.ReleaseTemporary(objectsTexture);
				RenderTexture.ReleaseTemporary(obstaclesTexture);
				RenderTexture.ReleaseTemporary(obstaclesZBuffer);
				
				oldResolution = newResolution;
				oldColorizeTriggers = colorizeTriggers;
				return;
			}

                        
			RenderTexture obstacleTriggersMask = RenderTexture.GetTemporary(src.width >> maskDownsample,src.height >> maskDownsample,0,src.format);
			mergeMasksMaterial.SetTexture("_ObstMaskTex",obstaclesTexture);
			mergeMasksMaterial.SetTexture("_TrigMaskTex",objectsTexture);
			Graphics.Blit(objectsTexture,obstacleTriggersMask,mergeMasksMaterial);

			RenderTexture buffer2;
			buffer2 = RenderTexture.GetTemporary(src.width >> maskDownsample, src.height >> maskDownsample, 0);
			
			GetDepthTransMask(objectsTexture,buffer2);

			RenderTexture.ReleaseTemporary(objectsTexture);
			RenderTexture.ReleaseTemporary(obstaclesTexture);
			RenderTexture.ReleaseTemporary(obstaclesZBuffer);
            

			//Rendering background
			backgroundCamera.CopyFrom(mainCam);
			if (forceForwardRenderingOnBackground)
				backgroundCamera.renderingPath = RenderingPath.Forward;
			
			backgroundTexture = RenderTexture.GetTemporary(src.width >> backgroundDownsample,src.height >> backgroundDownsample, src.depth, src.format);
			backgroundCamera.targetTexture = backgroundTexture;
			
			if (triggersCamera != null)
				backgroundCamera.CopyFrom(triggersCamera);
			
			backgroundCamera.cullingMask = BackgroundLayers.value;
			
			
			if (messageBeforeRender && messageReciever != null)
				messageReciever.SendMessage(messageMethod,backgroundCamera);
			
			switch (backgroundRenderType)
			{
			case BackgroundRender.simple:
				float storedShadowDistance = QualitySettings.shadowDistance;
				if (noShadows)				
					QualitySettings.shadowDistance = 0;

				if (lightsToDisable != null && lightsToDisable.Length > 0)
				{					 
					bool[] savedLights = new bool[lightsToDisable.Length];
					for(int i = 0; i < lightsToDisable.Length; i++)
					{
						savedLights[i] = lightsToDisable[i].enabled;
						lightsToDisable[i].enabled = false;
					}

					backgroundCamera.Render();

					if (lightsToDisable != null && lightsToDisable.Length > 0)
					{
						for(int i = 0; i < lightsToDisable.Length; i++)
							lightsToDisable[i].enabled = savedLights[i];
					}
				}
				else
				{
					backgroundCamera.Render();
				}

				if (noShadows)
					QualitySettings.shadowDistance = storedShadowDistance;
				break;

			case BackgroundRender.outline:		
				Shader.SetGlobalFloat("_stw_outline",outline);
				Shader.SetGlobalColor("_sts_effect_color",effectColor);
				backgroundCamera.clearFlags = CameraClearFlags.Color;
				backgroundCamera.backgroundColor = backgroundColor;
				backgroundCamera.RenderWithShader(outlineShader,"");
				break;

			case BackgroundRender.hologram:		
				Shader.SetGlobalColor("_sts_effect_color",effectColor);
				Shader.SetGlobalFloat("_sts_color_factor",hologramColorFactor);
				backgroundCamera.clearFlags = CameraClearFlags.Color;
				backgroundCamera.backgroundColor = backgroundColor;	
				backgroundCamera.RenderWithShader(hologramShader,"");
				break;

			case BackgroundRender.alpha_hologram:		
				Shader.SetGlobalColor("_sts_effect_color",effectColor);
				Shader.SetGlobalFloat("_sts_color_factor",hologramColorFactor);
				backgroundCamera.clearFlags = CameraClearFlags.Color;
				backgroundCamera.backgroundColor = backgroundColor;	
				backgroundCamera.RenderWithShader(alphaHologramShader,"");
				break;

			case BackgroundRender.custom_shader_replacement:
				backgroundCamera.RenderWithShader(replacementShader,shaderTags);
				break;
			}

			if (colorizeTriggers)
				material.SetFloat("_ColorStrength",colorStrength);
			material.SetFloat("_BlurSpilling",blurSpilling);
			material.SetColor("_TintColor",tintColor);
			material.SetTexture("_MaskTex",buffer2);
			material.SetTexture("_ObstTrigMaskTex",obstacleTriggersMask);
			material.SetTexture("_BackTex",backgroundTexture);


			RenderTexture tempRT = RenderTexture.GetTemporary(src.width,src.height,0,src.format);

			Graphics.Blit(src,tempRT,material);

			Graphics.Blit(tempRT,dest);

			RenderTexture.ReleaseTemporary(tempRT);
			//Graphics.Blit(src,dest);

			//Clean up
			RenderTexture.ReleaseTemporary(backgroundTexture);
			RenderTexture.ReleaseTemporary(obstacleTriggersMask);
			
			RenderTexture.ReleaseTemporary(buffer2);

			//Restoring disabled renderers
			foreach (Renderer rend in disabledTriggers)
				if (rend != null)
					rend.enabled = true;
			foreach (Renderer rend in disabledObstacles)
				if (rend != null)
					rend.enabled = true;
			
			//Restoring original depth texture
			if (preserveDepthTexture)
			{
				Shader.SetGlobalTexture("_CameraDepthTexture",originalZBuffer);
			}
			


			oldResolution = newResolution;
			oldColorizeTriggers = colorizeTriggers;
		}
		else
		{
			Graphics.Blit(src,dest);
		}
	}
	
	
	
	// Use this for initialization
	void Start () 
	{
		if(!SystemInfo.supportsImageEffects)
		{
			enabled = false;
			return;
		}
		
		mainCam = GetComponent<Camera>();
		mainCam.depthTextureMode |= DepthTextureMode.Depth;
				
		//Shaders for mask rendering
		//redMaskShader = Shader.Find("Hidden/STS_red_mask");
		redMaskShaderRT = Shader.Find("Hidden/STS_red_mask_RT");
		redMaskShaderRTTM = Shader.Find("Hidden/STS_red_mask_RT_TM");
		depthMaskShaderRT = Shader.Find("Hidden/STS_depth_mask_RT");
		//alphaMaskShader = Shader.Find("Hidden/STS_alpha_mask");
		alphaMaskShaderRT = Shader.Find("Hidden/STS_alpha_mask_RT");
		//alphaMaskShader_ortho = Shader.Find("Hidden/STS_alpha_mask_ortho");
		alphaMaskShaderRT_ortho = Shader.Find("Hidden/STS_alpha_mask_RT_ortho");

		//Shaders for sfx background rendering
		outlineShader = Shader.Find("See-Through System/Outline");
		hologramShader = Shader.Find("See-Through System/Hologram");
		alphaHologramShader = Shader.Find("See-Through System/Alpha_Hologram");
		
		//for monitoring camera render area resizes
		oldResolution = new Vector2((int)mainCam.pixelWidth,(int)mainCam.pixelHeight);
		
		//Creating cameras;
		GameObject obj = new GameObject("STS_mask_camera");
		obj.transform.parent = transform;
		objectsCamera = obj.AddComponent<Camera>();
		objectsCamera.CopyFrom(mainCam);
		objectsCamera.cullingMask = TriggerLayers;
		objectsCamera.backgroundColor = new Color(0,0,0,0);
		objectsCamera.clearFlags = CameraClearFlags.Color;
		objectsCamera.depthTextureMode = DepthTextureMode.None;
		if (preserveDepthTexture)
		{
			originalZBuffer =  new RenderTexture((int)mainCam.pixelWidth, (int)mainCam.pixelHeight,0, RenderTextureFormat.Depth);
			originalZBuffer.format = RenderTextureFormat.RFloat;
		}
		objectsCamera.targetTexture = objectsTexture;
		objectsCamera.enabled = false;
		
		if (backgroundCamera == null)
		{
			obj = new GameObject("STS_background_camera");
			obj.transform.parent = transform;
			backgroundCamera = obj.AddComponent<Camera>();
			backgroundCamera.CopyFrom(mainCam);
			backgroundCamera.cullingMask = backgroundCamera.cullingMask & (~ObstacleLayers.value);
			backgroundCamera.depthTextureMode = DepthTextureMode.None;
			backgroundCamera.enabled = false;
		}
		
		backgroundCamera.targetTexture = backgroundTexture;
		satMaterial.SetFloat("_power",1);

		Shader.SetGlobalTexture("_STS_TransparencyMask", new Texture2D(2,2));
	}
	
	//When we disable or delete the effect.....
	void OnDisable ()
	{
		if(curMaterial != null)		
			DestroyImmediate(curMaterial);
		if(curOrigMaterial != null)		
			DestroyImmediate(curOrigMaterial);
		if(curSatMaterial != null)		
			DestroyImmediate(curSatMaterial);
		if(curMaskMaterial != null)		
			DestroyImmediate(curMaskMaterial);
		if(curBlurMaterial != null)		
			DestroyImmediate(curBlurMaterial);
	}
	
}
