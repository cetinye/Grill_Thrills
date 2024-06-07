using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Grill_Thrills
{
	public class Food : MonoBehaviour, IFood
	{
		private LevelManager levelManager;

		[Header("Food Variables")]
		[SerializeField] private float cookTime;
		[SerializeField] private float cookTimeMultiplier;
		[SerializeField] private float idealCookTime;
		private float timeOnGrill;
		private bool isOnGrill = false;

		[Space()]
		[Header("Cook Slider")]
		[SerializeField] private Slider slider;
		[SerializeField] private Image sliderFillImg;
		[SerializeField] private Color ideallyCookedColor;
		[SerializeField] private Color overCookedColor;
		private bool isColoringStarted = false;

		[Space()]
		[Header("Materials")]
		[SerializeField] private Material meatMat;
		[SerializeField] private Material fatMat;
		[SerializeField] private Material boneMat;

		void Start()
		{
			levelManager = LevelManager.instance;
		}

		void OnCollisionEnter(Collision other)
		{
			if (other.collider.Equals(levelManager.GetGrillCollider()))
			{
				timeOnGrill = 0f;
				isOnGrill = true;
				ColorFood();
			}
		}

		void Update()
		{
			if (isOnGrill)
			{
				timeOnGrill += Time.deltaTime;
				UpdateSlider();
			}
		}

		public void Tapped()
		{
			Debug.Log("Tapped " + gameObject.name);
		}

		private void UpdateSlider()
		{
			slider.value = timeOnGrill / (cookTime / cookTimeMultiplier);
		}

		private void ColorFood()
		{
			if (!isColoringStarted)
			{
				isColoringStarted = true;
				SliderColorTween(ideallyCookedColor, idealCookTime).OnComplete(() => SliderColorTween(overCookedColor, cookTime - timeOnGrill));
			}
		}

		private Tween SliderColorTween(Color color, float time)
		{
			return sliderFillImg.DOColor(color, time).SetEase(Ease.Linear);
		}
	}
}
