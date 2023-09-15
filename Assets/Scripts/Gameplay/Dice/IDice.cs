using UnityEngine;

namespace Gameplay.Dice
{
    public interface IDice
    {
        public Rigidbody Rigidbody { get; }
        public Transform GetSide(int i);


        
        public void Throw(Vector3 force, Vector3 torque)
        {
            Rigidbody.useGravity = true;
            Rigidbody.constraints = RigidbodyConstraints.None;
            Rigidbody.velocity = Vector3.zero;
            Rigidbody.angularVelocity = Vector3.zero;
            Rigidbody.AddForce(force, ForceMode.Impulse);
            Rigidbody.AddTorque(torque, ForceMode.Impulse);
        }
        
        public int GetTopSide()
        {
            float maxY = float.MinValue;
            int max = -1;
            for (var i = 0; i < 6; i++)
            {
                float y = -GetSide(i).forward.y;
                if (!(y > maxY)) continue;
                maxY = y;
                max = i;
            }

            return max;
        }
    }
}