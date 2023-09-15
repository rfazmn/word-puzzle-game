using System.Collections.Generic;
using UnityEngine;

public class TouchHandler : Singleton<TouchHandler>
{
    [SerializeField] LineRenderer linePrefab;
    List<LineRenderer> lines = new List<LineRenderer>();
    SinglyLinkedList charList = new SinglyLinkedList();

    bool processTouch = true;

    void Start()
    {
        Input.multiTouchEnabled = false;

        Transform linesParent = new GameObject("LinesParent").transform;
        for (int i = 0; i < WordChecker.Instance.wordList.Count; i++)
        {
            LineRenderer lr = Instantiate(linePrefab, linesParent);
            Color color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
            lr.startColor = color;
            lr.endColor = color;
            lines.Add(lr);
        }
    }

    #region TouchDetection

    private void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        GetTouchEditor();
#else
		GetTouchMobile();
#endif
    }

    private void GetTouchEditor()
    {
        if (Input.GetMouseButton(0))
        {
            CheckHit();
        }

        if (Input.GetMouseButtonUp(0))
        {
            ProcessTouch();
        }
    }

    private void GetTouchMobile()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Moved:
                    CheckHit();
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    ProcessTouch();
                    break;
            }
        }
    }

    #endregion

    void ProcessTouch()
    {
        if (charList.root == null)
            return;

        SetProcessTouch(false);
        WordChecker.Instance.CheckWord(charList);
        charList.Clear();
    }

    public void SetProcessTouch(bool value)
    {
        processTouch = value;
    }

    public bool IsNeighbour(Cell cell)
    {
        return charList.root == null ? true : charList.root.cell.IsNeighbour(cell.gridPosition);
    }

    //This part can also be done with the cell's on mouse enter event
    void CheckHit()
    {
        if (WordChecker.Instance.GetCompleted() || !processTouch)
            return;

        CircleCollider2D hit = (CircleCollider2D)Physics2D.OverlapPoint(GameManager.Instance.mainCam.ScreenToWorldPoint(Input.mousePosition));

        if (hit == null || !hit.TryGetComponent(out Cell cell))
            return;

        if (cell.IsSelected())
        {
            if (charList.CheckIsPrevCurrent(cell))
            {
                charList.RemoveNode();
                lines[WordChecker.Instance.wordIndex].positionCount--;
            }

            return;
        }

        if (!IsNeighbour(cell))
            return;

        charList.AddNode(cell);
        cell.SetSelected(true);
        SetLinePosition(cell.transform.position);
    }

    void SetLinePosition(Vector3 position)
    {
        lines[WordChecker.Instance.wordIndex].positionCount++;
        lines[WordChecker.Instance.wordIndex].SetPosition(lines[WordChecker.Instance.wordIndex].positionCount - 1, position);
    }

    public void ResetLine()
    {
        lines[WordChecker.Instance.wordIndex].positionCount = 0;
    }
}