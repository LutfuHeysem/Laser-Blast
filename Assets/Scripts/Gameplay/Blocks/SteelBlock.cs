using UnityEngine;

namespace VectorFlow.Gameplay.Blocks
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class SteelBlock : MonoBehaviour, ILaserInteractable
    {
        public bool OnLaserHit(Vector2 hitPoint, Vector2 incomingDirection, LaserEmitter laserEmitter, out Vector2 outgoingDirection)
        {
            outgoingDirection = Vector2.zero;
            return false; 
        }
    }
}
