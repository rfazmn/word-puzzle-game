using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class SaveLoadSystem : MonoBehaviour
{
    public SaveLoadType saveLoadType;
    public string fileName;

    ISaveLoad GetSaveLoadByType(SaveLoadType saveLoadType)
    {
        ISaveLoad saveLoad = null;
        switch (saveLoadType)
        {
            case SaveLoadType.json:
                saveLoad = new JSONSaveLoad();
                break;
        }

        return saveLoad;
    }

#if UNITY_EDITOR
    [ContextMenu("Load Scene")]
    void LoadScene()
    {
        if (string.IsNullOrEmpty(fileName))
        {
            Debug.Log("File name cannot be empty");
            return;
        }

        string fullPath = $"Levels/{saveLoadType}/{fileName}";
        SceneData sceneData = GetSaveLoadByType(saveLoadType)?.LoadScene(fullPath);
        if (sceneData == null)
        {
            Debug.Log("Data could not found");
            return;
        }

        GridController gridController = FindObjectOfType<GridController>();
        WordChecker wordChecker = FindObjectOfType<WordChecker>();

        if (gridController == null || wordChecker == null)
        {
            Debug.Log("Some of object references could not found");
            return;
        }

        wordChecker.wordList = sceneData.words;

        gridController.gridSize = new Vector2Int(sceneData.gridSize.x, sceneData.gridSize.y);
        gridController.LoadGrid(sceneData.grid);
    }

    [ContextMenu("Save Scene")]
    void SaveScene()
    {
        if (string.IsNullOrEmpty(fileName))
        {
            Debug.Log("File name cannot be empty");
            return;
        }

        GridController gridController = FindObjectOfType<GridController>();
        WordChecker wordChecker = FindObjectOfType<WordChecker>();

        if (gridController == null || wordChecker == null)
        {
            Debug.Log("Some of object references could not found");
            return;
        }

        Vector2Int gridSize = gridController.gridSize;
        Transform cellParent = gridController.cellParent;

        if(gridController.cellParent == null)
        {
            Debug.Log("Cells not found");
            return;
        }

        int cellsCount = cellParent.childCount;

        if (cellsCount != gridSize.x * gridSize.y)
        {
            Debug.Log("Grid size and cells are not matching");
            return;
        }

        List<CellData> grid = new List<CellData>();

        for (int i = 0; i < cellsCount; i++)
        {
            if (cellParent.GetChild(i).TryGetComponent(out Cell cell))
                grid.Add(new CellData(cell.character));
        }

        SceneData sceneData = new SceneData(gridSize, grid, wordChecker.wordList);
        FileInfo fileInfo = new FileInfo($"{Application.dataPath}/Resources/Levels/{saveLoadType}/{fileName}.json");
        fileInfo.Directory.Create();
        GetSaveLoadByType(saveLoadType)?.SaveScene(fileInfo.FullName, sceneData);
        AssetDatabase.Refresh();
    }
#endif
}
