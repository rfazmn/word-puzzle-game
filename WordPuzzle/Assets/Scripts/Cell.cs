using DG.Tweening;
using UnityEngine;

[SelectionBase]
public class Cell : MonoBehaviour
{
    public char character;
    public SpriteRenderer backgroundSprite;
    public SpriteRenderer charSprite;
    public Vector2Int gridPosition;
    bool selected;

    public void Init(char _character, Sprite _charSprite, Vector2Int _gridPosition)
    {
        character = _character;
        charSprite.sprite = _charSprite;
        gridPosition = _gridPosition;
        selected = false;
    }

    public void Tickle()
    {
        transform.DOKill(true);
        transform.DOPunchRotation(new Vector3(0f, 0f, 15f), .15f, 20).SetEase(Ease.Linear);
    }

    public bool IsSelected()
    {
        return selected;
    }

    public void SetSelected(bool value)
    {
        selected = value;
    }

    public void OnCellRemoved()
    {
        SetSelected(false);
    }

    public bool IsNeighbour(Vector2Int _gridPos)
    {
        for (int i = 0; i < Util.eightDir.Count; i++)
        {
            if (gridPosition + Util.eightDir[i] == _gridPos)
                return true;
        }

        return false;
    }
}
