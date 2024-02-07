using UnityEngine;
using System.Collections;
using System;

[AddComponentMenu("Image Effects/See-Through System/Transparency masks global manager")]
public class STS_TransMask_Global : MonoBehaviour {
	
	
	public TextureInfo[] TransMasks;

	[System.Serializable]
	public class TextureInfo
	{
		[HideInInspector]
		[SerializeField]
		private STS_TransMask_Global pTransMaskComponent;

		[HideInInspector]
		[SerializeField]
		private Material pMaterial;

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

		public STS_TransMask_Global TransMaskComponent
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

		public Material material
		{
			get
			{
				return pMaterial;
			}
			set
			{
				pMaterial = value;
				TransMaskComponent.UpdateMaskInfo();
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
	}


	public void UpdateMaskInfo()
	{
		for (int i = 0; i < TransMasks.Length; i++)
		{
			if (TransMasks[i].material != null)
			{
				TransMasks[i].material.SetTexture("_STS_TransparencyMask", TransMasks[i].texture);
				if (TransMasks[i].useMainTextureTilingOffset)
				{
					TransMasks[i].material.SetTextureScale("_STS_TransparencyMask", TransMasks[i].material.mainTextureScale);
					TransMasks[i].material.SetTextureOffset("_STS_TransparencyMask", TransMasks[i].material.mainTextureOffset);
				}
				else
				{
					TransMasks[i].material.SetTextureScale("_STS_TransparencyMask", TransMasks[i].tiling);
					TransMasks[i].material.SetTextureOffset("_STS_TransparencyMask", TransMasks[i].offset);
				}
			}
		}
	}

	// Use this for initialization
	protected void Start () 
	{
		if (TransMasks == null)
		{
			TransMasks = new TextureInfo[0];
		}

		for (int i = 0; i < TransMasks.Length; i++)
		{
			if (TransMasks[i] == null)
				TransMasks[i] = new TextureInfo();
			TransMasks[i].TransMaskComponent = this;
		}
		UpdateMaskInfo();
	}


}
