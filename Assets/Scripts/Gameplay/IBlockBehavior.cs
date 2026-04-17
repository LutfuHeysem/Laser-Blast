using UnityEngine;
using VectorFlow.Managers;

namespace VectorFlow.Gameplay
{
    public interface IBlockBehavior
    {
        void OnBeamHit(Beam beam);
    }
}
