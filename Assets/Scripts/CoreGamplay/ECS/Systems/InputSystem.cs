using Leopotam.Ecs;
using UnityEngine;

namespace CoreGameplay
{

    sealed class InputSystem : IEcsRunSystem
    {
        readonly EcsWorld _world = null;
        EcsFilter<BallComponent> _filter = null;
        private SO.InputSettingsSO inputSettingsSO;
        private ICommand InputHandler;
        private float movementForce;

        public InputSystem(SO.InputSettingsSO inputSettingsSO)
        {
            this.inputSettingsSO = inputSettingsSO;
            switch (inputSettingsSO.type) 
            {
                case SO.InputSettingsSO.InputType.WASD: InputHandler = new WASDCommand(inputSettingsSO.SettingsWASD);break;
                case SO.InputSettingsSO.InputType.Gyroscope: InputHandler = new GyroCommand(inputSettingsSO.SettingsGyroscope);break;
                case SO.InputSettingsSO.InputType.JoystickNotImplemented: Debug.LogError("JOYSTICK NOT IMPLEMENTED"); break;
            }
            movementForce = InputHandler.MovementForce;
        }
        


        public void Run()
        {
            //Rigidbody firstBall = null;
            var direction = InputHandler.MoveInput();
            //Debug.Log($"####### direction = {direction}");


            if(direction.magnitude>0.15f)
            for (int firstIndex = 0; firstIndex < _filter.GetEntitiesCount(); firstIndex++)
            {
                ref var Ball = ref _filter.Get1(firstIndex);

                //Debug.Log("force = " + direction * movementForce);

                //Ball.Forces.Add(direction * movementForce * 30);
                //Ball.Rigidbody.AddForce(direction * movementForce);
                Ball.Rigidbody.MovePosition(Ball.Rigidbody.position + direction * movementForce);
                //Ball.Rigidbody.position += direction * movementForce;
            }
        }
        #region Command Variant WASD or Gyroscope
        interface ICommand
        {
            float MovementForce { get; set; }
            Vector3 MoveInput();
        }
        public class GyroCommand : ICommand
        {
            private Vector3 direction;

            private float _movementForce;
            public float MovementForce { get => _movementForce; set => _movementForce = value; }
            public GyroCommand(SO.InputSettingsSO.GyroscopeSettings settings)
            {
                _movementForce = settings.ForceMovement;
            }            

            public Vector3 MoveInput()
            {

                //direction = new Vector3(Input.acceleration.x, Input.acceleration.z, Input.acceleration.y);
                //direction -= Vector3.up * direction.y;
                //For Landscape

                direction = new Vector3(-Input.acceleration.y, 0 , Input.acceleration.x);

                return direction;
                //return Camera.main.transform.rotation * direction;
            }
        }
        public class WASDCommand : ICommand
        {
            private Vector3 offset;
            private float x, y;
            private float sens;

            private float _movementForce;
            public float MovementForce { get => _movementForce; set => _movementForce = value; }
            public WASDCommand(SO.InputSettingsSO.WASDSettings settings) 
            {
                sens = settings.sens; 
                _movementForce = settings.ForceMovement; 
            }
            public Vector3 MoveInput()
            {
                offset = Vector3.zero;
                x = 0; y = 0;

                if(Input.GetKey(KeyCode.W)) y += sens;
                if(Input.GetKey(KeyCode.A)) x -= sens;
                if(Input.GetKey(KeyCode.S)) y -= sens;
                if(Input.GetKey(KeyCode.D)) x += sens;

                return Vector3.Normalize(Camera.main.transform.up * y + Camera.main.transform.right * x);
            }
        }
        #endregion
    }
}