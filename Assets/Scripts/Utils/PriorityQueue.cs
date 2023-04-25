using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// customize this to take a gameobject and a priority value so you can keep track of node objects
public class PriorityQueue<TElement, TPriority> where TPriority : IComparable<TPriority>
{
    private List<Tuple<TElement, TPriority>> data;

    public PriorityQueue()
    {
        this.data = new List<Tuple<TElement, TPriority>>();
    }

    public void Enqueue(TElement item, TPriority priority)
    {
        data.Add(Tuple.Create(item, priority));
        int ci = data.Count - 1;

        while (ci > 0) 
        {
            int pi = (ci - 1) / 2;
            if (data[ci].Item2.CompareTo(data[pi].Item2) >= 0)
            {
                break;
            }

            Tuple<TElement, TPriority> tmp = data[ci];
            data[ci] = data[pi];
            data[pi] = tmp;

            ci = pi;
        }
    }

    // NOTE: Dequeue only returns the element, not the associated priority
    public TElement Dequeue()
    {
        if (data.Count == 0) { Debug.Log ("Tried to Dequeue but Priority Queue is empty"); return default(TElement); }

        int li = data.Count - 1;

        //store front item for return and remove from list
        Tuple<TElement, TPriority> frontItem = data[0];
        data[0] = data[li];
        data.RemoveAt(li);
        --li;

        // reorganize priority queue after removal
        int pi = 0;
        while (true)
        {
            int ci = pi * 2 + 1;
            if (ci > li)
            {
                break;
            }

            int rc = ci + 1;

            if (rc <= li && data[rc].Item2.CompareTo(data[ci].Item2) < 0)
            {
                ci = rc;
            }

            if (data[pi].Item2.CompareTo(data[ci].Item2) <= 0)
            {
                break;
            }

            Tuple<TElement, TPriority> tmp = data[pi];
            data[pi] = data[ci];
            data[ci] = tmp;

            pi = ci;
        }

        return frontItem.Item1;
    }
    public int Count()
    {
        return data.Count;
    }

    public Tuple<TElement, TPriority> Peek()
    {
        Tuple<TElement, TPriority> frontItem = data[0];
        return frontItem;
    }

    public Tuple<TElement, TPriority> ElementAt(int index)
    {
        Tuple<TElement, TPriority> element = data[index];
        return element;
    }

    public override string ToString()
    {
        string s = "";
        for (int i = 0; i < data.Count; i++)
        {
            s += data[i].ToString() + ", ";
        }

        s += "count = " + data.Count;

        return s;
    }

}
