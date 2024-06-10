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
		private Rigidbody rb;
		private BoxCollider boxCollider;

		[Space()]
		[Header("Cook Slider")]
		[SerializeField] private Slider slider;
		[SerializeField] private Image sliderFillImg;
		[SerializeField] private Image ideallyCookedSliderFillImg;
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

			rb = GetComponent<Rigidbody>();
			boxCollider = GetComponentInChildren<BoxCollider>();
		}

		void Start()
		{
			levelManager = LevelManager.instance;

			GetRandomIdealCookRange();
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

			if (isOnGrill && (1 - 0.25f - ideallyCookedSliderFillImg.fillAmount) <= slider.value && (1 - 0.25f) > slider.value)
			{
				Debug.LogWarning("IDEALLY COOKED " + gameObject.name);
				Invoke(nameof(DisappearFood), 0.5f);
			}
			else
			{
				Debug.LogWarning("!! NOT IDEALLY COOKED " + gameObject.name);
				Invoke(nameof(BurnFood), 0.5f);
			}

		}

		private void GetRandomIdealCookRange()
		{
			ideallyCookedSliderFillImg.fillAmount = Random.Range(0.05f, 0.25f);
		}

		private void UpdateSlider()
		{
			slider.value = timeOnGrill / (cookTime / cookTimeMultiplier);

			if (slider.value >= 1)
			{
				isOnGrill = false;
				Invoke(nameof(DisappearFood), 0.5f);
			}
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

		private void BurnFood()
		{
			DisableCollision();
			BurnFood(1f).OnComplete(() => Destroy(gameObject)); ;
		}

		private void DisappearFood()
		{
			DisableCollision();
			DisappearFood(1f).OnComplete(() => Destroy(gameObject));
		}

		private void DisableCollision()
		{
			boxCollider.enabled = false;
			rb.useGravity = false;
		}

		private Tween SliderColorTween(Color color, float time)
		{
			return sliderFillImg.DOColor(color, time).SetEase(Ease.Linear);
		}

		private Tween FoodMaterialColorTween(Material mat, Color color, float time)
		{
			return mat.DOColor(color, time).SetEase(Ease.Linear);
		}

		private Tween BurnFood(float time)
		{
			return transform.DOScale(0f, time).SetEase(Ease.InOutQuad);
		}

		private Tween DisappearFood(float time)
		{
			return transform.DOLocalMoveZ(0.05555f, time).SetEase(Ease.InOutQuad);
		}
	}
}
