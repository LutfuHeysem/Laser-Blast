using UnityEngine;

namespace VectorFlow.Gameplay.Blocks
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class TNTBlock : MonoBehaviour, ILaserInteractable
    {
        [Tooltip("Patlama yarıçapı")]
        public float explosionRadius = 1.5f;

        public bool OnLaserHit(Vector2 hitPoint, Vector2 incomingDirection, LaserEmitter laserEmitter, out Vector2 outgoingDirection)
        {
            outgoingDirection = Vector2.zero;
            Explode();
            return false; 
        }

        public void Explode()
        {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
            
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.gameObject == this.gameObject) continue;

                TNTBlock otherTNT = hitCollider.GetComponent<TNTBlock>();
                if (otherTNT != null)
                {
                    otherTNT.Explode();
                }
                
                if (hitCollider.GetComponent<GoalBlock>() == null && hitCollider.GetComponent<LaserEmitter>() == null)
                {
                    Destroy(hitCollider.gameObject);
                }
            }

            Destroy(gameObject); 
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }
}
