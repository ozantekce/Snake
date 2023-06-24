using System.Collections.Generic;
using UnityEngine;

public class TailList
{

    private Node _head;
    private Node _lastTail;

    private int _size = 0;

    public int Size { get => _size; set => _size = value; }
    public Node Head { get => _head; set => _head = value; }
    public Node LastTail { get => _lastTail; set => _lastTail = value; }


    private HashSet<Vector2Int> _containingPositions;


    public TailList(Vector2Int headPos)
    {
        _containingPositions = new HashSet<Vector2Int>();

        _head = new Node(headPos,_containingPositions);
        _lastTail = _head;
        _size = 1;

    }


    public TailList(TailList listOther)
    {

        _containingPositions = new HashSet<Vector2Int>();

        Node currentOther = listOther._head;
        _head = new Node(currentOther.pos, _containingPositions);
        _lastTail = _head;
        _size = 1;
        while (currentOther.next != null)
        {
            currentOther = currentOther.next;
            AddLast(new Node(currentOther.pos, _containingPositions));
            _size++;
        }
    }


    public void AddLast(Node newNode)
    {

        if (_lastTail == null)
        {
            _head = newNode;
            _lastTail = newNode;
        }
        else
        {
            newNode.prev = _lastTail;
            _lastTail.next = newNode;
            _lastTail = newNode;
        }
    }


    public void AddFirst(Node newNode)
    {
        if (_head == null)
        {
            _head = newNode;
            _lastTail = newNode;
        }
        else
        {
            newNode.next = _head;
            _head.prev = newNode;
            _head = newNode;
        }
    }


    public Node RemoveFirst()
    {
        if (_head == null) return null;
        Node removed = _head;
        if (_head.next == null)
        {
            _head = null;
            _lastTail = null;
        }
        else
        {
            _head = _head.next;
            _head.prev = null;
        }
        //
        removed.next = null;
        removed.prev = null;
        //
        return removed;
    }

    public Node RemoveLast()
    {
        if (_lastTail == null) return null;
        Node removed = _lastTail;
        if (_lastTail.prev == null)
        {
            _head = null;
            _lastTail = null;
        }
        else
        {
            _lastTail = _lastTail.prev;
            _lastTail.next = null;
        }
        //
        removed.next = null;
        removed.prev = null;
        //
        return removed;
    }



    public void MoveForward(Vector2Int nextPos)
    {
        Node temp = RemoveLast();
        temp.SetPos(nextPos,_containingPositions);
        AddFirst(temp);

    }

    public void MoveForward(Direction dir)
    {
        Vector2Int nextPos = GetHeadCurrentPos() + dir.ToDelta();
        MoveForward(nextPos);
    }


    public void MoveBackward(Vector2Int nextPos)
    {
        Node temp;
        temp = RemoveFirst();

        temp.SetPos(nextPos, _containingPositions);
        AddLast(temp);

    }


    public void AddNode(Vector2Int nextPos)
    {
        Node temp = new Node(nextPos, _containingPositions);
        temp.SetPos(nextPos, _containingPositions);
        AddFirst(temp);
        _size++;
    }

    public void AddNode(Direction dir)
    {
        Vector2Int nextPos = GetHeadCurrentPos() + dir.ToDelta();
        AddNode(nextPos);
    }


    public void AddNodeToLast(Vector2Int nextPos)
    {
        Node temp = new Node(nextPos, _containingPositions);
        temp.SetPos(nextPos, _containingPositions);
        AddLast(temp);
        _size++;
    }


    public Vector2Int GetHeadCurrentPos()
    {
        return _head.pos;
    }

    public Vector2Int GetLastTailCurrentPos()
    {
        return _lastTail.pos;
    }

    public Vector2Int GetLastTailNextPos()
    {
        if (_lastTail.prev == null)
            return _lastTail.pos;
        else
            return _lastTail.prev.pos;
    }

    public Vector2Int GetHeadPrevPos()
    {
        if (_head.next == null)
            return _head.pos;
        else if (_head.next.next == null)
            return _head.next.pos;
        else return _head.next.next.pos;
    }



    public bool ContainPos(Vector2Int pos)
    {
        return _containingPositions.Contains(pos);
    }


    public class Node
    {
        public Node next;
        public Node prev;
        public Vector2Int pos;

        public Node(Vector2Int pos)
        {
            this.pos = pos;
        }

        public Node(Vector2Int pos, HashSet<Vector2Int> poses) : this(pos)
        {
            poses.Add(pos);
        }


        public void SetPos(Vector2Int pos)
        {
            this.pos = pos;
        }

        public void SetPos(Vector2Int pos, HashSet<Vector2Int> poses)
        {
            if(poses.Contains(this.pos))poses.Remove(this.pos);
            this.pos = pos;
            poses.Add(this.pos);
        }

    }


}

