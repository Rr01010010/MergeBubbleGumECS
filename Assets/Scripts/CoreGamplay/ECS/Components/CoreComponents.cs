using UnityEngine;

namespace CoreGameplay 
{
    struct BallComponent
    {
        public Rigidbody Rigidbody;
        public System.Collections.Generic.List<Vector3> Forces;
    }
    struct CollisionEventComponent 
    {
        public enum TypeOfCollision { None, PlayerEnteredFinish }
        public TypeOfCollision CollisionType;
        public Collider Object;
    }
    struct PostgameMessageComponent 
    {
        public string Message;
        public string Scores;
    }
}