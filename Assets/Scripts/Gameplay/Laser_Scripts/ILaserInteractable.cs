using UnityEngine;

public interface ILaserInteractable 
{
    /// <summary>
    /// Called when the laser hits this object.
    /// </summary>
    /// <param name="hitPoint">The point in world space where the raycast hit.</param>
    /// <param name="incomingDirection">The direction the laser was traveling.</param>
    /// <param name="outgoingDirection">The new direction the laser should travel if it continues.</param>
    /// <returns>True if the laser should CONTINUE, False if the laser should STOP.</returns>
    bool OnLaserHit(Vector2 hitPoint, Vector2 incomingDirection, LaserEmitter laserEmitter, out Vector2 outgoingDirection);
}
