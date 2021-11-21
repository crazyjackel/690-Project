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

    [CreateAssetMenu(fileName = "PlayerWallet", menuName = "Enjin/PlayerWallet")]
    public class PlayerWallet : ScriptableObject
    {

        [SerializeField] string PLATFORM_URL;
        [SerializeField] int APP_ID;

        [SerializeField] string UserName;

        public ReactiveProperty<Texture2D> QRCode = new ReactiveProperty<Texture2D>();

        private string Access_Token;

        public void StartSession(string AccessToken)
        {
            Enjin.StartPlatformWithToken(PLATFORM_URL, APP_ID, AccessToken);
            Debug.Log($"Started Platform: {AccessToken}");
            Access_Token = AccessToken;
        }

        public void CreateUser()
        {
            User userInfo = Enjin.CreatePlayer(UserName);
        }

        public IObservable<Texture2D> RequestLinkQRCode()
        {
            return Observable.FromCoroutine<Texture2D>((observer, cancellationToken) => GetTexture(UserName, observer, cancellationToken));
        }

        IEnumerator GetTexture(string username, IObserver<Texture2D> observer, CancellationToken cancellationToken)
        {
            //Get User
            User userInfo = Enjin.GetUser(username);
            //Validate User
            if (userInfo == null)
            {
                observer.OnError(new Exception("Bad Username"));
                yield break;
            }

            //Get Identity
            Identity UserAppID = userInfo.identities.FirstOrDefault(
                iden =>
                {
                    if (iden != null && iden.app != null)
                    {
                        return iden.app.id == APP_ID;
                    }
                    return false;
                });
            //Validate Identity
            if (UserAppID == null)
            {
                observer.OnError(new Exception("Missing Identity"));
                yield break;
            }

            //Get QR
            string QR = UserAppID.linkingCodeQr;
            //Validate QR
            if (QR != null && QR != "")
            {
                observer.OnError(new Exception("Bad Link"));
                yield break;
            }

            //Do Request
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(QR);
            yield return www.SendWebRequest();

            //Return or Error Request
            if (www.result != UnityWebRequest.Result.Success)
            {
                observer.OnError(new Exception(www.error));
            }
            else
            {
                Texture2D myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                observer.OnNext(myTexture);
                observer.OnCompleted();
            }
        }
    }
}
