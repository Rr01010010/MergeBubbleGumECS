using Leopotam.Ecs;
using System.Collections.Generic;
using UnityEngine;

//using UnityEngine;

namespace CoreGameplay
{

    public class FinishSystem : IEcsRunSystem, IEcsInitSystem
    {
        readonly EcsWorld _world = null;
        EcsFilter<CollisionEventComponent> _filter = null;

        public FinishSystem(int count) { TargetCount = count; }

        private int TargetCount;
        private int counted;

        public void Init() { counted = 0; }

        public void Run()
        {
            foreach(var index in _filter)//for (int firstIndex = 0; firstIndex < _filter.GetEntitiesCount(); firstIndex++)
            {
                ref var collisionEvent = ref _filter.Get1(index);
                if(collisionEvent.CollisionType.Equals(CollisionEventComponent.TypeOfCollision.PlayerEnteredFinish))
                {
                    Debug.Log("Player entered to finish");

                    GameObject.Destroy(collisionEvent.Object.gameObject);
                    _filter.GetEntity(index).Destroy();

                    counted++;

                    if (counted == TargetCount)
                    {
                        ref var message = ref _world.NewEntity().Get<PostgameMessageComponent>();

                        message.Message = $"Wow! You are a super cool!";
                        message.Scores = $"taken all {counted} gums!";

                        EcsStartup.State = EcsStartup.StateType.End;
                    }
                }
            }
        }
    }
}