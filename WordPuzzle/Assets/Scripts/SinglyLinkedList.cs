
public class SinglyLinkedList
{
    public Node root;

    public void AddNode(Cell cell)
    {
        root = new Node(cell, root);
    }

    public void RemoveNode()
    {
        root?.cell?.OnCellRemoved();
        root = root.prev;
    }

    public void Clear()
    {
        root = null;
    }

    public bool CheckIsPrevCurrent(Cell cell)
    {
        return (root == null || root.prev == null) ? false : root.prev.cell == cell;
    }
}

public class Node
{
    public Cell cell;
    public Node prev;

    public Node(Cell _cell, Node _prev = null)
    {
        cell = _cell;
        prev = _prev;
    }
}
