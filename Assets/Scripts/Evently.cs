using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evently
{
    //private static eventManagerInstance
    //public static Instance => private member
    //dictionary that maps types to delegates
    //public, generic subscribe method that allows us to subscribr to events
    //public, generic Unsubscribe method that allows us to subscribe to events
    //public, generic Publish methos that allows us to subscribr to events
    private static Evently eventManagerInstance;
    //if it's null, assign it to a new Evently()
    //Otherwise, just return eventManagerInstance (the right side never gets evaluated)
    public static Evently Instance => eventManagerInstance ??= new Evently();
    //public static Evently Instance
    //{
    //    get
    //    {
    //        if (instance != null) return instance;
    //        if(instance==null)
    //        {
    //            instance = new Evently();
    //        }
    //        return instance;
    //    }
    //}
    private readonly Dictionary<Type, Delegate> delegates = new Dictionary<Type, Delegate>();
    public void Subscribe<T>(Action<T> del)
    {
        if (delegates.ContainsKey(typeof(T)))
        {
            delegates[typeof(T)] = Delegate.Combine(delegates[typeof(T)], del);
        }
        else
        {
            delegates[typeof(T)] = del;
        }
    }
    public void Unsubscribe<T>(Action<T> del)
    {
        if (!delegates.ContainsKey(typeof(T))) return;
        var currentDel = Delegate.Remove(delegates[typeof(T)], del);
        if (currentDel == null)
        {
            delegates.Remove(typeof(T));
        }
        else
        {
            delegates[typeof(T)] = currentDel;
        }
    }
    public void Publish<T>(T e)
    {
        if (e == null)
        {
            Debug.Log($"invalid event arg:{e.GetType()}");
            return;
        }
        if (delegates.ContainsKey(e.GetType()))
        {
            delegates[e.GetType()].DynamicInvoke(e);
        }
    }
}
