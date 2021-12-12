using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enjin.SDK.Core;
using Enjin.SDK.DataTypes;
using UnityEngine.Networking;
using System.Linq;
using UniRx;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace Enjin.SDK.Core
{
    [Serializable]
    public class PlayerWallet
    {
        [SerializeField]
        private bool _isLoaded = false;
        public bool IsLoaded => _isLoaded; 

        [SerializeField]
        private int _playerIdentityID;
        public int PlayerIdentityID => _playerIdentityID;

        [SerializeField]
        private string _playerETHAddress;
        public string PlayerETHAddress => _playerETHAddress;

        [SerializeField]
        private string _playerLinkCode;
        public string PlayerLinkCode => _playerLinkCode;

        [SerializeField]
        private string _playerLink;
        public string PlayerLink => _playerLink;

        public void LoadFromIdentity(Identity identity)
        {
            _isLoaded = true;
            _playerIdentityID = identity.id;
            _playerETHAddress = identity.wallet.ethAddress;
            _playerLinkCode = identity.linkingCode;
            _playerLink = identity.linkingCodeQr;
        }
    }
}
