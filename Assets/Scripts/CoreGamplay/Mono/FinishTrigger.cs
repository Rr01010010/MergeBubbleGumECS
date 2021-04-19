using Leopotam.Ecs;
using UnityEngine;

namespace CoreGameplay
{
    public class FinishTrigger : MonoBehaviour, IEcsInitSystem
    {
        //THIS IS NOT THE BEST PRACTICE, IF THE PROJECT WILL BE REFINED IN THE FUTURE, 
        //THEN IS NECESSARY TO CHANGE THE LOGIC OF THIS CLASS, 
        //DESIRABLY UNLINKING IT FROM ECS AS A INIT SYSTEM.

        [SerializeField] Collider Trigger;
        
        void Start()
        {
            if (Trigger == null)
                if (!transform.TryGetComponent<Collider>(out Trigger))
                    Debug.LogError("Finish Trigger haven't collider component");

            //[SerializeField] CoreGameplay.EcsStartup _ecsStartUp;
            //if (_ecsStartUp == null)
            //{
            //    var goCS = GameObject.FindGameObjectWithTag(CoreGameplay.EcsStartup.ECSStartUpTag);
            //    if (goCS == null)
            //        Debug.LogError("Finish Trigger cannot found ECS StartUp object with EcsStartUp component");
            //
            //    if (!goCS.TryGetComponent<CoreGameplay.EcsStartup>(out _ecsStartUp))
            //        Debug.LogError("Finish Trigger cannot found ECS StartUp object with EcsStartUp component");
            //}
            // THIS CODE can be helpful when the class is unlinked from the init system

            Trigger.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag.Equals(CoreGameplay.EcsStartup.PlayerTag))
            {
                foreach(var i in _filter) 
                {
                    if(_filter.Get1(i).Rigidbody.gameObject.Equals(other.gameObject) &&
                        _filter.Get1(i).Rigidbody.position.Equals(other.transform.position)) 
                    {

                        ref var colllisionData = ref _filter.GetEntity(i).Get<CoreGameplay.CollisionEventComponent>();
                        colllisionData.Object = other;
                        colllisionData.CollisionType = CoreGameplay.CollisionEventComponent.TypeOfCollision.PlayerEnteredFinish;
                    }
                }

            }


        }

        readonly EcsWorld _world = null;
        EcsFilter<BallComponent> _filter = null;
        public void Init()
        {

        }
    }
}