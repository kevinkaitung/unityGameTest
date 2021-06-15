using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Facebook.Unity;
using Facebook.MiniJSON;

public class fbManager
{
    public delegate void OnFBLoginSucced(Facebook.Unity.AccessToken token);
    public delegate void OnFBLoginFaild(bool isCancel, string errorInfo);

    public static void Init()
    {
        FB.Init(() =>
        {
            Debug.Log("FB OnInitComplete!");
            Debug.Log("FB.AppId: " + FB.AppId);
            Debug.Log("FB.GraphApiVersion: " + FB.GraphApiVersion);
            //獲取應用連結
            //FBGetAPPLinkUrl();

        }, (isUnityShutDown) =>
        {
            Debug.Log("FB OnHideUnity： " + isUnityShutDown);
        });
    }

    public static void FBLogin(OnFBLoginSucced onFBLoginSucced = null, OnFBLoginFaild onFBLoginFaild = null)
    {
        var perms = new List<string>() { "public_profile", "email", "user_friends" };
        FB.LogInWithReadPermissions(perms, (result) => 
        {
            if (FB.IsLoggedIn)
            {
                Debug.Log("FBLoginSucceed");
                if (onFBLoginSucced != null)
                {
                    onFBLoginSucced(Facebook.Unity.AccessToken.CurrentAccessToken);
                }
            }
            else
            {
                Debug.Log("FBLoginFaild");
                Debug.Log(result.RawResult);
                if (onFBLoginFaild != null)
                {
                    onFBLoginFaild(result.Cancelled, result.Error);
                }
            }
        });
    }
}
