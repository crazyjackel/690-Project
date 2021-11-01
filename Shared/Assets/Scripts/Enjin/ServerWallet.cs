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
        [SerializeField] string PLATFORM_URL;
        [SerializeField] string APP_SECRET;
        [SerializeField] int APP_ID;

        private string Access_Token;
        public void StartPlatform()
        {
            //Prevent Multiple Instancing of Starting Platform to make Startup Thread-Safe
            Enjin.StartPlatform(PLATFORM_URL, APP_ID, APP_SECRET);
            Debug.Log($"Started Platform: {Enjin.AccessToken}");
            Access_Token = Enjin.AccessToken;
        }

        public string GetAccessToken()
        {
            return Access_Token;
        }
    }
}
