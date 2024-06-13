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

		[Header("Flash Variables")]
		[SerializeField] private float flashInterval = 0.5f;
		private Color defaultColor;

		void Start()
		{
			levelManager = LevelManager.instance;
			defaultColor = levelTimerText.color;
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
