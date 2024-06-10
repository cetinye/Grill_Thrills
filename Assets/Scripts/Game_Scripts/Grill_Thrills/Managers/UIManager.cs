using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Grill_Thrills
{
	public class UIManager : MonoBehaviour
	{
		private LevelManager levelManager;

		[SerializeField] private TMP_Text correctText;
		[SerializeField] private TMP_Text wrongText;

		void Start()
		{
			levelManager = LevelManager.instance;
		}

		public void UpdateCorrectText(int correctCount)
		{
			correctText.text = "Correct: " + correctCount;
		}

		public void UpdateWrongText(int wrongCount)
		{
			wrongText.text = "Wrong: " + wrongCount;
		}
	}
}
