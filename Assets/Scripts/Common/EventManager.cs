using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class EventManager : Singleton<EventManager>
{
    private Dictionary<Type, EventDelegate> delegates = new Dictionary<Type, EventDelegate>();
    private Dictionary<Delegate, EventDelegate> delegateLookup = new Dictionary<Delegate, EventDelegate>();

    public delegate void EventDelegate<T>(T e) where T : GlobalEvent;
    public delegate void EventDelegate(GlobalEvent e);

    public void AddListener<T>(EventDelegate<T> del) where T : GlobalEvent
    {

        this.AddDelegate<T>(del);
    }

    private EventDelegate AddDelegate<T>(EventDelegate<T> del) where T : GlobalEvent
    {
        if (this.delegateLookup.ContainsKey(del))
        {
            return null;
        }

        EventDelegate tempDel;
        EventDelegate internalDelegate = (e) => del((T)e);

        this.delegateLookup[del] = internalDelegate;

        if (this.delegates.TryGetValue(typeof(T), out tempDel))
        {
            this.delegates[typeof(T)] = tempDel += internalDelegate;
        }
        else
        {
            this.delegates[typeof(T)] = internalDelegate;
        }

        return internalDelegate;
    }

    public void RemoveListener<T>(EventDelegate<T> del) where T : GlobalEvent
    {
        EventDelegate internalDelegate;

        if (this.delegateLookup.TryGetValue(del, out internalDelegate))
        {
            EventDelegate tempDel;

            if (this.delegates.TryGetValue(typeof(T), out tempDel))
            {
                tempDel -= internalDelegate;

                if (tempDel == null)
                {
                    this.delegates.Remove(typeof(T));
                }
                else
                {
                    this.delegates[typeof(T)] = tempDel;
                }
            }

            this.delegateLookup.Remove(del);
        }
    }

    public bool HasListener<T>(EventDelegate<T> del) where T : GlobalEvent
    {
        return this.delegateLookup.ContainsKey(del);
    }

    public void Trigger(GlobalEvent e)
    {
        EventDelegate del;

        if (this.delegates.TryGetValue(e.GetType(), out del))
        {
            del.Invoke(e);

        }
    }

}
