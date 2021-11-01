using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enjin.SDK.Core;
using Enjin.SDK.DataTypes;

namespace Enjin.SDK.Core
{
    [CreateAssetMenu(fileName = "PlayerWallet", menuName = "Enjin/PlayerWallet")]
    public class PlayerWallet : ScriptableObject
    {
        [SerializeField] string PLATFORM_URL;
        [SerializeField] int APP_ID;

        private string Access_Token;

        public void StartSession(string AccessToken)
        {
            Enjin.StartPlatformWithToken(PLATFORM_URL, APP_ID, AccessToken);
            Debug.Log($"Started Platform: {AccessToken}");
            Access_Token = AccessToken;
        }

    }
}
