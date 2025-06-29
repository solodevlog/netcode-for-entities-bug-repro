using Unity.Mathematics;
using Unity.NetCode;

public enum Buttons
{
    Empty = 0,
    Fire = 1 << 1,
    Jump = 1 << 2,
    Sprint = 1 << 3,
}

public struct NetcodePlayerInput : ICommandData
{
    [GhostField] public NetworkTick Tick { get; set; }
    [GhostField] public float2 Movement;
    [GhostField] public float2 Look;
    [GhostField] public int Buttons;

    public void SetButton(Buttons button, bool value)
    {
        if (value)
        {
            Buttons |= (int)button;
        }
        else
        {
            Buttons &= ~(int)button;
        }
    }

    public readonly bool GetButton(Buttons button)
    {
        return (Buttons & (int)button) != 0;
    }
}