using UnityEngine;
using DG.Tweening;

namespace Grill_Thrills
{
	public class Ball : MonoBehaviour
	{
		[SerializeField] private float xRotation;
		[SerializeField] private float yRotation;
		[Space()]
		[SerializeField] private float yChangeTime;
		[SerializeField] private float yLowBound;
		[SerializeField] private float yUpBound;

		void Start()
		{
			MoveUpDown();
		}

		void Update()
		{
			transform.Rotate(xRotation * Time.deltaTime, yRotation * Time.deltaTime, 0f);
		}

		private void MoveUpDown()
		{
			transform.DOLocalMoveY(yLowBound, yChangeTime).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
		}
	}
}
