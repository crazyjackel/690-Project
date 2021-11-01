using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A Gate Structure that can be Opened or Closed to Allow info in.
/// Listeners added when closed are called when the Gate is opened
/// </summary>
public class QueuedGate
{
    private bool isConnected = false;
    private ConcurrentQueue<Action> connectedActions = new ConcurrentQueue<Action>();
    public void Open()
    {
        isConnected = true;
        while(connectedActions.TryDequeue(out Action action))
        {
            action();
        }
    }

    public void Close()
    {
        if (isConnected)
        {
            isConnected = false;
        }
    }

    public void AddListener(Action callback)
    {
        if (isConnected)
        {
            callback();
            return;
        }
        connectedActions.Enqueue(callback);
    }
}

/// <summary>
/// DO NOT CONNECT OUTSIDE OF CLASS
/// </summary>
public class QueuedGate<T>
{
    private T inst;
    private bool isConnected = false;
    private ConcurrentQueue<Action<T>> connectedActions = new ConcurrentQueue<Action<T>>();
    public void Open(T inst)
    {
        isConnected = true;
        this.inst = inst;
        while (connectedActions.TryDequeue(out Action<T> action))
        {
            action(inst);
        }
    }

    public void Close()
    {
        if (isConnected)
        {
            isConnected = false;
        }
    }

    public void AddListener(Action<T> callback)
    {
        if (isConnected)
        {
            callback(inst);
            return;
        }
        connectedActions.Enqueue(callback);
    }
}
