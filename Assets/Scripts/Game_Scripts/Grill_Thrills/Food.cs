using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

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
		private int slotIndex;
		private bool isClickable = false;

		[Space()]
		[Header("Cook Slider")]
		[SerializeField] private Slider slider;
		[SerializeField] private Image sliderFillImg;
		[SerializeField] private Image ideallyCookedSliderFillImg;
		[SerializeField] private Color ideallyCookedColor;
		[SerializeField] private Color overCookedColor;
		private bool isColoringStarted = false;

		[Space()]
		[Header("Spatula")]
		[SerializeField] private GameObject spatula;
		[SerializeField] private Vector3 endSpatulaRotation;
		[SerializeField] private float spatulaRotTime;

		[Space()]
		[Header("Particles")]
		[SerializeField] private ParticleSystem smoke;
		[SerializeField] private ParticleSystem burn;

		[Space()]
		[Header("Feedbacks")]
		[SerializeField] private SpriteRenderer correctSpr;
		[SerializeField] private SpriteRenderer wrongSpr;

		[Space()]
		[Header("Materials")]
		[SerializeField] private MeshRenderer meshRenderer;
		[Space()]
		[SerializeField] private Material dissolveMaterial;
		[SerializeField] private float dissolveTime;
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

		[Space()]
		[Header("Tweens")]
		private Tween sliderColorTween, foodColorTween;

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
				isClickable = true;
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
			if (!isClickable) return;

			isClickable = false;

			if (isOnGrill && (1 - 0.25f - ideallyCookedSliderFillImg.fillAmount) <= slider.value && (1 - 0.25f) > slider.value)
			{
				Debug.LogWarning("IDEALLY COOKED " + gameObject.name);
				ShowFeedback(correctSpr, 0.5f);
				levelManager.Correct();
				SpawnSpatula();
				// Invoke(nameof(DisappearFood), 0.5f);
			}
			else
			{
				Wrong();
			}

			isOnGrill = false;
			sliderColorTween.Kill();
			foodColorTween.Kill();
		}

		private void Wrong()
		{
			Debug.LogWarning("!! NOT IDEALLY COOKED " + gameObject.name);
			ShowFeedback(wrongSpr, 0.5f);
			levelManager.Wrong();
			// Invoke(nameof(BurnFood), 0.5f);
			BurnFood();
		}

		private void SpawnSpatula()
		{
			GameObject spawnedSpatula = Instantiate(spatula, transform);
			spawnedSpatula.transform.localPosition = new Vector3(0f, -0.01289f, 0.00243f);
			spawnedSpatula.SetActive(true);

			Sequence spatulaSeq = DOTween.Sequence();
			spatulaSeq.Append(spawnedSpatula.transform.DOLocalMove(Vector3.zero, 0.5f));
			spatulaSeq.Append(spawnedSpatula.transform.DOLocalRotate(endSpatulaRotation, spatulaRotTime));
			spatulaSeq.OnComplete(() => DisappearFood());
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
				isClickable = false;
				isOnGrill = false;
				Wrong();
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
			StartCoroutine(DissolveRoutine());
			// burn.Play();
			// BurnFood(1f).OnComplete(() => DestroyFood());
		}

		private void DisappearFood()
		{
			DisableCollision();
			smoke.Play();
			DisappearFood(0.5f).OnComplete(() => DestroyFood());
		}

		private void DisableCollision()
		{
			boxCollider.enabled = false;
			rb.useGravity = false;
		}

		public void SetSlotIndex(int index)
		{
			slotIndex = index;
		}

		private Tween SliderColorTween(Color color, float time)
		{
			sliderColorTween = sliderFillImg.DOColor(color, time).SetEase(Ease.Linear);
			return sliderColorTween;
		}

		private Tween FoodMaterialColorTween(Material mat, Color color, float time)
		{
			foodColorTween = mat.DOColor(color, time).SetEase(Ease.Linear);
			return foodColorTween;
		}

		private Tween BurnFood(float time)
		{
			return transform.DOScale(0f, time).SetEase(Ease.InOutQuad);
		}

		private Tween DisappearFood(float time)
		{
			return transform.DOLocalMoveZ(transform.localPosition.z, time).SetEase(Ease.InOutQuad);
		}

		private void ShowFeedback(SpriteRenderer renderer, float time)
		{
			renderer.transform.DOScale(0.0033f, time);
			renderer.DOFade(1f, time).SetEase(Ease.InOutQuad);
		}

		private void DestroyFood()
		{
			levelManager.FreeSpawnPoint(slotIndex);
			Destroy(gameObject);
		}

		private IEnumerator DissolveRoutine()
		{
			Material[] newMaterials = new Material[meshRenderer.materials.Length];

			// Instantiate and assign dissolve materials
			for (int i = 0; i < meshRenderer.materials.Length; i++)
			{
				Material dissolve = Instantiate(dissolveMaterial) as Material;
				newMaterials[i] = dissolve;
			}

			meshRenderer.materials = newMaterials;

			// Apply dissolve effect
			for (int i = 0; i < meshRenderer.materials.Length; i++)
			{
				int index = i; // Capture the index for the closure
				meshRenderer.materials[i].DOFloat(0.65f, "_Dissolve_Amount", dissolveTime).OnComplete(() =>
				{
					if (index == meshRenderer.materials.Length - 1)
					{
						DestroyFood();
					}
				});
			}

			yield return new WaitForSeconds(1f);
		}
	}
}
