using UnityEngine;
using System.Collections.Generic;

namespace VectorFlow.Gameplay.Blocks
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class PrismBlock : MonoBehaviour, ILaserInteractable
    {
        [Header("Ayarlar")]
        public GameObject laserEmitterPrefab;

        [Tooltip("Lazerin çıkacağı 4 boş GameObject'i buraya sürükleyin.")]
        public List<Transform> exitPoints;

        public bool OnLaserHit(Vector2 hitPoint, Vector2 incomingDirection, LaserEmitter laserEmitter, out Vector2 outgoingDirection)
        {
            outgoingDirection = Vector2.zero;

            if (laserEmitterPrefab != null && exitPoints != null)
            {
                foreach (Transform exitPoint in exitPoints)
                {
                    //geldiği yöne en yakın olan çıkışı sayma
                    if (Vector2.Distance(hitPoint, exitPoint.position) > 0.2f)
                    {
                        SpawnSubLaser(exitPoint);
                    }
                }
            }
            else
            {
                Debug.LogWarning("PrismBlock: Prefab veya Exit Points eksik!");
            }

            return false; // Ana lazeri burada sonlandırıyoruz.
        }

        private void SpawnSubLaser(Transform exitTransform)
        {
            // 1. Obveyi oluştur
            GameObject newLaserObj = Instantiate(laserEmitterPrefab, exitTransform.position, exitTransform.rotation);

            // 2. Yönünü ayarla (Bunu offset hesabından ÖNCE yapmalıyız ki startingPoint doğru yöne baksın)
            newLaserObj.transform.right = exitTransform.right;

            LaserEmitter newEmitter = newLaserObj.GetComponent<LaserEmitter>();
            if (newEmitter != null)
            {
                // --- SİHİRLİ DOKUNUŞ BURASI ---
                // Eğer startingPoint silah namlusu için ileri alınmışsa, aradaki farkı (offset) buluyoruz.
                if (newEmitter.startingPoint != null)
                {
                    // startingPoint'in dünya üzerindeki yeri ile ana objenin yeri arasındaki fark
                    Vector3 offset = newEmitter.startingPoint.position - newLaserObj.transform.position;

                    // Ana objeyi bu fark kadar geriye çekiyoruz. 
                    // Böylece startingPoint tam olarak 'exitTransform.position' noktasına oturuyor!
                    newLaserObj.transform.position -= offset;
                }
                // -------------------------------

                // Z eksenini 2D oyunlar için güvenliğe al (Kopukluğu önlemek için)
                Vector3 fixedPos = newLaserObj.transform.position;
                fixedPos.z = 0f;
                newLaserObj.transform.position = fixedPos;

                // Görsel oku kapat (çünkü bu sadece prizmadan çıkan bir ışın, silah değil)
                SpriteRenderer sr = newLaserObj.GetComponent<SpriteRenderer>();
                if (sr != null) sr.enabled = false;

                // FİZİKSEL ÇAKIŞMAYI ÖNLEME:
                // Üst üste binen görünmez okların collider'ları birbirini engellemesin diye siliyoruz.
                Collider2D coll = newLaserObj.GetComponent<Collider2D>();
                if (coll != null) Destroy(coll);

                newEmitter.isActivated = true;
                // Kendi collider'ını (prizma) yoksay
                newEmitter.ShootLaser(GetComponent<Collider2D>());
            }
        }
    }
}