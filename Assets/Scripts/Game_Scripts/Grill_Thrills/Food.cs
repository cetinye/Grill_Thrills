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
		[SerializeField] private MeshRenderer meshRenderer;
		[Space()]
		[SerializeField] private Material meatMat;
		[SerializeField] private Material fatMat;
		[SerializeField] private Material boneMat;
		[Space()]
		[SerializeField] private int meatMatIndex;
		[SerializeField] private int fatMatIndex;
		[SerializeField] private int boneMatIndex;

		[Space()]
		[Header("Material Colors")]
		[SerializeField] private Color m_cookedColor;
		[SerializeField] private Color m_overcookedColor;
		[Space()]
		[SerializeField] private Color f_cookedColor;
		[SerializeField] private Color f_overcookedColor;
		[Space()]
		[SerializeField] private Color b_cookedColor;
		[SerializeField] private Color b_overcookedColor;

		void Awake()
		{
			if (meatMat != null)
			{
				meatMat = Instantiate(meatMat);
				meshRenderer.materials[meatMatIndex] = meatMat;
			}

			if (fatMat != null)
			{
				fatMat = Instantiate(fatMat);
				meshRenderer.materials[fatMatIndex] = fatMat;
			}

			if (boneMat != null)
			{
				boneMat = Instantiate(boneMat);
				meshRenderer.materials[boneMatIndex] = boneMat;
			}
		}

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
				ColorFoodSlider();
				ColorFoodMaterials();
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

		private void ColorFoodSlider()
		{
			if (!isColoringStarted)
			{
				isColoringStarted = true;
				SliderColorTween(ideallyCookedColor, idealCookTime).OnComplete(() => SliderColorTween(overCookedColor, cookTime - timeOnGrill));
			}
		}

		private void ColorFoodMaterials()
		{
			if (meatMat != null)
				FoodMaterialColorTween(meshRenderer.materials[meatMatIndex], m_cookedColor, idealCookTime).OnComplete(() => FoodMaterialColorTween(meshRenderer.materials[meatMatIndex], m_overcookedColor, cookTime - timeOnGrill)); ;

			if (fatMat != null)
				FoodMaterialColorTween(meshRenderer.materials[fatMatIndex], f_cookedColor, idealCookTime).OnComplete(() => FoodMaterialColorTween(meshRenderer.materials[fatMatIndex], f_overcookedColor, cookTime - timeOnGrill)); ;

			if (boneMat != null)
				FoodMaterialColorTween(meshRenderer.materials[boneMatIndex], b_cookedColor, idealCookTime).OnComplete(() => FoodMaterialColorTween(meshRenderer.materials[boneMatIndex], b_overcookedColor, cookTime - timeOnGrill)); ;
		}

		private Tween SliderColorTween(Color color, float time)
		{
			return sliderFillImg.DOColor(color, time).SetEase(Ease.Linear);
		}

		private Tween FoodMaterialColorTween(Material mat, Color color, float time)
		{
			return mat.DOColor(color, time).SetEase(Ease.Linear);
		}
	}
}