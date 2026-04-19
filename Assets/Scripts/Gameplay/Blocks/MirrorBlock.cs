using UnityEngine;

namespace VectorFlow.Gameplay.Blocks
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class MirrorBlock : MonoBehaviour, ILaserInteractable
    {
        [Header("Grid Snapping Settings")]
        [Tooltip("Check this if your game uses diagonal lasers/mirrors. Uncheck if it's strictly Up/Down/Left/Right.")]
        public bool allowDiagonals = false;

        public bool OnLaserHit(Vector2 hitPoint, Vector2 incomingDirection, LaserEmitter laserEmitter, out Vector2 outgoingDirection)
        {
            // 1. Snap the incoming direction just to be absolutely safe
            incomingDirection = SnapDirection(incomingDirection);

            RaycastHit2D hit = Physics2D.Raycast(hitPoint - incomingDirection * 0.1f, incomingDirection, 0.5f);

            if (hit.collider != null)
            {
                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.audioPlay("mirror");
                }

                if (VectorFlow.Managers.ScoreManager.Instance != null)
                {
                    VectorFlow.Managers.ScoreManager.Instance.AddScore(10); // Aynadan sektirme bonusu
                }

                // 2. Snap the physical hit normal so Unity's corner-collision errors are erased
                Vector2 perfectNormal = SnapDirection(hit.normal);

                // 3. Reflect the laser using the perfect grid-aligned normal
                Vector2 rawOutgoing = Vector2.Reflect(incomingDirection, perfectNormal);

                // 4. Snap the final result to guarantee it stays strictly on your grid axes
                outgoingDirection = SnapDirection(rawOutgoing);

                return true;
            }

            // If the raycast misses internally, send it straight back
            outgoingDirection = SnapDirection(-incomingDirection);
            return true;
        }

        /// <summary>
        /// Forces a Vector2 to point exactly in 90-degree or 45-degree increments.
        /// </summary>
        private Vector2 SnapDirection(Vector2 dir)
        {
            // Convert direction to an angle in degrees
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            // Determine our snapping step (90 degrees for 4-way, 45 degrees for 8-way)
            float step = allowDiagonals ? 45f : 90f;

            // Round the angle to the nearest step
            float snappedAngle = Mathf.Round(angle / step) * step;

            // Convert the perfectly rounded angle back into a Vector2 direction
            float rad = snappedAngle * Mathf.Deg2Rad;
            Vector2 snappedDir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

            // Eliminate microscopic floating point leftovers (e.g., turning 0.000000114 into 0)
            snappedDir.x = Mathf.Round(snappedDir.x * 1000f) / 1000f;
            snappedDir.y = Mathf.Round(snappedDir.y * 1000f) / 1000f;

            return snappedDir.normalized;
        }
    }
}