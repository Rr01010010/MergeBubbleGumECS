using Leopotam.Ecs;
using System.Collections.Generic;
using UnityEngine;

namespace CoreGameplay 
{
    
    public class BallsAttractionSystem : IEcsRunSystem, IEcsInitSystem        
    {
        readonly EcsWorld _world = null;
        EcsFilter<BallComponent> _filter = null;
        private SO.BallSettingsSO ballSettingsSO;

        public float UpperDistanceBorder;
        public float LowerDistanceBorder;
        public float attractionForce;
        public int mask;
        private Rigidbody firstRigid = null, secondRigid = null;
        public BallsAttractionSystem(SO.BallSettingsSO ballSettingsSO)
        {
            this.ballSettingsSO = ballSettingsSO;

            #if UNITY_EDITOR
            ballSettingsSO.ballsSystem = this;
            #endif

            UpperDistanceBorder = ballSettingsSO.physicalParameters.UpperDistanceBorder;
            LowerDistanceBorder = ballSettingsSO.physicalParameters.LowerDistanceBorder;
            attractionForce = ballSettingsSO.physicalParameters.attractionForce;
            mask = ballSettingsSO.physicalParameters.RaycastMask;
        }

        public void Init()
        {
            var parent = new GameObject();
            parent.name = "BallsParent";
            foreach (var pos in ballSettingsSO.positions)
            {
                var ball = _world.NewEntity();
                var component = ball.Get<BallComponent>();//проблема в том что структуру нельзя изменить

                ball.Get<BallComponent>().Rigidbody = GameObject.Instantiate(ballSettingsSO.BallRigidbodyPrefab, pos, Quaternion.identity, parent.transform);
                ball.Get<BallComponent>().Forces = new List<Vector3>();

            }
        }

        public void Run () 
        {


            for (int firstIndex = 0; firstIndex < _filter.GetEntitiesCount(); firstIndex++) 
            {
                var firstBall = _filter.Get1(firstIndex);
                firstRigid = firstBall.Rigidbody;

                for (int secondIndex = firstIndex+1; secondIndex < _filter.GetEntitiesCount(); secondIndex++) 
                {
                    var secondBall = _filter.Get1(secondIndex);
                    secondRigid = secondBall.Rigidbody;
                    Vector3 direction = secondRigid.position - firstRigid.position;

                    if (!Physics.Raycast(firstRigid.position, direction, direction.magnitude, mask)) 
                    {
                        var distance = direction.magnitude;

                        if (UpperDistanceBorder < distance) distance = UpperDistanceBorder;
                        else if (LowerDistanceBorder > distance) distance = LowerDistanceBorder;
                        var force = attractionForce * secondRigid.mass * firstRigid.mass / (distance * distance);

                        //Debug.Log($"force = {force}");

                        firstBall.Forces.Add(Vector3.Normalize(direction) * force);
                        secondBall.Forces.Add(Vector3.Normalize(-direction) * force);

                        Debug.DrawLine(firstRigid.position, secondRigid.position, Color.cyan);
                    }
                    
                }

                if (firstBall.Forces.Count > 0)
                    for (int i = firstBall.Forces.Count - 1; i >= 0; i--)
                    {
                        firstRigid.AddForce(firstBall.Forces[i]);
                        firstBall.Forces.RemoveAt(i);
                    }

                else firstBall.Rigidbody.velocity = Vector3.zero;
                
            }            
        }
    }
}