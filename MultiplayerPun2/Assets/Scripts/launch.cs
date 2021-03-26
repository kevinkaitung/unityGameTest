using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
// Include Facebook namespace
using Facebook.Unity;
 
namespace mySection
{
    public class launch : MonoBehaviourPunCallbacks
    {
        // 遊戲版本的編碼, 可讓 Photon Server 做同款遊戲不同版本的區隔.
        string gameVersion = "1";
 
        [Tooltip("遊戲室玩家人數上限. 當遊戲室玩家人數已滿額, 新玩家只能新開遊戲室來進行遊戲.")]
        [SerializeField]
        private byte maxPlayersPerRoom = 4;

        bool isConnecting = false;

        void Awake()
        {
            // 確保所有連線的玩家均載入相同的遊戲場景
            PhotonNetwork.AutomaticallySyncScene = true;

            /*if (!FB.IsInitialized) {
                // Initialize the Facebook SDK
                FB.Init(InitCallback, OnHideUnity);
            }
            else {
                // Already initialized, signal an app activation App Event
                //FB.ActivateApp();
                FacebookLogin();
            }*/
        }
 
        private void InitCallback ()
        {
            if (FB.IsInitialized) {
                // Signal an app activation App Event
                //FB.ActivateApp();
                // Continue with Facebook SDK
                // ...
                FacebookLogin();
            } else {
                Debug.Log("Failed to Initialize the Facebook SDK");
            }
        }

        private void OnHideUnity (bool isGameShown)
        {
            if (!isGameShown) {
                // Pause the game - we will need to hide
                Time.timeScale = 0;
            } else {
                // Resume the game - we're getting focus again
                Time.timeScale = 1;
            }
        }

        private void FacebookLogin()
        {
            if (FB.IsLoggedIn)
            {
                OnFacebookLoggedIn();
            }
            else
            {
                var perms = new List<string>(){"public_profile", "email", "user_friends"};
                FB.LogInWithReadPermissions(perms, AuthCallback);
            }
        }

        private void AuthCallback (ILoginResult result) {
            if (FB.IsLoggedIn) {
                // AccessToken class will have session details
                //var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
                // Print current access token's User ID
                //Debug.Log(aToken.UserId);
                // Print current access token's granted permissions
                /*foreach (string perm in aToken.Permissions) {
                    Debug.Log(perm);
                }*/
                OnFacebookLoggedIn();
            } else {
                //Debug.Log("User cancelled login");
                Debug.LogErrorFormat("Error in Facebook login {0}", result.Error);
            }
        }

        private void OnFacebookLoggedIn()
        {
            // AccessToken class will have session details
            string aToken = AccessToken.CurrentAccessToken.TokenString;
            string facebookId = AccessToken.CurrentAccessToken.UserId;
            PhotonNetwork.AuthValues = new AuthenticationValues();
            PhotonNetwork.AuthValues.AuthType = CustomAuthenticationType.Facebook;
            PhotonNetwork.AuthValues.UserId = facebookId; // alternatively set by server
            PhotonNetwork.AuthValues.AddAuthParameter("token", aToken);
            PhotonNetwork.ConnectUsingSettings();
            Debug.Log("Success");
        }

        // something went wrong
        public override void OnCustomAuthenticationFailed(string debugMessage)
        {
            Debug.LogErrorFormat("Error authenticating to Photon using Facebook: {0}", debugMessage);
        }

        void Start()
        {
            // 檢查是否與 Photon Cloud 連線
            if (PhotonNetwork.IsConnected)
            {
                // 已連線, 嚐試隨機加入一個遊戲室
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                // 未連線, 嚐試與 Photon Cloud 連線
                Debug.Log("hi");
                PhotonNetwork.GameVersion = gameVersion;
                PhotonNetwork.ConnectUsingSettings();
            }
        }

        // 與 Photon Cloud 連線
        public void Connect()
        {
            var perms = new List<string>(){"public_profile", "email"};
            FB.LogInWithReadPermissions(perms, AuthCallback);
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("PUN 呼叫 OnConnectedToMaster(), 已連上 Photon Cloud.");
            
            isConnecting = true;
            // 確認已連上 Photon Cloud
            // 隨機加入一個遊戲室
            if (isConnecting)
            {
                PhotonNetwork.JoinRandomRoom();
            }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogWarningFormat("PUN 呼叫 OnDisconnected() {0}.", cause);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("PUN 呼叫 OnJoinRandomFailed(), 隨機加入遊戲室失敗.");
            
            // 隨機加入遊戲室失敗. 可能原因是 1. 沒有遊戲室 或 2. 有的都滿了.    
            // 好吧, 我們自己開一個遊戲室.
            PhotonNetwork.CreateRoom("fighting room", new RoomOptions { MaxPlayers = maxPlayersPerRoom });
        }
        
        public override void OnJoinedRoom()
        {
            Debug.Log("PUN 呼叫 OnJoinedRoom(), 已成功進入遊戲室中.");
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                Debug.Log("我是第一個進入遊戲室的玩家");
                Debug.Log("我得主動做載入場景的動作");
                PhotonNetwork.LoadLevel("mainScene");
            }
        }
    }
}