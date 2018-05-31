using UnityEngine;

// SingletonBase
// GlobalBase
namespace Util
{
	public class SingletonBase<T> : MonoBehaviour where T : MonoBehaviour
	{
		static	private		T			instance	= null;

		static public T Instance
		{
			get
			{
				if (instance == null)
				{
					instance = GameObject.FindObjectOfType<T>();
					if (instance == null)
					{
						string typeString = typeof(T).ToString();

						Object obj = Resources.Load(typeString);
						if (obj != null)
						{
							GameObject gameObject = Instantiate(obj) as GameObject;
							gameObject.name = typeString;
							instance = gameObject.GetComponent<T>();
							if (instance == null)
							{
								Debug.LogWarning("WARNING : PREFAB NOT FOUND - " + typeString);
								instance = gameObject.AddComponent<T>();
							}
						}
						else
						{
							GameObject gameObject = new GameObject(typeString);
							instance = gameObject.AddComponent<T>();
						}
					}

					DontDestroyOnLoad(instance.gameObject);
				}

				return instance;
			}
		}

		static public bool Instantiated
		{
			get
			{
				if (instance == null)
					return false;

				return true;
			}
		}

		static public void DestroySelf()
		{
			if (instance == null)
				return;

			GameObject.Destroy(instance);
		}
	}


	public class GlobalBase<T> : MonoBehaviour where T : MonoBehaviour
	{
		static	private		T			instance	= null;

		static public T Instance
		{
			get
			{
				if (instance == null)
				{
					instance = GameObject.FindObjectOfType<T>();
					if (instance == null)
					{
						string typeString = typeof(T).ToString();
						GameObject gameObject = new GameObject(typeString);
						instance = gameObject.AddComponent<T>();
					}
				}

				return instance;
			}
		}
	}
}