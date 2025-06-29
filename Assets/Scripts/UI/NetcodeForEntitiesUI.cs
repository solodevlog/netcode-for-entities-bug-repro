using System;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetcodeForEntitiesUI : MonoBehaviour
{
    [SerializeField] private Button _startServerButton;
    [SerializeField] private Button _joinGameButton;

    private void OnEnable()
    {
        _startServerButton.onClick.AddListener(StartServer);
        _joinGameButton.onClick.AddListener(JoinGame);
    }

    private void OnDisable()
    {
        _startServerButton.onClick.RemoveListener(StartServer);
        _joinGameButton.onClick.RemoveListener(JoinGame);
    }

    private void StartServer()
    {
        var serverWorld = ClientServerBootstrap.CreateServerWorld("ServerWorld");
        var clientWorld = ClientServerBootstrap.CreateClientWorld("ClientWorld");

        foreach (var world in World.All)
        {
            if (world.Flags == WorldFlags.Game)
            {
                world.Dispose();
                break;
            }
        }

        if (World.DefaultGameObjectInjectionWorld == null)
            World.DefaultGameObjectInjectionWorld = serverWorld;

        SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Single);

        ushort port = 7979;
        
        var networkStreamDriver = serverWorld.EntityManager.CreateEntityQuery(typeof(NetworkStreamDriver)).GetSingletonRW<NetworkStreamDriver>();
        networkStreamDriver.ValueRW.Listen(NetworkEndpoint.AnyIpv4.WithPort(port));

        var connectNetworkEndpoint = NetworkEndpoint.LoopbackIpv4.WithPort(port); 
        networkStreamDriver = clientWorld.EntityManager.CreateEntityQuery(typeof(NetworkStreamDriver)).GetSingletonRW<NetworkStreamDriver>();
        networkStreamDriver.ValueRW.Connect(clientWorld.EntityManager, connectNetworkEndpoint);
    }

    private void JoinGame()
    {
        var clientWorld = ClientServerBootstrap.CreateClientWorld("ClientWorld");

        foreach (var world in World.All)
        {
            if (world.Flags == WorldFlags.Game)
            {
                world.Dispose();
                break;
            }
        }

        if (World.DefaultGameObjectInjectionWorld == null)
            World.DefaultGameObjectInjectionWorld = clientWorld;

        SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Single);

        ushort port = 7979;
        string ip = "127.0.0.1";
        
        var connectNetworkEndpoint = NetworkEndpoint.Parse(ip, port); 
        var networkStreamDriver = clientWorld.EntityManager.CreateEntityQuery(typeof(NetworkStreamDriver)).GetSingletonRW<NetworkStreamDriver>();
        networkStreamDriver.ValueRW.Connect(clientWorld.EntityManager, connectNetworkEndpoint);
    }
}
