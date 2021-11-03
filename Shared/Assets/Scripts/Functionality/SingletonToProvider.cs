using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class SingletonToProvider<T> : MonoBehaviour, IProvider where T:MonoBehaviour
{
    [SerializeField]
    private T _convert;

    public T getRef()
    {
        return _convert;
    }

    public void getRef(ref T val)
    {
        val = _convert;
    }

    private void OnEnable()
    {
        DepInjector.AddProvider(this);
    }

    private void OnDisable()
    {
        DepInjector.Remove(this);
    }

    private void OnDestroy()
    {
        DepInjector.Remove(this);
    }
}
