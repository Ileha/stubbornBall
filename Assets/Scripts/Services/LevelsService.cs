using System;
using System.Collections.Generic;
using Common;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Services
{
    public class LevelsService : IInitializable, IDisposable 
    {
        private const int MenyLevel = 0;
        private AbstractSaver<ProgramSaveData> _levelData;

        public LevelsService() 
        {
            _levelData = new AbstractSaver<ProgramSaveData>("level.dat");
        }

        public async UniTask<Level> GetLevelInfo(int level) 
        {
            if (MenyLevel != level && SceneManager.sceneCountInBuildSettings > level) 
            {
                try 
                {
                    return _levelData.Data.availableLevelsData[level];
                }
                catch (Exception err) 
                {
                    Level result = new Level(level);
                    _levelData.Data.availableLevelsData.Add(level, result);
                    await _levelData.Save();
                    return result;
                }
            }
            else 
            {
                throw new Exception("scene is out of range");
            }
        }

        public async UniTask<Level[]> GetAllLevels() 
        {
            int scenesCount = SceneManager.sceneCountInBuildSettings;
            List<Level> resultArray = new List<Level>(scenesCount);

            for (int i = 0; i < scenesCount; i++) 
            {
                if (i == MenyLevel) { continue; }

                if (!_levelData.Data.availableLevelsData.ContainsKey(i)) 
                {
                    Level level = new Level(i);
                    _levelData.Data.availableLevelsData.Add(i, level);
                    resultArray.Add(level);
                    continue;
                }

                resultArray.Add(_levelData.Data.availableLevelsData[i]);
            }

            await _levelData.Save();

            return resultArray.ToArray();
        }

        public async UniTask SetLevelStar(Level level, int stars) 
        {
            level.Stars = Mathf.Max(level.Stars, stars);
            level.passed = true;
            await _levelData.Save();
        }

        public void Initialize()
        {
            _levelData.Load().Wait();
        }

        public void Dispose()
        {
            _levelData.Dispose();
        }
    }
}