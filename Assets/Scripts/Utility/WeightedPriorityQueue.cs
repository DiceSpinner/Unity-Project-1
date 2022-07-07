using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightedPriorityQueue<T> : IEnumerable<WeightedPriorityWrapper<T>>
{
    /*
    Description: A queue of items sorted by their priorities, items with higher
    priority will be popped first. Additionally, each item has a weight factor
    assigned to it. Popping items will cause their weight factor to drop by 1.
    The item will not be fully removed from the queue if the weight factor is
    greater than 0. Enqueuing an item returns the key to access the weight
    factor of that item. This can be used to modify its weight factor.

    === Private attributes ===
    container: Items stored in this queue. The first item in the queue will be popped from the front.
    size: Size of the queue
    pointer: Points to the current item being popped
    contents: A dictionary that contains the same items as container that provides random access functionlity
    */

    private LinkedList<WeightedPriorityWrapper<T>> container = new LinkedList<WeightedPriorityWrapper<T>>();
    private Dictionary<int, LinkedListNode<WeightedPriorityWrapper<T>>> contents = new();

    private int size = 0;
    private LinkedListNode<WeightedPriorityWrapper<T>> pointer = null;

    public override string ToString()
    {
        if (size == 0)
        {
            return "[]";
        }
        string result = "[";
        foreach (WeightedPriorityWrapper<T> w in container)
        {
            result = result + w.ToString() + ", ";
        }
        result = result.Remove(result.Length - 2) + "]";
        return result;
    }

    public WeightedPriorityWrapper<T> GetWrappedItem(int key)
    {
        if (contents.ContainsKey(key))
        {
            return contents[key].Value;
        }
        throw new InvalidKeyException(key + "");
    }
    public int Enqueue(T element, int priority, int weight)
    {
        int key = IdDistributor.GetId(Setting.ID_WEIGHTED_PRIORITY_QUEUE);
        WeightedPriorityWrapper<T> item = new WeightedPriorityWrapper<T>(element, priority, weight);
        
        LinkedListNode<WeightedPriorityWrapper<T>> node = container.First;
        LinkedListNode<WeightedPriorityWrapper<T>> new_node = new LinkedListNode<WeightedPriorityWrapper<T>>(item);
        item.key = key;
        contents[key] = new_node;

        size++;
        pointer = null;
        while (node != null)
        {
            WeightedPriorityWrapper<T> current = node.Value;
            if (Compare(item, current) <= 0)
            {
                container.AddBefore(node, new_node);
                return key;
            }
            node = node.Next;
        }
        container.AddLast(new_node);
        return key;
    }

    public T Dequeue()
    {
        if (size <= 0)
        {
            return default(T);
        }
        if (pointer == null)
        {
            Reset();
            return default(T);
        }
        WeightedPriorityWrapper<T> item = pointer.Value;
        item.weight -= 1;
        if (item.weight <= 0)
        {
            Remove(pointer.Value);

            if (item.weight < 0)
            {
                return Dequeue();
            }
        }
        else
        {
            pointer = pointer.Next;
        }
        return item.value;
    }

    public int Size()
    {
        return size;
    }

    public bool Remove(int key) {
        if (!contents.ContainsKey(key)) {
            return false;
        }
        container.Remove(contents[key]);
        LinkedListNode<WeightedPriorityWrapper<T>> node = contents[key];
        contents.Remove(key);
        size--;
        IdDistributor.RecycleId(Setting.ID_WEIGHTED_PRIORITY_QUEUE, key);
        if (pointer != null && pointer.Equals(node)) {
            pointer = pointer.Next;
        }
        return true;            
    }

    public bool Remove(WeightedPriorityWrapper<T> w)
    {
        int key = w.key;
        return Remove(key);
    }

    public void SetWeight(int key, int weight)
    {
        if (contents.ContainsKey(key))
        {
            contents[key].Value.weight = weight;
        }
        else
        {
            throw new InvalidOperationException(key + "");
        }

    }

    public int GetWeight(int key)
    {
        if (contents.ContainsKey(key))
        {
            return contents[key].Value.weight;
        }
        throw new InvalidKeyException(key + "");
    }

    public void SetValue(int key, T value)
    {
        if (contents.ContainsKey(key))
        {
            contents[key].Value.value = value;
        }
        else
        {
            throw new InvalidKeyException(key + "");
        }
    }

    public void Reset()
    {
        if (size == 0)
        {
            return;
        }
        pointer = container.First;
    }

    private int Compare(WeightedPriorityWrapper<T> x, WeightedPriorityWrapper<T> y)
    {
        return x.priority - y.priority;
    }

    public IEnumerator<WeightedPriorityWrapper<T>> GetEnumerator()
    {
        return container.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return container.GetEnumerator();
    }
}

public class WeightedPriorityWrapper<T>
{
    public int weight;
    public T value;
    public int priority;
    public int key;
    public WeightedPriorityWrapper(T o, int p, int w)
    {
        weight = w;
        priority = p;
        value = o;
        key = -1;
    }

    public override string ToString()
    {
        return string.Format("{{V: {0}, P: {1}, W: {2}}}", value.ToString(), priority, weight);
    }
}
