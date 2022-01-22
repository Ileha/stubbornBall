using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.SceneManagement;
using Zenject;

public class SavesService : IInitializable, IDisposable 
{
	private const int MenyLevel = 0;
	
	private BinaryFormatter _formatter = new BinaryFormatter();
	private string _path = Application.persistentDataPath + "/level.dat";
	private ProgramSaveData data;

	public SavesService() 
	{
		data = new ProgramSaveData();
	}

	public Level GetLevelInfo(int level) 
	{
		if (MenyLevel != level && SceneManager.sceneCountInBuildSettings > level) 
		{
			try 
			{
				return data.availableLevelsData[level];
			}
			catch (Exception err) 
			{
				Level result = new Level(level);
				data.availableLevelsData.Add(level, result);
				Save();
				return result;
			}
		}
		else 
		{
			throw new Exception("scene is out of range");
		}
	}

	public Level[] GetAllLevels() 
	{
		int scenesCount = SceneManager.sceneCountInBuildSettings;
		List<Level> resultArray = new List<Level>(scenesCount);

		for (int i = 0; i < scenesCount; i++) 
		{
			if (i == MenyLevel) { continue; }

			if (!data.availableLevelsData.ContainsKey(i)) 
			{
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

	public void SetLevelStar(Level level, int stars) 
	{
		level.Stars = Mathf.Max(level.Stars, stars);
		level.passed = true;
		Save();
	}

	public void Save() 
	{
		using (FileStream fs = new FileStream(_path, FileMode.OpenOrCreate)) 
		{
		    _formatter.Serialize(fs, data);
		}
	}

	private void Load() 
	{
		using (FileStream fs = new FileStream(_path, FileMode.OpenOrCreate)) 
		{
			try 
			{
				//Debug.Log(path);
				data = (ProgramSaveData)_formatter.Deserialize(fs);
			}
			catch (Exception err) 
			{
				if (data == null) 
				{
					data = new ProgramSaveData();
				}
			}
		}
	}

	public void Initialize()
	{
		Load();
	}

	public void Dispose()
	{
		Save();
	}
}