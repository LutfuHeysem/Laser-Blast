using UnityEngine;

namespace VectorFlow.Gameplay.Blocks
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class GlassBlock : MonoBehaviour, ILaserInteractable
    {
        // public SoundManager sm; sat�r�n� KALDIRDIK.

        public bool OnLaserHit(Vector2 hitPoint, Vector2 incomingDirection, LaserEmitter laserEmitter, out Vector2 outgoingDirection)
        {
            outgoingDirection = incomingDirection;

            if (VectorFlow.Managers.ScoreManager.Instance != null)
            {
                VectorFlow.Managers.ScoreManager.Instance.AddScore(100, transform.position);
            }

            // SoundManager'a Singleton (Instance) �zerinden direkt eri�iyoruz.
            // Hata almamak i�in sahnede SoundManager oldu�undan emin olmak ad�na null kontrol� yapmak iyi bir pratiktir.
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.audioPlay("break");
            }

            Destroy(gameObject);
            return true;
        }
    }
}