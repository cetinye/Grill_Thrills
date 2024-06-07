using UnityEngine;

namespace Grill_Thrills
{
	public class LevelManager : MonoBehaviour
	{
		public static LevelManager instance;

		[Header("Camera")]
		public Camera mainCamera;

		[Header("Scene Objects")]
		[SerializeField] private BoxCollider grillCollider;

		void Awake()
		{
			if (instance == null)
			{
				instance = this;
			}
			else
			{
				Destroy(gameObject);
			}
		}

		public BoxCollider GetGrillCollider()
		{
			return grillCollider;
		}
	}
}
