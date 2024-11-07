using UnityEngine;
using UnityEditor;
using System.IO;
using System;

namespace Grill_Thrills
{
    public class CSVtoSO_Grill_Thrills
    {
        //Check .csv path
        private static string CSVPath = "/Editor/CSVtoSO/Grill_Thrills/LevelCSV_Grill_Thrills.csv";

        [MenuItem("Tools/CSV_to_SO/Grill_Thrills/Generate")]
        public static void GenerateSO()
        {
            int startingNamingIndex = 1;
            string[] allLines = File.ReadAllLines(Application.dataPath + CSVPath);

            for (int i = 1; i < allLines.Length; i++)
            {
                allLines[i] = RedefineString(allLines[i]);
            }

            for (int i = 1; i < allLines.Length; i++)
            {
                string[] splitData = allLines[i].Split(';');

                //Check data indexes
                LevelSO level = ScriptableObject.CreateInstance<LevelSO>();
                level.levelId = int.Parse(splitData[0]);
                level.fastIncluded = Convert.ToBoolean(int.Parse(splitData[1]));
                level.mediumIncluded = Convert.ToBoolean(int.Parse(splitData[2]));
                level.slowIncluded = Convert.ToBoolean(int.Parse(splitData[3]));
                level.fastCookSpeed = int.Parse(splitData[4]);
                level.mediumCookSpeed = int.Parse(splitData[5]);
                level.slowCookSpeed = int.Parse(splitData[6]);
                level.fastSpawnRate = float.Parse(splitData[7]);
                level.mediumSpawnRate = float.Parse(splitData[8]);
                level.slowSpawnRate = float.Parse(splitData[9]);
                level.fastCookRange = float.Parse(splitData[10]);
                level.mediumCookRange = float.Parse(splitData[11]);
                level.slowCookRange = float.Parse(splitData[12]);
                level.numberOfMaxFoods = int.Parse(splitData[13]);
                level.spawnFrequency = float.Parse(splitData[14]);
                level.levelUpCriteria = int.Parse(splitData[15]);
                level.levelDownCriteria = int.Parse(splitData[16]);
                level.idealCookScore = int.Parse(splitData[17]);
                level.rawOvercookScore = int.Parse(splitData[18]);
                level.penaltyPoint = int.Parse(splitData[19]);

                AssetDatabase.CreateAsset(level, $"Assets/Data/Grill_Thrills/Levels/{"GrillThrills_Level " + startingNamingIndex}.asset");
                startingNamingIndex++;
            }

            AssetDatabase.SaveAssets();

            static string RedefineString(string val)
            {
                char[] charArr = val.ToCharArray();
                bool isSplittable = true;

                for (int i = 0; i < charArr.Length; i++)
                {
                    if (charArr[i] == '"')
                    {
                        charArr[i] = ' ';
                        isSplittable = !isSplittable;
                    }

                    if (isSplittable && charArr[i] == ',')
                        charArr[i] = ';';

                    if (isSplittable && charArr[i] == '.')
                        charArr[i] = ',';
                }

                return new string(charArr);
            }
        }
    }
}