using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

//Because of spectator mode it spawns player as spectatorPrefab if it's spectator, and playerPrefab if it's client/host

public class PrefabsNetworkManager : NetworkManager
{
    [SerializeField] GameObject spectatorPrefab;
    [SerializeField] NetworkManagerHUD hud;
    CreateMMOCharacterMessage characterMessage;

    public struct CreateMMOCharacterMessage : NetworkMessage
    {
        public bool ifSpectator;
    }
    public override void OnStartServer()
    {
        base.OnStartServer();
        NetworkServer.RegisterHandler<CreateMMOCharacterMessage>(OnCreateCharacter);
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();
        NetworkClient.Send(characterMessage);
    }

    void OnCreateCharacter(NetworkConnectionToClient conn, CreateMMOCharacterMessage message)
    {
        GameObject gameobject;
        if (message.ifSpectator.Equals(true)) gameobject = Instantiate(spectatorPrefab);
        else gameobject = Instantiate(playerPrefab);
        NetworkServer.AddPlayerForConnection(conn, gameobject);
    }
 
    private void Awake()=> hud.onButtonClick += SetSpectatorMode;

    private void SetSpectatorMode(object sender, bool b)
    {
        characterMessage = new CreateMMOCharacterMessage
        {
            ifSpectator = b
        };
      
    }
}
