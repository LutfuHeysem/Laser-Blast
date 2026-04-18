using UnityEngine;
using VectorFlow.Managers;

namespace VectorFlow.Gameplay.Blocks
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class GoalBlock : MonoBehaviour, ILaserInteractable
    {
        public bool OnLaserHit(Vector2 hitPoint, Vector2 incomingDirection, LaserEmitter laserEmitter, out Vector2 outgoingDirection)
        {
            outgoingDirection = Vector2.zero;
            
            if (VectorFlow.Managers.GameManager.Instance != null)
            {
                VectorFlow.Managers.GameManager.Instance.WinLevel();
            }
            
            return false; 
        }
    }
}
