using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enjin.SDK.Core;
using System.Threading.Tasks;

namespace Enjin.SDK.Core
{
    [CreateAssetMenu(fileName = "ServerWallet", menuName = "Enjin/ServerWallet")]
    public class ServerWallet : ScriptableObject
    {
        [SerializeField] string _platformUrl;
        public string PlatformURL => _platformUrl;

        [SerializeField] int _appID;
        public int AppID => _appID;

        [SerializeField] int _developerID;
        public int DeveloperID => _developerID;

        [SerializeField] string _developerAddress;
        public string DeveloperAddress => _developerAddress;

        [SerializeField] string _appSecret;

        private string Access_Token;
        public string StartPlatform()
        {
            //Prevent Multiple Instancing of Starting Platform to make Startup Thread-Safe
            Enjin.StartPlatform(_platformUrl, _appID, _appSecret);
            Debug.Log($"Started Platform: {Enjin.AccessToken}");
            Access_Token = Enjin.AccessToken;
            return Access_Token;
        }
    }
}
