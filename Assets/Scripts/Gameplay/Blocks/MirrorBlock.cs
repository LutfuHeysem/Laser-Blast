using UnityEngine;

namespace VectorFlow.Gameplay.Blocks
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class MirrorBlock : MonoBehaviour, ILaserInteractable
    {
        public bool OnLaserHit(Vector2 hitPoint, Vector2 incomingDirection, LaserEmitter laserEmitter, out Vector2 outgoingDirection)
        {
            RaycastHit2D hit = Physics2D.Raycast(hitPoint - incomingDirection * 0.1f, incomingDirection, 0.5f);
            
            if (hit.collider != null)
            {
                outgoingDirection = Vector2.Reflect(incomingDirection, hit.normal);
                return true; 
            }
            
            outgoingDirection = -incomingDirection;
            return true;
        }
    }
}
