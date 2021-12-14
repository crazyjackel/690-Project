using Enjin.SDK.DataTypes;
using Enjin.SDK.Core;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System;
using Enjin.SDK.Utility;
using UniRx;
using System.Threading;

public static class EnjinAsync
{
    public class Response<T>
    {
        public T response { get; set; }
        public ResponseCodes code { get; set; }
    }


    public static IObservable<Response<Request>> MintFungibleToken(int appID, int senderID, string address, string itemID, int value)
    {
        string str_itemId = @""""+ itemID + @"""";
        string str_address = @"""" + address + @"""";
        string query = $"mutation mintFToken{{request:CreateEnjinRequest(appId:{appID},identityId:{senderID},type:MINT,mint_token_data:{{token_id:{str_itemId},recipient_address:{str_address},value:{value}}}){{id,encodedData,state}}}}";

        return GraphQLQuery.Post(query).Select(resp =>
        {
            var ret = new Response<Request>
            {
                code = resp.response,
                response = (resp.response != ResponseCodes.SUCCESS) ? null : JsonUtility.FromJson<Request>(EnjinHelpers.GetJSONString(resp.queryReturn, 2))
            };
            return ret;
        });
    }

    public static IObservable<Response<User>> CreateUser(string username)
    {
        string query = string.Format(Enjin.SDK.Core.Enjin.UserTemplate.GetQuery["CreateUser"], username);

        return GraphQLQuery.Post(query).Select(resp =>
        {
            var ret = new Response<User>
            {
                code = resp.response,
                response = (resp.response != ResponseCodes.SUCCESS) ? null : JsonUtility.FromJson<User>(EnjinHelpers.GetJSONString(resp.queryReturn, 2))
            };
            return ret;
        });
    }


    public static IObservable<Response<User>> GetUserObservable(string username)
    {

        string query = string.Format(Enjin.SDK.Core.Enjin.UserTemplate.GetQuery["GetUserForName"], username);
        return GraphQLQuery.Post(query).Select(resp =>
        {
            var ret = new Response<User>
            {
                code = resp.response,
                response = (resp.response != ResponseCodes.SUCCESS) ? null : JsonUtility.FromJson<User>(EnjinHelpers.GetJSONString(resp.queryReturn, 2))
            };
            return ret;
        });
    }

    public static async Task<Response<User>> GetUser(string username)
    {
        string query = string.Format(Enjin.SDK.Core.Enjin.UserTemplate.GetQuery["GetUserForName"], username);
        var resp = await GraphQLQuery.Post(query);
        var ret = new Response<User>
        {
            code = resp.response,
            response = (resp.response != ResponseCodes.SUCCESS) ? null : JsonUtility.FromJson<User>(EnjinHelpers.GetJSONString(resp.queryReturn, 2))
        };
        return ret;
    }

    public static void GetUser(string username, Action<User> OnComplete, Action<ResponseCodes> OnError)
    {
        string query = string.Format(Enjin.SDK.Core.Enjin.UserTemplate.GetQuery["CreateUser"], username);
        GraphQLQuery.Post(
            query, 
            x => OnComplete(JsonUtility.FromJson<User>(EnjinHelpers.GetJSONString(x, 2))), 
            OnError);
    }
}
