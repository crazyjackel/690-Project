using Enjin.SDK.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerSideWalletManager : MonoBehaviour
{
    public static ServerSideWalletManager Singleton;


    private void Awake()
    {
        if (Singleton != null && Singleton != this)
        {
            Destroy(gameObject);
            return;
        }
        Singleton = this;
        DontDestroyOnLoad(gameObject);
    }

}
