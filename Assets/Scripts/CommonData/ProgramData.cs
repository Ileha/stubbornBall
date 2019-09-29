using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.SceneManagement;

public class ProgramData {
	private BinaryFormatter formatter = new BinaryFormatter();

#if UNITY_STANDALONE
	private string path = "level.dat";
#endif
#if UNITY_ANDROID || UNITY_IOS
	private string path = Application.persistentDataPath + "/level.dat";
#endif

	private ProgramSaveData data;
	public int MenyLevel { get { return 0; } }								//хардкод

	public ProgramData() {
		data = new ProgramSaveData();
	}

	public Level GetLevelInfo(int level) {
		if (MenyLevel != level && SceneManager.sceneCountInBuildSettings > level) {
			try {
				return data.availableLevelsData[level];
			}
			catch (Exception err) {
				Level result = new Level(level);
				data.availableLevelsData.Add(level, result);
				Save();
				return result;
			}
		}
		else {
			throw new Exception("scene is out of range");
		}
	}

	public Level[] GetAllLevels() {
		int scenesCount = SceneManager.sceneCountInBuildSettings;
		List<Level> resultArray = new List<Level>(scenesCount);

		for (int i = 0; i < scenesCount; i++) {
			if (i == MenyLevel) { continue; }

			if (!data.availableLevelsData.ContainsKey(i)) {
				Level level = new Level(i);
				data.availableLevelsData.Add(i, level);
				resultArray.Add(level);
				continue;
			}

			resultArray.Add(data.availableLevelsData[i]);
		}

        Save();

		return resultArray.ToArray();
	}

	public void SetLevelStar(Level level, int stars) {
		level.Stars = Mathf.Max(level.Stars, stars);
		level.passed = true;
		Save();
	}

	public void Save() {
		using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate)) {
		    formatter.Serialize(fs, data);
		}
	}

	public void Load() {
		using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate)) {
			try {
				//Debug.Log(path);
				data = (ProgramSaveData)formatter.Deserialize(fs);
			}
			catch (Exception err) {
				if (data == null) {
					data = new ProgramSaveData();
				}
			}
		}

	}
}

[Serializable]
public class ProgramSaveData {
	public Dictionary<int, Level> availableLevelsData;

	public ProgramSaveData() {
		availableLevelsData = new Dictionary<int, Level>();
	}
}

[Serializable]
public class Level
{
	public int Stars;
	public int SceneNumber;
	public bool passed;

	public Level(int number) {
		passed = false;
		Stars = 0;
		SceneNumber = number;
	}

	public override bool Equals(object other)
	{
		if (other == null) { return false; }
		if (object.ReferenceEquals(this, other)) { return true; }
		if (!(other is Level)) { return false; }
		Level o = other as Level;

		return this.SceneNumber == o.SceneNumber;
	}

	public override int GetHashCode()
	{
		return SceneNumber;
	}
}