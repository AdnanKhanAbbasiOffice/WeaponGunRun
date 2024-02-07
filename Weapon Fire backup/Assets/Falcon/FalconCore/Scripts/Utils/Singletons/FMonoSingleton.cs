using UnityEngine;

namespace Falcon.FalconCore.Scripts.Utils.Singletons
{
	public abstract class FMonoSingleton<T> : MonoBehaviour where T : FMonoSingleton<T>, new()
	{
		private static T _instance;

		public static T Instance
		{
			get
			{
				if( _instance == null )
				{
					_instance = FalconMain.Instance.AddIfNotExist<T>();
				}
				return _instance;
			}

			protected set { _instance = value; }
		}

		protected virtual void Awake()
		{
			DontDestroyOnLoad( gameObject );
			if( _instance != this && _instance != null )
			{
				Destroy( this );
			}
		}
	}
}