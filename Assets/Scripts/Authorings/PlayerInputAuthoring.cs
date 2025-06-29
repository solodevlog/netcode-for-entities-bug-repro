using Unity.Entities;
using UnityEngine;

public class PlayerInputAuthoring : MonoBehaviour
{
    private class PlayerInputAuthoringBaker : Baker<PlayerInputAuthoring>
    {
        public override void Bake(PlayerInputAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddBuffer<NetcodePlayerInput>(entity);
        }
    }
}