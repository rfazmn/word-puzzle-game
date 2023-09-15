using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GridController : Singleton<GridController>
{
    [SerializeField] Cell cellPrefab;
    public Transform cellParent;
    [SerializeField] SpriteRenderer gridBg;
    public Vector2Int gridSize;
    float cellUnitSize = 1f;

    #region GridStuff
#if UNITY_EDITOR
    [ContextMenu("Create Grid")]
    //This method does not guarantee that all words will be in the grid.
    //Make sure the size of the grid is large enough.
    //You can add words that are in your word list but not in the grid with the "Grid Controller" -> "Change Cell" method.
    public void CreateGrid()
    {
        ClearGrid();

        if (gridSize.x <= 0 || gridSize.y <= 0)
            return;

        GameManager gameManager = FindObjectOfType<GameManager>();
        WordChecker wordChecker = FindObjectOfType<WordChecker>();

        float backgroundSpacing = .225f;

        Vector3 bgPosition = new Vector3(0f, 0f, 1f);
        Vector2 bgSize = new Vector2(gridSize.x + backgroundSpacing, gridSize.y + backgroundSpacing);
        InitGridBackground(bgPosition, bgSize);
        InitGameCamByGridWidth();

        float xStart = -gridSize.x * .5f + cellUnitSize * .5f;
        float yStart = -gridSize.y * .5f + cellUnitSize * .5f;
        Vector2 startPosition = new Vector2(xStart, yStart);

        Dictionary<Vector2Int, int> filledPositions = new Dictionary<Vector2Int, int>();
        List<Word> tempWords = new List<Word>(wordChecker.wordList);

        int wordIndex = 0;

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector2Int gridPos = new Vector2Int(x, y);
                if (filledPositions.TryGetValue(gridPos, out int _))
                    continue;

                int currentIndex = x * gridSize.x + y + 1;
                float rate = (float)currentIndex / (gridSize.x * gridSize.y);
                float fillChance = Random.Range(rate, 1f);
                float avgChance = (1f + rate) / 2f;

                if (fillChance >= avgChance)
                {
                    string answer = tempWords[wordIndex].answer;

                    int charIndex = answer[0] - 'a';
                    int filledPosCount = filledPositions.Count;
                    List<Vector2Int> tempFilledPos = new List<Vector2Int>();
                    filledPositions.Add(gridPos, charIndex);
                    tempFilledPos.Add(gridPos);

                    for (int i = 1; i < answer.Length; i++)
                    {
                        List<Vector2Int> availablePositions = GetAvailableNeighbours(filledPositions, gridPos);
                        if (availablePositions == null)
                        {
                            for (int j = 0; j < tempFilledPos.Count; j++)
                            {
                                filledPositions.Remove(tempFilledPos[j]);
                            }

                            break;
                        }

                        charIndex = answer[i] - 'a';
                        gridPos = availablePositions[Random.Range(0, availablePositions.Count)];
                        filledPositions.Add(gridPos, charIndex);
                        tempFilledPos.Add(gridPos);
                    }

                    if (filledPosCount == filledPositions.Count)
                        continue;

                    wordIndex++;
                    if (wordIndex == tempWords.Count)
                        goto outOfFor;
                }
            }
        }


    outOfFor:

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector2Int gridPos = new Vector2Int(x, y);
                int charIndex = filledPositions.ContainsKey(gridPos) ? filledPositions[gridPos] : Random.Range(0, gameManager.chars.Length);
                Cell cell = Instantiate(cellPrefab, cellParent);
                cell.Init(gameManager.chars[charIndex].name[0], gameManager.chars[charIndex], new Vector2Int(x, y));
                cell.transform.position = startPosition;
                startPosition.y += cellUnitSize;
            }

            startPosition.y = yStart;
            startPosition.x += cellUnitSize;
        }
    }

    public void LoadGrid(List<CellData> _grid)
    {
        ClearGrid();

        if (gridSize.x <= 0 || gridSize.y <= 0)
            return;

        GameManager gameManager = FindObjectOfType<GameManager>();

        float xStart = -gridSize.x * .5f + cellUnitSize * .5f;
        float yStart = -gridSize.y * .5f + cellUnitSize * .5f;
        Vector2 startPosition = new Vector2(xStart, yStart);

        float backgroundSpacing = .225f;
        Vector3 bgPosition = new Vector3(0f, 0f, 1f);
        Vector2 bgSize = new Vector2(gridSize.x + backgroundSpacing, gridSize.y + backgroundSpacing);
        InitGridBackground(bgPosition, bgSize);
        InitGameCamByGridWidth();

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                int currentIndex = x * gridSize.x + y;
                int charIndex = _grid[currentIndex].character - 'a';
                Cell cell = Instantiate(cellPrefab, cellParent);
                cell.Init(_grid[currentIndex].character, gameManager.chars[charIndex], new Vector2Int(x, y));
                cell.transform.position = startPosition;
                startPosition.y += cellUnitSize;
            }

            startPosition.y = yStart;
            startPosition.x += cellUnitSize;
        }
    }
#endif

    void ClearGrid()
    {
        if (cellParent == null)
        {
            cellParent = new GameObject("CellParent").transform;
            return;
        }

        int cellCount = cellParent.childCount;
        for (int i = 0; i < cellCount; i++)
        {
            DestroyImmediate(cellParent.GetChild(0).gameObject);
        }
    }

    void InitGridBackground(Vector3 position, Vector2 size)
    {
        gridBg.transform.position = position;
        gridBg.size = size;
    }

    void InitGameCamByGridWidth()
    {
        int sizeDiff = gridSize.y - gridSize.x;
        float orthographicSize = sizeDiff >= 3 ? gridSize.y - 2.5f : gridSize.x;
        GameManager gameManager = FindObjectOfType<GameManager>();
        gameManager.mainCam.orthographicSize = Mathf.Max(8, orthographicSize);
    }

    List<Vector2Int> GetAvailableNeighbours(Dictionary<Vector2Int, int> filledPositions, Vector2Int currentPosition)
    {
        List<Vector2Int> neigbourPositions = new List<Vector2Int>();

        for (int x = Mathf.Max(0, currentPosition.x - 1); x <= Mathf.Min(currentPosition.x + 1, gridSize.x - 1); x++)
        {
            for (int y = Mathf.Max(0, currentPosition.y - 1); y <= Mathf.Min(currentPosition.y + 1, gridSize.y - 1); y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                if ((pos.x == currentPosition.x && pos.y == currentPosition.y) || filledPositions.TryGetValue(pos, out int _))
                    continue;

                neigbourPositions.Add(pos);
            }
        }

        return neigbourPositions.Count > 0 ? neigbourPositions : null;
    }

    #endregion

    #region ChangeCells

    public char targetChar; 
#if UNITY_EDITOR
    [ContextMenu("Change Cell")]
    void ChangeCellManually()
    {
        int charIndex = targetChar - 'a';
        if (charIndex < 0 || charIndex > 25)
            return;

        GameManager gameManager = FindObjectOfType<GameManager>();

        GameObject[] selectedObjects = Selection.gameObjects;
        for (int i = 0; i < selectedObjects.Length; i++)
        {
            if (!selectedObjects[i].TryGetComponent(out Cell selectedCell))
                continue;

            selectedCell.Init(gameManager.chars[charIndex].name[0], gameManager.chars[charIndex], selectedCell.gridPosition);
        }
    }
#endif
    #endregion
}
