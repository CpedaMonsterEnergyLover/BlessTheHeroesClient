using Gameplay.Dice;
using UnityEngine;

namespace Gameplay.Simulation
{
    [RequireComponent(typeof(Rigidbody))]
    public class SimulationCube : MonoBehaviour, IDice
    {
        [SerializeField] private Transform[] sides = new Transform[6];
        [SerializeField] private new Rigidbody rigidbody;

        public Dice.Dice DiceReference { get; set; }
        

        
        public void ActualizePosition()
        {
            if(DiceReference is null) return;
            Rigidbody.position = DiceReference.Rigidbody.position;
            Rigidbody.rotation = DiceReference.Rigidbody.rotation;
        }
        
        
        // IDice
        public Rigidbody Rigidbody => rigidbody;
        public Transform GetSide(int i) => sides[i];
    }
}