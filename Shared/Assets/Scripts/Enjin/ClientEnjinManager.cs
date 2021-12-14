using Enjin.SDK.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Enjin.SDK.DataTypes;
using System.Threading;
using System.Linq;
using UnityEngine.Networking;
using System.Threading.Tasks;

namespace Enjin.SDK.Core {
    public class ClientEnjinManager : EnjinManager
    {
        [SerializeField]
        private string _platformURL;
        public string PlatformURL => _platformURL;

        [SerializeField] 
        private int _appID;
        public int AppID => _appID;

        [SerializeField]
        private string _access_Token;
        public string Access_Token => _access_Token;

        [SerializeField]
        private int _playerIdentityID;

        [SerializeField]
        private string _playerETHAddress;

        [SerializeField]
        private string _playerLinkCode;

        [SerializeField]
        private string _playerLink;

        public string Username { get; set; }

        public ReactiveProperty<Texture2D> QRCode = new ReactiveProperty<Texture2D>();
        public ReactiveProperty<bool> isLogin = new ReactiveProperty<bool>(false);

        public void Login(string userName)
        {
            if (_enjinManagerNetworked == null) return;
            _enjinManagerNetworked.LoginServerRpc(_network.LocalClientId, userName);
        }
        public void CreateUser(string userName)
        {
            if (_enjinManagerNetworked == null) return;
            _enjinManagerNetworked.RegisterServerRpc(_network.LocalClientId, userName);
        }
        public void PingServer()
        {
            if (_enjinManagerNetworked == null) return;
            Debug.Log("Ping");
            _enjinManagerNetworked.PingServerRpc(_network.LocalClientId);
        }
        public void MintItem()
        {
            _enjinManagerNetworked.MintItemServerRpc(_network.LocalClientId, "70000000000010dc", 1);
        }
        public async void LoadQRCode(string _LinkCode)
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(_LinkCode);
            await www.SendWebRequest();

            //Return or Error Request
            if (www.result == UnityWebRequest.Result.Success)
            {
                Texture2D myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                QRCode.Value = myTexture;
            }
            else
            {
                Debug.Log($"Error - Bad Link {www.result}");
            }
        }

        protected override void Init()
        {
            _enjinManagerNetworked.OnCreateSuccess += LoadQRCode;
            _enjinManagerNetworked.OnLogicSuccess += OnLogicSuccess;
        }

        private void OnLogicSuccess()
        {
            isLogin.Value = true;
        }

        protected override void DeInit()
        {
            isLogin.Value = false;
        }

        public void UpdateAccessToken()
        {
            if (_enjinManagerNetworked == null) return;
            Debug.Log("Requesting Update");
            //_enjinManagerNetworked.RequestTokenServerRPC(_network.LocalClientId);
        }
    }
}