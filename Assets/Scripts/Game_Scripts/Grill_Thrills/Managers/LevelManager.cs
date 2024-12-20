using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Grill_Thrills
{
	public class LevelManager : MonoBehaviour
	{
		public static LevelManager instance;
		[SerializeField] private UIManager uiManager;

		[Header("Level Variables")]
		[SerializeField] private int levelId;
		[SerializeField] private LevelSO levelSO;
		[SerializeField] private List<LevelSO> levels = new List<LevelSO>();
		private float score;

		[Header("Camera")]
		public Camera mainCamera;
		[SerializeField] private Vector3 startPos;
		[SerializeField] private Vector3 startRot;
		[SerializeField] private Vector3 endPos;
		[SerializeField] private Vector3 endRot;

		[Header("Scene Objects")]
		[SerializeField] private BoxCollider grillCollider;
		[SerializeField] private Transform spawnParent;
		[SerializeField] private Material ditherTransparencyMatGray;
		[SerializeField] private Material ditherTransparencyMatWhite;
		[SerializeField] private List<Transform> spawnPoints = new List<Transform>();
		[SerializeField] private List<Food> foodPrefabs = new List<Food>();
		private List<int> usedSpawnPointIndexes = new List<int>();
		private List<Food> fastFoods = new List<Food>();
		private List<Food> mediumFoods = new List<Food>();
		private List<Food> slowFoods = new List<Food>();
		[SerializeField] private List<Food> foodsToSpawn = new List<Food>();
		private int ideallyCookedCount = 0, correctCount = 0, wrongCount = 0;
		public int totalIdeallyCookedCount = 0, totalCorrectCount = 0, totalWrongCount = 0;
		private bool lockWrong1 = true, lockWrong2 = true;

		[Header("Timer Variables")]
		[SerializeField] private float levelTime;
		private bool isLevelTimerOn = false;
		private float levelTimer;

		[Header("Flash Interval")]
		[SerializeField] private bool isFlashable = true;

		public Slider levelSlider;

		public void LevelSlider()
		{
			PlayerPrefs.SetInt("Grill_Thrills_LevelID", Mathf.CeilToInt(levelSlider.value));
		}

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

			mainCamera.transform.SetPositionAndRotation(startPos, Quaternion.Euler(startRot));

			GameStateManager.OnGameStateChanged += OnGameStateChanged;

			AssignLevelVariables();
		}

		void OnEnable()
		{
			ditherTransparencyMatGray.SetFloat("_Opacity", 1f);
			ditherTransparencyMatWhite.SetFloat("_Opacity", 1f);
		}

		/// <summary>
		/// This function is called when the MonoBehaviour will be destroyed.
		/// </summary>
		void OnDestroy()
		{
			GameStateManager.OnGameStateChanged -= OnGameStateChanged;
		}

		void Start()
		{
			StartGame();
		}

		void Update()
		{
			LevelTimer();
		}

		private void AssignLevelVariables()
		{
			levelId = PlayerPrefs.GetInt("Grill_Thrills_LevelID", 1);
			levelId = Mathf.Clamp(levelId, 1, levels.Count);

			levelSO = levels[levelId - 1];
		}

		private void StartGame()
		{
			AudioManager.instance.Play(SoundType.Background);
			AudioManager.instance.Play(SoundType.BackgroundGrill);

			CategorizeFoods();
			PrepareFoodsToSpawn();

			GameStateManager.SetGameState(GameState.Idle);
		}

		private void OnGameStateChanged()
		{
			switch (GameStateManager.GetGameState())
			{
				case GameState.Idle:
					StartCoroutine(MoveCameraRoutine());
					break;

				case GameState.Playing:
					levelTimer = levelTime;
					isLevelTimerOn = true;
					InvokeRepeating(nameof(SpawnFood), 1f, levelSO.spawnFrequency);
					break;

				case GameState.TimesUp:
					CancelInvoke(nameof(SpawnFood));
					isLevelTimerOn = false;
					levelTimer = 0f;
					uiManager.UpdateTimer(levelTimer);
					break;

				default:
					break;
			}
		}

		private void LevelTimer()
		{
			if (!isLevelTimerOn) return;

			levelTimer -= Time.deltaTime;
			uiManager.UpdateTimer(levelTimer);

			if (levelTimer <= 0f)
			{
				GameStateManager.SetGameState(GameState.TimesUp);
				Debug.Log("Score: " + CalculateScore());

				// UnityEngine.SceneManagement.SceneManager.LoadScene(0);
			}

			if (levelTimer <= 5.2f && isFlashable)
			{
				isFlashable = false;
				// GameManager.instance.PlayFx("Countdown", 0.7f, 1f);
				uiManager.FlashRed();
			}
		}

		public int CalculateScore()
		{
			float inGameScore = (totalIdeallyCookedCount * levelSO.idealCookScore) + (totalCorrectCount * levelSO.rawOvercookScore) - (totalWrongCount * levelSO.penaltyPoint);
			float maxInGame = (totalCorrectCount + totalIdeallyCookedCount + totalWrongCount) * levelSO.idealCookScore;

			return Mathf.Clamp(Mathf.RoundToInt(inGameScore / maxInGame * 1000f), 0, 1000);
		}

		private void SpawnFood()
		{
			// if all slots used, dont spawn
			if (usedSpawnPointIndexes.Count == spawnPoints.Count)
				return;

			// if max food amount present on grill, dont spawn
			if (usedSpawnPointIndexes.Count >= levelSO.numberOfMaxFoods)
				return;

			for (int i = 0; i < 1; i++)
			{
				int spawnPointIndex = GetSpawnPointIndex();
				Food food = Instantiate(GetFood(), spawnPoints[spawnPointIndex].position, Quaternion.Euler(new Vector3(0f, 0f, 0f)), spawnParent);
				food.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
				food.SetSlotIndex(spawnPointIndex);
			}
		}

		private Food GetFood()
		{
			return foodsToSpawn[Random.Range(0, foodsToSpawn.Count)];
		}

		private void CategorizeFoods()
		{
			foreach (Food food in foodPrefabs)
			{
				if (food.GetFoodType() == FoodType.Fast)
					fastFoods.Add(food);
				else if (food.GetFoodType() == FoodType.Medium)
					mediumFoods.Add(food);
				else if (food.GetFoodType() == FoodType.Slow)
					slowFoods.Add(food);
			}
		}

		private void PrepareFoodsToSpawn()
		{
			for (int i = 0; i < levelSO.fastSpawnRate; i++)
			{
				foodsToSpawn.Add(GetFastFood());
			}

			for (int i = 0; i < levelSO.mediumSpawnRate; i++)
			{
				foodsToSpawn.Add(GetMediumFood());
			}

			for (int i = 0; i < levelSO.slowSpawnRate; i++)
			{
				foodsToSpawn.Add(GetSlowFood());
			}

			foodsToSpawn.Shuffle();
		}

		private Food GetFastFood()
		{
			return fastFoods[Random.Range(0, fastFoods.Count)];
		}

		private Food GetMediumFood()
		{
			return mediumFoods[Random.Range(0, mediumFoods.Count)];
		}

		private Food GetSlowFood()
		{
			return slowFoods[Random.Range(0, slowFoods.Count)];
		}

		private int GetSpawnPointIndex()
		{
			int pointIndex;

			do
			{
				pointIndex = Random.Range(0, spawnPoints.Count);

			} while (usedSpawnPointIndexes.Contains(pointIndex));

			usedSpawnPointIndexes.Add(pointIndex);

			return pointIndex;
		}

		public void FreeSpawnPoint(int pointIndex)
		{
			usedSpawnPointIndexes.Remove(pointIndex);
		}

		public BoxCollider GetGrillCollider()
		{
			return grillCollider;
		}

		public void Correct(bool isIdeal)
		{
			if (isIdeal)
			{
				ideallyCookedCount++;
				totalIdeallyCookedCount++;
			}
			else
			{
				correctCount++;
				totalCorrectCount++;
			}
			uiManager.UpdateCorrectText(correctCount + ideallyCookedCount);
			DecideLevel();
		}

		public void Wrong()
		{
			wrongCount++;
			totalWrongCount++;
			uiManager.UpdateWrongText(wrongCount);
			DecideLevel();
		}

		private void DecideLevel()
		{
			if (wrongCount == 1 && lockWrong1)
			{
				lockWrong1 = false;
				correctCount = 0;
				ideallyCookedCount = 0;
			}
			else if (wrongCount == 2 && lockWrong2)
			{
				lockWrong2 = false;
				correctCount = 0;
				ideallyCookedCount = 0;
				wrongCount = 0;

				ChangeLevel(false);
				return;
			}
			else if (correctCount + ideallyCookedCount >= levelSO.levelUpCriteria)
			{
				ChangeLevel(true);
			}
		}

		public LevelSO GetLevelSO()
		{
			return levelSO;
		}

		public float GetCorrectCount()
		{
			return correctCount + ideallyCookedCount;
		}

		public void ChangeLevel(bool isUp)
		{
			Debug.LogFormat("Changing level {0} to {1}", levelId, isUp ? levelId + 1 : levelId - 1);

			if (isUp)
				levelId++;

			else
				levelId--;

			Debug.LogFormat("New level is {0}", levelId);

			correctCount = 0;
			wrongCount = 0;
			ideallyCookedCount = 0;
			lockWrong1 = true;
			lockWrong2 = true;

			PlayerPrefs.SetInt("Grill_Thrills_LevelID", levelId);

			Debug.Log("Saved level in PlayerPrefs");

			AssignLevelVariables();
		}

		private void Reset()
		{
			usedSpawnPointIndexes.Clear();
			fastFoods.Clear();
			mediumFoods.Clear();
			slowFoods.Clear();
			foodsToSpawn.Clear();

			ideallyCookedCount = 0;
			correctCount = 0;
			wrongCount = 0;
			score = 0;
		}

		IEnumerator MoveCameraRoutine()
		{
			yield return new WaitForEndOfFrame();
			mainCamera.transform.DOMove(endPos, 5f);
			mainCamera.transform.DORotate(endRot, 5f);
			yield return new WaitForSeconds(5f);

			ditherTransparencyMatGray.DOFloat(0f, "_Opacity", 1f);
			ditherTransparencyMatWhite.DOFloat(0f, "_Opacity", 1f);
			yield return new WaitForSeconds(1f);

			GameStateManager.SetGameState(GameState.Playing);
		}
	}

	public static class IListExtensions
	{
		public static void Shuffle<T>(this IList<T> ts)
		{
			var count = ts.Count;
			var last = count - 1;
			for (var i = 0; i < last; ++i)
			{
				var r = UnityEngine.Random.Range(i, count);
				var tmp = ts[i];
				ts[i] = ts[r];
				ts[r] = tmp;
			}
		}
	}
}
