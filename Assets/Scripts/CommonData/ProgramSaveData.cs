using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

[Serializable]
public class ProgramSaveData 
{
    public Dictionary<int, Level> availableLevelsData;

    public ProgramSaveData() 
    {
        availableLevelsData = new Dictionary<int, Level>();
    }
}

[Serializable]
public class Level
{
    public int Stars;
    public int SceneNumber;
    public bool passed;

    public Scene Scene => SceneManager.GetSceneByBuildIndex(SceneNumber);

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