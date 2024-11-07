using UnityEngine;

namespace Grill_Thrills
{
	public class LevelSO : ScriptableObject
	{
		public int levelId;
		public bool fastIncluded;
		public bool mediumIncluded;
		public bool slowIncluded;
		public int fastCookSpeed;
		public int mediumCookSpeed;
		public int slowCookSpeed;
		public float fastSpawnRate;
		public float mediumSpawnRate;
		public float slowSpawnRate;
		public float fastCookRange;
		public float mediumCookRange;
		public float slowCookRange;
		public int numberOfMaxFoods;
		public float spawnFrequency;
		public int levelUpCriteria;
		public int levelDownCriteria;
		public int idealCookScore;
		public int rawOvercookScore;
		public int penaltyPoint;
	}
}
