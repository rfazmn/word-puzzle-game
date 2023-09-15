using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Word
{
    [JsonProperty("w0")] public string question;
    [JsonProperty("w1")] public string answer;
}

public interface ISaveLoad
{
    SceneData LoadScene(string fileName);
    void SaveScene(string fileName, SceneData sceneData);
}

public enum SaveLoadType
{
    json
}

#region SaveLoad
[Serializable]
public class SceneData
{
    [JsonProperty("s0")] public Vector2Ints gridSize;
    [JsonProperty("s1")] public List<CellData> grid;
    [JsonProperty("s2")] public List<Word> words;

    public SceneData(Vector2Int _gridSize, List<CellData> _grid, List<Word> _words)
    {
        gridSize = new Vector2Ints(_gridSize.x, _gridSize.y);
        grid = _grid;
        words = _words;
    }
}

[Serializable]
public class CellData
{
    [JsonProperty("c0")] public char character;

    public CellData(char _ch)
    {
        character = _ch;
    }
}
#endregion

[Serializable]
public struct Vector2Ints
{
    public int x;
    public int y;
    public Vector2Ints(int _x, int _y)
    {
        x = _x;
        y = _y;
    }
}