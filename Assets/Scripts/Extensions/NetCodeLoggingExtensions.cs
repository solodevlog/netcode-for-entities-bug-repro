using Unity.NetCode;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public static class NetCodeLoggingExtensions
{
    public static void LogPrediction(this NetworkTime time, ref SystemState state, FixedString512Bytes customMessage = default)
    {
        time.LogInternal(state.WorldUnmanaged, customMessage, LogType.Log);
    }

    public static void LogWarningPrediction(this NetworkTime time, ref SystemState state, FixedString512Bytes customMessage = default)
    {
        time.LogInternal(state.WorldUnmanaged, customMessage, LogType.Warning);
    }

    public static void LogErrorPrediction(this NetworkTime time, ref SystemState state, FixedString512Bytes customMessage = default)
    {
        time.LogInternal(state.WorldUnmanaged, customMessage, LogType.Error);
    }

    private static void LogInternal(this NetworkTime time, WorldUnmanaged world, FixedString512Bytes customMessage, LogType logType)
    {
        FixedString32Bytes simulation = time.IsInPredictionLoop
            ? (time.IsFirstTimeFullyPredictingTick ? "Forward" : "Re-simulate")
            : "OutsidePredictionLoop";

        FixedString512Bytes prefix = $"[{time.ServerTick.TickValue}][{simulation}][{world.Name}] ";

        switch (logType)
        {
            case LogType.Warning:
                Debug.LogWarning($"{prefix}{customMessage}");
                break;
            case LogType.Error:
                Debug.LogError($"{prefix}{customMessage}");
                break;
            default:
                Debug.Log($"{prefix}{customMessage}");
                break;
        }
    }
}