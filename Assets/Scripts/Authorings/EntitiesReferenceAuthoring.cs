using Unity.Entities;
using UnityEngine;

namespace DefaultNamespace
{
    public class EntitiesReferenceAuthoring : MonoBehaviour
    {
        public GameObject CharacterPrefab;
        
        private class EntitiesReferenceAuthoringBaker : Baker<EntitiesReferenceAuthoring>
        {
            public override void Bake(EntitiesReferenceAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new EntitiesReferences
                {
                    CharacterPrefabEntity = GetEntity(authoring.CharacterPrefab, TransformUsageFlags.Dynamic),
                });
            }
        }
    }
}