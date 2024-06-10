using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Grill_Thrills
{
	public class LevelManager : MonoBehaviour
	{
		public static LevelManager instance;

		[Header("Camera")]
		public Camera mainCamera;
		[SerializeField] private Vector3 startPos;
		[SerializeField] private Vector3 startRot;
		[SerializeField] private Vector3 endPos;
		[SerializeField] private Vector3 endRot;

		[Header("Scene Objects")]
		[SerializeField] private BoxCollider grillCollider;
		[SerializeField] private Transform spawnParent;
		[SerializeField] private List<Transform> spawnPoints = new List<Transform>();
		[SerializeField] private List<Food> foodPrefabs = new List<Food>();
		private List<Transform> usedSpawnPoints = new List<Transform>();

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
		}

		void Start()
		{
			GameStateManager.OnGameStateChanged += OnGameStateChanged;
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
					InvokeRepeating(nameof(SpawnFood), 1f, 3f);
					break;

				default:
					break;
			}
		}

		private void SpawnFood()
		{
			for (int i = 0; i < 1; i++)
			{
				int foodIndex = Random.Range(0, foodPrefabs.Count);
				Food food = Instantiate(foodPrefabs[foodIndex], GetSpawnPoint().position, Quaternion.Euler(new Vector3(0f, 0f, 0f)), spawnParent);
				food.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			}
		}

		private Transform GetSpawnPoint()
		{
			int pointIndex;
			Transform selectedPoint;

			do
			{
				pointIndex = Random.Range(0, spawnPoints.Count);
				selectedPoint = spawnPoints[pointIndex];

			} while (usedSpawnPoints.Contains(selectedPoint));

			usedSpawnPoints.Add(selectedPoint);

			return selectedPoint;
		}

		public BoxCollider GetGrillCollider()
		{
			return grillCollider;
		}

		private void Reset()
		{
			usedSpawnPoints.Clear();
		}

		IEnumerator MoveCameraRoutine()
		{
			yield return new WaitForEndOfFrame();
			mainCamera.transform.DOMove(endPos, 5f);
			mainCamera.transform.DORotate(endRot, 5f);
			yield return new WaitForSeconds(5f);

			GameStateManager.SetGameState(GameState.Playing);
		}
	}
}
