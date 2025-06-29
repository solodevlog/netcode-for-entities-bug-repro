using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup), OrderFirst = true)]
partial struct PredictionUpdateSystem_Test : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<NetworkTime>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var nt = SystemAPI.GetSingleton<NetworkTime>();
        nt.LogPrediction(ref state, "PredictionUpdateSystem_Test Update");
    }
}