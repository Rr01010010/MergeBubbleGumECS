using Leopotam.Ecs;
using UnityEngine;

namespace CoreGameplay
{
    public class EndGame : MonoBehaviour, IEcsInitSystem,IEcsRunSystem
    {
        //THIS IS NOT THE BEST PRACTICE, IF THE PROJECT WILL BE REFINED IN THE FUTURE, 
        //THEN IS NECESSARY TO CHANGE THE LOGIC OF THIS CLASS, 
        //DESIRABLY UNLINKING IT FROM ECS AS A INIT SYSTEM.

        [Header("UI Serialization")]
        [SerializeField] GameObject StartLayer;
        [SerializeField] TMPro.TMP_Text messageLabel;
        [SerializeField] TMPro.TMP_Text scoreLabel;

        readonly EcsWorld _world = null;
        EcsFilter<PostgameMessageComponent> _filter = null;
        EcsFilter<BallComponent> _filterForClose;
        public void Init()
        {
            _endCalculated = false;
        }


        public void OnCloseGame()
        {
            Application.Quit();
        }

        public void OnReturnToMenu() 
        {
            EcsStartup.State = EcsStartup.StateType.Init;

            _endCalculated = false;

            StartLayer.SetActive(true);

            gameObject.SetActive(false);
        }

        private bool _endCalculated = false;//костыль :(
        public void Run()
        {
            if (!_endCalculated) 
            {
                _endCalculated = true;
                //var filter = _world.GetFilter(typeof(BallComponent));            
                //if(filter!=null)
                foreach (int index in _filterForClose)
                {
                    GameObject.Destroy(_filterForClose.Get1(index).Rigidbody.gameObject);
                    _filterForClose.GetEntity(index).Destroy();
                }

                foreach (var index in _filter)
                {
                    ref var messageComponent = ref _filter.Get1(index);

                    messageLabel.text = messageComponent.Message;
                    scoreLabel.text = messageComponent.Scores;
                }
                gameObject.SetActive(true);
            }
            
        }
    }
}