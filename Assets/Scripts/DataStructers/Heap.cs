using System;


public class Heap<T> where T : IComparable<T>
{
    private const int INIT_CAPACITY = 11;
    private T[] heap;
    private int CAPACITY;
    private int size;


    public Heap(int capacity)
    {
        this.CAPACITY = capacity;
        heap = new T[capacity];
    }

    public Heap()
    {
        this.CAPACITY = INIT_CAPACITY;
        heap = new T[CAPACITY];
    }



    public void Clear()
    {
        this.size = 0;
    }

    public void Insert(T element)
    {

        if (size < CAPACITY)
        {
            heap[size] = element;

            int current = size;
            size++;
            while (heap[current].CompareTo(heap[GetParentIndex(current)]) < 0)
            {
                Swap(heap, current, GetParentIndex(current));
                current = GetParentIndex(current);
            }
        }
        else
        {
            // GROW
            Grow(CAPACITY + 1);
            Insert(element);
        }

    }

    public T Remove()
    {

        if (!IsEmpty())
        {
            size--;
            return ExtractMin(heap, size);
        }
        else
        {
            throw new Exception("Heap is empty");
            //throw new RuntimeException("Heap is empty");
        }

    }


    public T Peek()
    {
        return heap[0];
    }


    private T ExtractMin(T[] heap, int n)
    {
        T min = heap[0];
        heap[0] = heap[n];
        n = n - 1;
        Heapify(heap, 0, n);
        return min;
    }


    private void Heapify(T[] heap, int i, int n)
    {

        int smallest = i;
        //heap[GetLeftChildIndex(i)].val < heap[smallest].val
        if (GetLeftChildIndex(i) <= n
            && heap[GetLeftChildIndex(i)].CompareTo(heap[smallest]) < 0)
        {
            smallest = GetLeftChildIndex(i);
        }
        //heap[GetRightChildIndex(i)].val < heap[smallest].val
        if (GetRightChildIndex(i) <= n
            && heap[GetRightChildIndex(i)].CompareTo(heap[smallest]) < 0)
        {
            smallest = GetRightChildIndex(i);
        }

        if (smallest != i)
        {
            Swap(heap, i, smallest);
            Heapify(heap, smallest, n);
        }

    }


    private void Swap(T[] heap, int i1, int i2)
    {
        T holder = heap[i1];
        heap[i1] = heap[i2];
        heap[i2] = holder;
    }


    private void Grow(int minCapacity)
    {
        int oldCapacity = heap.Length;
        int newCapacity = oldCapacity + (oldCapacity >> 1);
        if (newCapacity - minCapacity < 0)
            newCapacity = minCapacity;
        Array.Resize(ref heap, newCapacity);
        CAPACITY = newCapacity;
    }


    public int Count { get { return size; } }

    public T[] HeapData { get => heap; set => heap = value; }

    public bool IsEmpty() { return size == 0; }


    private int GetLeftChildIndex(int index) { return index * 2 + 1; }
    private int GetRightChildIndex(int index) { return index * 2 + 2; }
    private int GetParentIndex(int index) { return (int)((index / 2.0) - 0.5); }

    /*
    private class Element<E> where E : IComparable<E>
    {
        public E element;
        public Element(E element)
        {
            this.element = element;
        }

        public int CompareTo(Element<E> other)
        {
            if(this.element==null) return -1;
            else if(other == null || other.element == null) return +1;
            return element.CompareTo(other.element);
        }



    }
    */

}