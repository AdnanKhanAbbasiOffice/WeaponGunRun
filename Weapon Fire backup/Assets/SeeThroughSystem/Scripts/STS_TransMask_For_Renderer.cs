using UnityEngine;
using System.Collections;
using System;

[AddComponentMenu("Image Effects/See-Through System/Transparency masks for Renderer")]
public class STS_TransMask_For_Renderer : MonoBehaviour {
	
	
	public TextureInfo[] transparencyMasks;

	public Renderer rend;

	[System.Serializable]
	public class TextureInfo
	{
		[HideInInspector]
		[SerializeField]
		private STS_TransMask_For_Renderer pTransMaskComponent;

		[HideInInspector]
		[SerializeField]
		private Texture pTexture;

		[HideInInspector]
		[SerializeField]
		private bool pUseMainTextureTilingOffset = true;

		[HideInInspector]
		[SerializeField]
		private Vector2 pTiling = Vector2.one;

		[HideInInspector]
		[SerializeField]
		private Vector2 pOffset = Vector2.one;

		[HideInInspector]
		[SerializeField]
		private bool pAffectAllInstancesOfMaterial = true;

		public STS_TransMask_For_Renderer TransMaskComponent
		{
			get
			{
				return pTransMaskComponent;
			}
			set
			{
				pTransMaskComponent = value;
			}
		}

		public Texture texture
		{
			get
			{
				return pTexture;
			}
			set
			{
				pTexture = value;
				if (Application.isPlaying)
					TransMaskComponent.UpdateMaskInfo();
			}
		}

		public bool useMainTextureTilingOffset
		{
			get
			{
				return pUseMainTextureTilingOffset;
			}
			set
			{
				pUseMainTextureTilingOffset = value;
				if (Application.isPlaying)
					TransMaskComponent.UpdateMaskInfo();
			}
		}

		public Vector2 tiling
		{
			get
			{
				return pTiling;
			}
			set
			{
				pTiling = value;
				if (Application.isPlaying)
					TransMaskComponent.UpdateMaskInfo();
			}
		}


		public Vector2 offset
		{
			get
			{
				return pOffset;
			}
			set
			{
				pOffset = value;
				if (Application.isPlaying)
					TransMaskComponent.UpdateMaskInfo();
			}
		}

		public bool affectAllInstancesOfMaterial
		{
			get
			{
				return pAffectAllInstancesOfMaterial;
			}
			set
			{
				if (!Application.isPlaying)
					pAffectAllInstancesOfMaterial = value;
			}
		}
	}


	public void UpdateMaskInfo()
	{
		if (rend.sharedMaterials.Length != transparencyMasks.Length)
		{
			Debug.LogWarning(gameObject.name + " : Glow masks count doesn't fit materials count. If you change material count on this renderer in realtime, be sure to update glow masks accordingly.");
			Array.Resize(ref transparencyMasks, rend.sharedMaterials.Length);
		}
		for (int i = 0; i < rend.sharedMaterials.Length; i++)
		{
			if (rend.sharedMaterials != null)
			{
				if (transparencyMasks[i].affectAllInstancesOfMaterial)
				{
					rend.sharedMaterials[i].SetTexture("_STS_TransparencyMask", transparencyMasks[i].texture);
					if (transparencyMasks[i].useMainTextureTilingOffset)
					{
						rend.sharedMaterials[i].SetTextureScale("_STS_TransparencyMask", rend.sharedMaterials[i].mainTextureScale);
						rend.sharedMaterials[i].SetTextureOffset("_STS_TransparencyMask", rend.sharedMaterials[i].mainTextureOffset);
					}
					else
					{
						rend.sharedMaterials[i].SetTextureScale("_STS_TransparencyMask", transparencyMasks[i].tiling);
						rend.sharedMaterials[i].SetTextureOffset("_STS_TransparencyMask", transparencyMasks[i].offset);
					}
				}
				else
				{
					rend.materials[i].SetTexture("_STS_TransparencyMask", transparencyMasks[i].texture);
					if (transparencyMasks[i].useMainTextureTilingOffset)
					{
						rend.materials[i].SetTextureScale("_STS_TransparencyMask", rend.materials[i].mainTextureScale);
						rend.materials[i].SetTextureOffset("_STS_TransparencyMask", rend.materials[i].mainTextureOffset);
					}
					else
					{
						rend.materials[i].SetTextureScale("_STS_TransparencyMask", transparencyMasks[i].tiling);
						rend.materials[i].SetTextureOffset("_STS_TransparencyMask", transparencyMasks[i].offset);
					}
				}
			}
		}
	}

	// Use this for initialization
	protected void Start () 
	{
		rend = GetComponent<Renderer>();

		if (rend == null)
		{
			Debug.LogWarning(gameObject.name + " : STS_TransMask_For_Renderer component should be placed on object with Renderer component present. Disabling STS_TransMask_For_Renderer component.");
			this.enabled = false;
			return;
		}

		if (transparencyMasks == null)
		{
			transparencyMasks = new TextureInfo[rend.sharedMaterials.Length];
		}

		if (transparencyMasks.Length != rend.sharedMaterials.Length)
		{
			Array.Resize(ref transparencyMasks, rend.sharedMaterials.Length);
		}

		for (int i = 0; i < transparencyMasks.Length; i++)
		{
			if (transparencyMasks[i] == null)
				transparencyMasks[i] = new TextureInfo();
			transparencyMasks[i].TransMaskComponent = this;
		}
		UpdateMaskInfo();
	}


}
