using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Grill_Thrills
{
	public class UIManager : MonoBehaviour
	{
		private LevelManager levelManager;

		[SerializeField] private TMP_Text levelTimerText;
		[SerializeField] private TMP_Text correctText;
		[SerializeField] private TMP_Text wrongText;

		[Header("Slider Variables")]
		[SerializeField] private RectTransform sliderRectTransform;
		[SerializeField] private Vector3 startPos;
		[SerializeField] private Vector3 endPos;
		[SerializeField] private float lerpFactor;
		[SerializeField] private float score;
		private Vector3 target;
		private float stepAmount;

		[Header("Flash Variables")]
		[SerializeField] private float flashInterval = 0.5f;
		private Color defaultColor;

		void Start()
		{
			levelManager = LevelManager.instance;
			defaultColor = levelTimerText.color;
			sliderRectTransform.localPosition = startPos;

			CalculateStepAmount();
		}

		void Update()
		{
			// set target slider position
			// target = LevelManager.instance.GetScore() * stepAmount;
			target = new Vector3(startPos.x + (score * stepAmount), sliderRectTransform.localPosition.y, sliderRectTransform.localPosition.z);

			// prevent slider from going out of bounds
			if (target.x > endPos.x)
				target = endPos;

			// lerp slider fill
			sliderRectTransform.localPosition = Vector3.Lerp(sliderRectTransform.localPosition, target, Time.deltaTime * lerpFactor);
		}

		public void UpdateTimer(float time)
		{
			levelTimerText.text = time.ToString("F0");
		}

		public void UpdateCorrectText(int correctCount)
		{
			correctText.text = "Correct: " + correctCount;
		}

		public void UpdateWrongText(int wrongCount)
		{
			wrongText.text = "Wrong: " + wrongCount;
		}

		private void CalculateStepAmount()
		{
			stepAmount = (endPos.x - startPos.x) / levelManager.GetLevelSO().minScoreToPass;
		}

		public void FlashRed()
		{
			Sequence redFlash = DOTween.Sequence();

			redFlash.Append(levelTimerText.DOColor(Color.red, flashInterval))
					.SetEase(Ease.Linear)
					.Append(levelTimerText.DOColor(defaultColor, flashInterval))
					.SetEase(Ease.Linear)
					.SetLoops(6);

			redFlash.Play();
		}
	}
}
