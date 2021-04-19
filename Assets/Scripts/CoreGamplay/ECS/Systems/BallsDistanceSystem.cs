using Leopotam.Ecs;
using System.Collections.Generic;
using UnityEngine;

namespace CoreGameplay
{

    public class BallsDistanceSystem : IEcsRunSystem, IEcsInitSystem
    {
        readonly EcsWorld _world = null;
        EcsFilter<BallComponent> _filter = null;
        private SO.BallSettingsSO ballSettingsSO;
        private float borderDistance;
        private BallComponent firstBall, secondBall;

        public BallsDistanceSystem(SO.BallSettingsSO ballSettingsSO)
        {
            this.ballSettingsSO = ballSettingsSO;
            wasCountedBalls = ballSettingsSO.positions.Count;
        }
        int wasCountedBalls;

        public void Init()
        {
            borderDistance = (ballSettingsSO.BallRigidbodyPrefab.transform.localScale.magnitude / Mathf.Sqrt(3.0f)) * 1.1f;
        }
        public void Run()
        {

            for (int firstIndex = 0; firstIndex < _filter.GetEntitiesCount(); firstIndex++)
            {
                firstBall = _filter.Get1(firstIndex);

                for (int secondIndex = firstIndex + 1; secondIndex < _filter.GetEntitiesCount(); secondIndex++)
                {
                    secondBall = _filter.Get1(secondIndex);

                    var distance = Vector3.Distance(secondBall.Rigidbody.position, firstBall.Rigidbody.position);


                    if (distance <= borderDistance) 
                    {
                        int scores = wasCountedBalls - _filter.GetEntitiesCount();

                        ref var message = ref _world.NewEntity().Get<PostgameMessageComponent>();

                        message.Message = $"You lose, lucky next time :)";                         
                        message.Scores = scores < 2 ? $"taken {scores} gum" : $"taken {scores} gums";

                        EcsStartup.State = EcsStartup.StateType.End;
                    }
                }

            }
        }
    }
}