using UnityEngine;

namespace VectorFlow.Gameplay.Blocks
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class PrismBlock : MonoBehaviour, ILaserInteractable
    {
        [Tooltip("Lazerin bölüneceği GameObject (LaserEmitter içeren) Prefab'i")]
        public GameObject laserEmitterPrefab;

        public bool OnLaserHit(Vector2 hitPoint, Vector2 incomingDirection, LaserEmitter laserEmitter, out Vector2 outgoingDirection)
        {
            outgoingDirection = Vector2.zero;

            if (laserEmitterPrefab != null)
            {
                SpawnSubLaser(transform.right);
                SpawnSubLaser(-transform.right);
            }
            else
            {
                Debug.LogWarning("PrismBlock: laserEmitterPrefab atanmamış!");
            }

            return false; 
        }

        private void SpawnSubLaser(Vector2 direction)
        {
            GameObject newLaserObj = Instantiate(laserEmitterPrefab, transform.position, Quaternion.identity);
            newLaserObj.transform.up = direction; 

            // Alt lazerin Arrow görseli görünmesin (üst üste binmeyi engeller)
            SpriteRenderer sr = newLaserObj.GetComponent<SpriteRenderer>();
            if (sr != null) sr.enabled = false;

            LaserEmitter newEmitter = newLaserObj.GetComponent<LaserEmitter>();
            if (newEmitter != null)
            {
                newEmitter.isActivated = true;
                // Kendisini doğuran prizmanın collider'ını yok saymasını söylüyoruz (Sonsuz döngüyü engeller)
                newEmitter.ShootLaser(GetComponent<Collider2D>());
            }
        }
    }
}
