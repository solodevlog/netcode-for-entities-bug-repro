using Unity.Entities;
using Unity.NetCode;
using UnityEngine;
using UnityEngine.InputSystem;


[UpdateInGroup(typeof(GhostInputSystemGroup))]
[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
public partial class ReadPlayerInputSystem : SystemBase
{
    private InputAction _moveAction;
    private InputAction _lookAction;
    private InputAction _jumpAction;
    
    protected override void OnCreate()
    {
        RequireForUpdate<NetworkStreamInGame>();
        RequireForUpdate<NetcodePlayerInput>();

        _moveAction = InputSystem.actions.FindAction("Move");
        _lookAction = InputSystem.actions.FindAction("Look");
        _jumpAction = InputSystem.actions.FindAction("Jump");
    }

    protected override void OnUpdate()
    {
        var nt = SystemAPI.GetSingleton<NetworkTime>();
        
        nt.LogPrediction(ref CheckedStateRef, "ReadPlayerInputSystem Update");

        var tick = nt.ServerTick;
        var prevTick = tick;
        prevTick.Subtract(1);
        
        foreach (var inputBuffer in SystemAPI
                     .Query<DynamicBuffer<NetcodePlayerInput>>()
                     .WithAll<GhostOwnerIsLocal>())
        {
            var input = new NetcodePlayerInput
            {
                Tick = tick,
                Movement = _moveAction.ReadValue<Vector2>(),
                Look = _lookAction.ReadValue<Vector2>(),
                Buttons = 0,
            };
            
            input.SetButton(Buttons.Jump, _jumpAction.IsPressed());
            inputBuffer.AddCommandData(input);
        }
    }
}