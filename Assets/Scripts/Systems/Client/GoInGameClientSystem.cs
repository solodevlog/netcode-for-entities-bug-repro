using DefaultNamespace;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
[RequireMatchingQueriesForUpdate]
partial struct GoInGameClientSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EntitiesReferences>();
        state.RequireForUpdate<NetworkId>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (id, entity) in SystemAPI.Query<RefRO<NetworkId>>().WithNone<NetworkStreamInGame>()
                     .WithEntityAccess())
        {
            ecb.AddComponent<NetworkStreamInGame>(entity);
            Debug.Log("Setting Client as InGame");

            var rpcEntity = ecb.CreateEntity();
            ecb.AddComponent<GoInGameRequestRpc>(rpcEntity);
            ecb.AddComponent<SendRpcCommandRequest>(rpcEntity);
        }

        ecb.Playback(state.EntityManager);
    }
}


public struct GoInGameRequestRpc : IRpcCommand
{
}