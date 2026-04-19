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
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.audioPlay("explode");
            }
            Explode();
            return false; 
        }

        public void Explode()
        {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
            
            if (VectorFlow.Managers.ScoreManager.Instance != null)
            {
                VectorFlow.Managers.ScoreManager.Instance.AddScore(150); // TNT Patlama Puanı
            }

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
                    if (VectorFlow.Managers.ScoreManager.Instance != null && hitCollider.GetComponent<TNTBlock>() == null)
                    {
                        VectorFlow.Managers.ScoreManager.Instance.AddScore(50); // Etraftaki objeleri yok etme bonusu
                    }
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
