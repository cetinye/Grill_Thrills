using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

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

		[Header("Camera")]
		public Camera mainCamera;
		[SerializeField] private Vector3 startPos;
		[SerializeField] private Vector3 startRot;
		[SerializeField] private Vector3 endPos;
		[SerializeField] private Vector3 endRot;

		[Header("Scene Objects")]
		[SerializeField] private BoxCollider grillCollider;
		[SerializeField] private Transform spawnParent;
		[SerializeField] private Material ditherTransparencyMat;
		[SerializeField] private List<Transform> spawnPoints = new List<Transform>();
		[SerializeField] private List<Food> foodPrefabs = new List<Food>();
		[SerializeField] private List<int> usedSpawnPointIndexes = new List<int>();
		private int correctCount = 0, wrongCount = 0;

		[Header("Timer Variables")]
		[SerializeField] private float levelTime;
		private bool isLevelTimerOn = false;
		private float levelTimer;

		[Header("Flash Interval")]
		[SerializeField] private bool isFlashable = true;

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

		}

		void OnEnable()
		{
			ditherTransparencyMat.SetFloat("_Opacity", 1f);
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
			levelSO = levels[levelId];
		}

		private void StartGame()
		{
			AssignLevelVariables();
			GameStateManager.SetGameState(GameState.Idle);
		}

		private void OnGameStateChanged()
		{
			switch (GameStateManager.GetGameState())
			{
				case GameState.Idle:
					Reset();
					StartCoroutine(MoveCameraRoutine());
					break;

				case GameState.Playing:
					Reset();
					levelTimer = levelTime;
					isLevelTimerOn = true;
					InvokeRepeating(nameof(SpawnFood), 1f, 1f);
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
			}

			if (levelTimer <= 5.2f && isFlashable)
			{
				isFlashable = false;
				// GameManager.instance.PlayFx("Countdown", 0.7f, 1f);
				uiManager.FlashRed();
			}
		}

		private void SpawnFood()
		{
			// if all slots used, dont spawn
			if (usedSpawnPointIndexes.Count == spawnPoints.Count)
				return;

			for (int i = 0; i < 1; i++)
			{
				int foodIndex = Random.Range(0, foodPrefabs.Count);
				int spawnPointIndex = GetSpawnPointIndex();
				Food food = Instantiate(foodPrefabs[foodIndex], spawnPoints[spawnPointIndex].position, Quaternion.Euler(new Vector3(0f, 0f, 0f)), spawnParent);
				food.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
				food.SetSlotIndex(spawnPointIndex);
			}
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

		public void Correct()
		{
			correctCount++;
			uiManager.UpdateCorrectText(correctCount);
		}

		public void Wrong()
		{
			wrongCount++;
			uiManager.UpdateWrongText(wrongCount);
		}

		private void Reset()
		{
			usedSpawnPointIndexes.Clear();

			correctCount = 0;
			wrongCount = 0;
		}

		IEnumerator MoveCameraRoutine()
		{
			yield return new WaitForEndOfFrame();
			mainCamera.transform.DOMove(endPos, 5f);
			mainCamera.transform.DORotate(endRot, 5f);
			yield return new WaitForSeconds(5f);

			ditherTransparencyMat.DOFloat(0f, "_Opacity", 1f);
			yield return new WaitForSeconds(1f);

			GameStateManager.SetGameState(GameState.Playing);
		}
	}
}
