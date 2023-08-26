using System.Collections.Generic;
using Gameplay.Dice;
using UnityEngine;

namespace Simulation
{
    public class DiceRollReplay
    {
        public int Length { get; private set; } = 0;
        public List<Vector3> Positions { get; } = new();
        public List<Quaternion> Rotations { get; } = new();
        public int Result { get; private set; }

        public void Stamp(Rigidbody rigidbody)
        {
            Length++;
            Positions.Add(rigidbody.position);
            Rotations.Add(rigidbody.rotation);
        }

        public void GetResult(IDice cube)
        {
            Result = cube.GetTopSide();
        }
    }
}