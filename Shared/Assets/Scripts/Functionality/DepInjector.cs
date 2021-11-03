using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Credit To Shiny Shoe for Helping me learn this when modding there game.
 * Dep Injection is important kids
 */
public static class DepInjector
{
    public static void AddProvider(IProvider addProvider)
    {
        if(addProvider == null || providers.Contains(addProvider))
        {
            return;
        }

        foreach(IClient client in clients)
        {
            client.NewProviderAvailable(addProvider);
        }
        if(addProvider is IClient addClient)
        {
            AddClient(addClient);
        }
        foreach(IClient client in clients)
        {
            client.NewProviderFullyInstalled(addProvider);
        }
        providers.Add(addProvider);
    }

    public static void AddClient(IClient addClient)
    {
        if(addClient == null || clients.Contains(addClient))
        {
            return;
        }

        foreach(IProvider provider in providers)
        {
            addClient.NewProviderAvailable(provider);
        }
        foreach (IProvider provider in providers)
        {
            addClient.NewProviderFullyInstalled(provider);
        }
        clients.Add(addClient);
    }

    public static void Remove<T>(T o)
    {
        if(o is IProvider p)
        {
            RemoveProvider(p);
        }

        if(o is IClient c)
        {
            RemoveClient(c);
        }
    }

    public static bool MapProvider<T,U>(IProvider provider, ref U val) where T: SingletonToProvider<U> where U : MonoBehaviour
    {
        if (provider is T ret)
        {
            val = ret.getRef();
            return true;
        }
        return false;
    }

    public static bool UnmapProvider<T,U>(IProvider provider, ref U val) where T : SingletonToProvider<U> where U : MonoBehaviour
    {
        if (provider is T)
        {
            val = default(U);
            return true;
        }
        return false;
    }

    public static bool MapProvider<T>(IProvider provider, ref T val)
    {
        if(provider is T ret)
        {
            val = ret;
            return true;
        }
        return false;
    }

    public static bool UnmapProvider<T>(IProvider provider, ref T val)
    {
        if(provider is T)
        {
            val = default(T);
            return true;
        }
        return false;
    }

    private static void RemoveProvider(IProvider removeProvider)
    {
        if (!providers.Contains(removeProvider))
        {
            return;
        }
        foreach(IClient client in clients)
        {
            if(client != removeProvider)
            {
                client.ProviderRemoved(removeProvider);
            }
        }
        providers.Remove(removeProvider);
    }

    private static void RemoveClient(IClient removeClient)
    {
        if (clients.Contains(removeClient))
        {
            clients.Remove(removeClient);
        }
    }

    private static volatile List<IProvider> providers = new List<IProvider>();
    private static volatile List<IClient> clients = new List<IClient>();

    private const string Credits = "Thank you, Mark for helping me learn this when I was modding your game";
}
