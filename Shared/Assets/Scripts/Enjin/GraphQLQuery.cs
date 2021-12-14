using Enjin.SDK.Core;
using Enjin.SDK.GraphQL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;


public class GraphQLQuery : GraphQuery
{
    public static new GraphQLQuery instance;

    public class RequestHandler
    {
        public ResponseCodes response { get; set; }
        public string queryReturn { get; set; }
    }
    public new void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public static async void Post(string details, Action<string> OnComplete, Action<ResponseCodes> OnError)
    {
        RequestHandler handle = await Post(details);
        if(handle.response == ResponseCodes.SUCCESS)
        {
            OnComplete?.Invoke(handle.queryReturn);
        }
        else
        {
            OnError?.Invoke(handle.response);
        }
    }

    public static IObservable<RequestHandler> Post(string details)
    {
        return Observable.FromCoroutine<RequestHandler>((observer, cancellationToken) => Post(details, observer, cancellationToken));
    }

    public static IEnumerator Post(string details, IObserver<RequestHandler> observer, CancellationToken cancellationToken)
    {
        details = details.Trim('\r');
        details = QuerySorter(details);

        
        Query query = new Query { query = details };
        string jsonData = JsonUtility.ToJson(query);

        UnityWebRequest request = UnityWebRequest.Post(Enjin.SDK.Core.Enjin.GraphQLURL, UnityWebRequest.kHttpVerbPOST);

        request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonData)) as UploadHandler;
        request.SetRequestHeader("Content-Type", "application/json; charset=utf-8");
        request.SetRequestHeader("Authorization", "Bearer " + Enjin.SDK.Core.Enjin.AccessToken);

        request.downloadHandler = new DownloadHandlerBuffer();

        RequestHandler returnValue = new RequestHandler();
        if (request.error != null)
        {
            returnValue.response = ResponseCodes.INTERNAL;
            returnValue.queryReturn = "";
        }
        else
        {
            yield return request.SendWebRequest();
            returnValue.response = (ResponseCodes)System.Convert.ToInt32(request.responseCode);
            returnValue.queryReturn = request.downloadHandler.text;
        }

        observer.OnNext(returnValue);
        observer.OnCompleted();
    }
}
