using MLAPI.Messaging;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enjin.SDK.Core;
using System.Threading;
using System;
using System.Threading.Tasks;
using MLAPI;
using UniRx;

namespace Enjin.SDK.Core
{
    public abstract class EnjinManager : MonoBehaviour, IClient, IProvider, Initializeable
    {
        //Enjin Manager Networks Variables
        protected EnjinManagerNetworked _enjinManagerNetworked => _enjinManagerNetworkedProp.Value;
        private ReactiveProperty<EnjinManagerNetworked> _enjinManagerNetworkedProp = new ReactiveProperty<EnjinManagerNetworked>();
        public IReadOnlyReactiveProperty<bool> isConnected;
        
        protected NetworkManager _network;
        private bool initialized = false;

        //public ReactiveProperty<string> AccessToken { get; protected set; } = new ReactiveProperty<string>();

        public virtual void OnEnable()
        {
            isConnected = _enjinManagerNetworkedProp.Select(x => x != null).ToReactiveProperty();
            DepInjector.AddProvider(this);
        }

        public virtual void OnDisable()
        {
            DepInjector.Remove(this);
        }

        public virtual void OnDestroy()
        {
            DepInjector.Remove(this);
        }

        public virtual void NewProviderAvailable(IProvider newProvider)
        {
            DepInjector.MapProvider<NetworkManagerProvider, NetworkManager>(newProvider, ref _network);
            DepInjector.MapProvider(newProvider, _enjinManagerNetworkedProp);
        }

        public virtual void NewProviderFullyInstalled(IProvider newProvider)
        {
            TryInitialize(false);
        }
        public virtual void ProviderRemoved(IProvider removeProvider)
        {
            if(DepInjector.UnmapProvider<NetworkManagerProvider, NetworkManager>(removeProvider, ref _network) || DepInjector.UnmapProvider(removeProvider, _enjinManagerNetworkedProp))
            {
                DeInitialize();
            }
        }

        public virtual bool CanInitialize()
        {
            return _enjinManagerNetworked != null && _network != null;
        }
        public void TryInitialize(bool requireIntiailization = false)
        {
            initialized = initialized && !requireIntiailization;
            if (!initialized && CanInitialize())
            {
                Initialize();
            }
        }
        public void Initialize()
        {
            initialized = true;
            Init();
        }

        public void DeInitialize()
        {
            if (initialized)
            {
                initialized = false;
                DeInit();
            }
        }
        

        protected virtual void DeInit() { }
        protected virtual void Init() { }
    }
}