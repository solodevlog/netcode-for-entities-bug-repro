using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
[RequireMatchingQueriesForUpdate]
partial struct GoInGameServerSystem : ISystem
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
        var entitiesReference = SystemAPI.GetSingleton<EntitiesReferences>();

        using var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (rpc, entity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>>().WithAll<GoInGameRequestRpc>()
                     .WithEntityAccess())
        {
            ecb.AddComponent<NetworkStreamInGame>(rpc.ValueRO.SourceConnection);

            Debug.Log($"Client Connected to Server");
            ecb.DestroyEntity(entity);

            var networkId = state.EntityManager.GetComponentData<NetworkId>(rpc.ValueRO.SourceConnection).Value;
            var characterEntity = ecb.Instantiate(entitiesReference.CharacterPrefabEntity);

            var spawnPosition = new float3(UnityEngine.Random.Range(-10f, +10f), 0f, 0f);
            
            ecb.SetComponent(characterEntity, LocalTransform.FromPosition(spawnPosition));
            
            ecb.AddComponent(characterEntity, new GhostOwner { NetworkId = networkId });
            ecb.AppendToBuffer(rpc.ValueRO.SourceConnection, new LinkedEntityGroup { Value = characterEntity });
        }

        ecb.Playback(state.EntityManager);
    }
}

