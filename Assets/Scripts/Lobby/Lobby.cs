using CoinAction.Game;
using CoinAction.UI;
using Mirror;
using Mirror.Examples.Tanks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CoinAction.Lobby
{
    public class Lobby : NetworkBehaviour
    {
        #region Server
        [Header("Server variables")]

        [SerializeField]
        int playersToStart = 2;

        [SerializeField]
        Match matchPrefab;

        private System.Tuple<bool, NetworkMatch> networkMatch = System.Tuple.Create<bool, NetworkMatch>(false, null);
        private NetworkMatch NetworkMatch
        {
            get
            {
                if (!networkMatch.Item1)
                {
                    networkMatch = System.Tuple.Create<bool, NetworkMatch>(true, this.GetComponent<NetworkMatch>());
                }

                return networkMatch.Item2;
            }
        }

        private readonly Vector3 step = Vector3.up * 100;

        private int matchCounter = 0;

        private Vector3 placing = Vector3.zero;

        private List<Room> rooms;


        public override void OnStartServer()
        {
            base.OnStartServer();

            rooms = new List<Room>();

            NetworkMatch.matchId = Guid.NewGuid();

            CoinAction.NetworkManager.Instance.ClientReady += MoveToLobby;
            CoinAction.NetworkManager.Instance.ClientDisconnected += Kick;
        }

        [Command(requiresAuthority = false)]
        public void CmdCreateRoom(string name, NetworkConnectionToClient sender = null)
        {
            User player;
            Room existent = rooms.FirstOrDefault(r => r.Name == name);

            Debug.Log("CmdCreateRoom " + name);

            if (existent == null)
            {
                Kick(sender);
                player = sender.identity.GetComponent<User>();
                existent = new Room { Name = name, Players = new List<User> { player } };
                rooms.Add(existent);
                if (existent.Players.Count >= playersToStart)
                {
                    existent.Match = Instantiate(matchPrefab, placing, Quaternion.identity, this.transform);
                    existent.Match.Finished += delegate () { rooms.Remove(existent); };
                    matchCounter++;
                    placing += step;
                    if (placing.y >= float.MaxValue - step.y)
                    {
                        placing = Vector3.zero;
                    }
                    NetworkServer.Spawn(existent.Match.gameObject);
                    player.NetworkMatch.matchId = existent.Match.NetworkMatch.matchId;
                }
            }
            else
            {
                Response(sender, LobbyResponses.Exist);
            }
        }

        [Command(requiresAuthority = false)]
        public void CmdJoinRoom(string name, NetworkConnectionToClient sender = null)
        {
            Room existent = rooms.FirstOrDefault(r => r.Name == name);
            User player;

            if (existent != null)
            {
                Kick(sender);
                player = sender.identity.GetComponent<User>();
                existent.Players.Add(player);
                if (existent.Players.Count >= playersToStart)
                {
                    if (existent.Match == null)
                    {
                        existent.Match = Instantiate(matchPrefab, placing, Quaternion.identity, this.transform);
                        placing += step;
                        if (placing.y >= float.MaxValue - step.y)
                        {
                            placing = Vector3.zero;
                        }
                        existent.Match.Finished += delegate () 
                        { 
                            rooms.Remove(existent);
                        };
                        existent.Match.UserLeft += MoveToLobby;
                        NetworkServer.Spawn(existent.Match.gameObject);
                        existent.Match.NetworkMatch.matchId = Guid.NewGuid();
                        foreach (User p in existent.Players)
                        {
                            p.NetworkMatch.matchId = existent.Match.NetworkMatch.matchId;
                        }
                    }
                    else
                    {
                        player.NetworkMatch.matchId = existent.Match.NetworkMatch.matchId;
                    }

                }
                else
                {
                    Response(sender, LobbyResponses.Notexist);
                }
            }
        }

        private void MoveToLobby(NetworkConnectionToClient conn)
        { 
            User user = conn.identity.GetComponent<User>(); 
            if (user != null) 
            {
                user.NetworkMatch.matchId = NetworkMatch.matchId;
            }
        }

        private void Kick(NetworkConnectionToClient conn)
        {
            User user = null;
            Room existent = rooms.FirstOrDefault(r => 
            { 
                user = r.Players.FirstOrDefault(p => p.netIdentity.netId == conn.identity.netId); 
                return user != null; 
            });

            if (user != null)
            {
                existent.Players.Remove(user);
                if (existent.Players.Count == 0)
                {
                    rooms.Remove(existent);
                }
            }
        }
        #endregion


        #region Client

        public override void OnStartClient()
        {
            base.OnStartClient();
        }

        private void ClienStart()
        {
            if (isClient)
            {
                Menus.Instance.LobbyMenu.CreateClick += CreateRoomRequest;
                Menus.Instance.LobbyMenu.JoinClick += JoinRoomRequest;
            }
        }

        private void CreateRoomRequest(string name)
        {
            CmdCreateRoom(name);
        }
        private void JoinRoomRequest(string name)
        {
            CmdJoinRoom(name);
        }

        [TargetRpc]
        private void Response(NetworkConnection target, LobbyResponses response)
        {
            switch (response)
            {
                case LobbyResponses.Notexist:
                    NotificationsPopUp.ShowMessage("Room with same name not exist.");
                    break;
                case LobbyResponses.Exist:
                    NotificationsPopUp.ShowMessage("Room with same name already exist.");
                    break;
            }
        }
        #endregion

        private void Start()
        {
            ClienStart();
        }
    }

    public enum LobbyResponses { Exist, Notexist }
}
