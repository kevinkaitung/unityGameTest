using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace mySection
{
    public class gameManager : MonoBehaviourPunCallbacks
    {
        [Tooltip("Prefab- 玩家的角色")]
        public GameObject playerPrefab;
        public GameObject[] spawnPoints = new GameObject[2];
        private Transform[] spawnPointsTrans = new Transform[2];

        private void Start() 
        {
            for(int i = 0; i < 2; i++)
            {
                spawnPointsTrans[i] = spawnPoints[i].GetComponent<Transform>();
            }
            //spawnPointsTrans = spawnPoints.GetComponent<Transform>();
            if (playerPrefab == null)
            {
                Debug.LogError("playerPrefab 遺失, 請在 Game Manager 重新設定", this);
            }
            else
            {
                Debug.LogFormat("動態生成玩家角色 {0}", Application.loadedLevelName);
                PhotonNetwork.Instantiate(this.playerPrefab.name, spawnPointsTrans[PhotonNetwork.CurrentRoom.Players.Count - 1].position, Quaternion.identity, 0);
            }
        }
        // 玩家離開遊戲室時, 把他帶回到遊戲場入口
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        //新玩家進入時，呼叫OnPlayerEnteredRoom
        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.LogFormat("{0} 進入遊戲室", other.NickName);
        }

        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.LogFormat("{0} 離開遊戲室", other.NickName);
        }

    } 
}
