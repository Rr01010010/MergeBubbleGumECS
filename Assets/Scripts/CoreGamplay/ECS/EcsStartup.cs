using Leopotam.Ecs;
using UnityEngine;

namespace CoreGameplay
{
    sealed class EcsStartup : MonoBehaviour
    {
        public enum StateType { Init, Gameplay, End }
        public static StateType State;


        public static string ECSStartUpTag = "ECS StartUp";
        public static string PlayerTag = "Player";

        public EcsWorld _world;
        EcsSystems _gameplaySystems;
        EcsSystems _endSystem;
        [SerializeField] SO.BallSettingsSO ballSettingsSO;
        [SerializeField] SO.InputSettingsSO inputSettingsSO;

        [SerializeField] FinishTrigger FinishTrigger;
        [SerializeField] EndGame endGameSystem;
        [SerializeField] GameObject StartLayer;

        void Start()
        {
            State = StateType.Init;
            // void can be switched to IEnumerator for support coroutines.
            
            _world = new EcsWorld();

            _endSystem = new EcsSystems(_world);

            #if UNITY_EDITOR
            Leopotam.Ecs.UnityIntegration.EcsWorldObserver.Create(_world);
            Leopotam.Ecs.UnityIntegration.EcsSystemsObserver.Create(_endSystem);
            #endif

            _endSystem
                .Add(endGameSystem)
                .OneFrame<PostgameMessageComponent>()
                .Init();
        }
        private void SystemsInit()
        {
            if (_gameplaySystems != null) 
            {
                _gameplaySystems.Destroy(); 
                _gameplaySystems = null; 
            }

                _gameplaySystems = new EcsSystems(_world);

            #if UNITY_EDITOR
            Leopotam.Ecs.UnityIntegration.EcsSystemsObserver.Create(_gameplaySystems);
            #endif

            _gameplaySystems
                .Add(new BallsAttractionSystem(ballSettingsSO))
                .Add(new InputSystem(inputSettingsSO))
                .Add(FinishTrigger)
                .Add(new FinishSystem(ballSettingsSO.positions.Count))
                .Add(new BallsDistanceSystem(ballSettingsSO))
                
                // inject service instances here (order doesn't important), for example:
                // .Inject (new CameraService ())
                // .Inject (new NavMeshSupport ())
                .Init();
        }




        void FixedUpdate()
        {
            switch (State)
            {
                case StateType.Init: break;
                case StateType.Gameplay: _gameplaySystems?.Run(); break;
                case StateType.End: _endSystem?.Run(); break;
            }
        }

        void OnDestroy()
        {
            if (_gameplaySystems != null)
            {
                _gameplaySystems.Destroy();
                _gameplaySystems = null;
                _world.Destroy();
                _world = null;
            }
        }


        public void OnStartGame()
        {
            StartLayer.SetActive(false);

            State = StateType.Init;

            SystemsInit();

            State = StateType.Gameplay;
        }

    }
}