using UnityEngine;

namespace VectorFlow.Gameplay.Blocks
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class GlassBlock : MonoBehaviour, ILaserInteractable
    {
        public bool OnLaserHit(Vector2 hitPoint, Vector2 incomingDirection, LaserEmitter laserEmitter, out Vector2 outgoingDirection)
        {
            outgoingDirection = incomingDirection;
            if (VectorFlow.Managers.ScoreManager.Instance != null)
            {
                VectorFlow.Managers.ScoreManager.Instance.AddScore(100);
            }
            Destroy(gameObject);
            return true; 
        }
    }
}
