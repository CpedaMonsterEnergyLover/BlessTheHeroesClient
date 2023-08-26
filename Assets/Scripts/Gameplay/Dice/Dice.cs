using System.Text;
using Camera;
using Cysharp.Threading.Tasks;
using Gameplay.Interaction;
using Scriptable;
using Simulation;
using TMPro;
using UI.Interaction;
using UnityEngine;

namespace Gameplay.Dice
{
    public class Dice : MonoBehaviour, IDice, IInteractableOnDrag, IInteractableOnClick
    {
        [SerializeField] protected new Rigidbody rigidbody;
        [SerializeField] private TMP_Text[] texts = new TMP_Text[6];

        private bool isReplaying;
        

        
        public async UniTask ReplayAsync(DiceRollReplay replay)
        {
            isReplaying = true;
            rigidbody.useGravity = false;
            rigidbody.constraints = RigidbodyConstraints.FreezeAll;

            for (int frame = 0; frame < replay.Length; frame++)
            {
                transform.position = replay.Positions[frame];
                transform.rotation = replay.Rotations[frame];
                await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
            }

            rigidbody.constraints = RigidbodyConstraints.None;
            rigidbody.useGravity = true;
            isReplaying = false;
        }

        public void Repaint(DiceSet diceSet, int index, int offset)
        {
            if (offset < 0) offset += 6;
            string[] strings = diceSet.GetDiceSideStrings(index);
            for (int side = 0; side < 6; side++) 
                texts[side].SetText(strings[(side + offset) % 6]);
        }

        public static void PrintDiceRollResult(int[] result)
        {
            StringBuilder sb = new StringBuilder();
            foreach (int res in result) 
                sb.Append(res).Append(" ");
            sb.Remove(sb.Length - 1, 1);
            Debug.Log($"Dice roll result. Result: [{sb}]");
        }

        
        
        // IDice
        public Rigidbody Rigidbody => rigidbody;
        public Transform GetSide(int i) => texts[i].transform;
        
        
        // IInteractableOnDrag
        public bool CanInteract => !isReplaying;
        public void OnDragStart(InteractionResult result)
        {
            rigidbody.useGravity = false;
        }

        public void OnDragEnd(InteractionResult target)
        {
            rigidbody.useGravity = true;
        }

        public InteractionTooltipData OnDrag(InteractionResult target)
        {
            Ray ray = MainCamera.Camera.ScreenPointToRay(MainCamera.Instance.GetMousePosition());
            if (ray.direction.y == 0) return null;
            
            float distance = (1.3f - ray.origin.y) / ray.direction.y;
            Vector3 point = ray.GetPoint(distance);
            Vector3 direction = (point - rigidbody.position) * 2.5f;
            rigidbody.velocity = direction;
            
            rigidbody.angularVelocity = new Vector3(
                direction.z,
                direction.y,
                -direction.x
            );
            return null;
        }

        
        // IInteractableOnClick
        public bool CanClick => CanInteract;
        public virtual void OnClick(InteractionResult result)
        {
            if(rigidbody.transform.position.y > 1.3f) return;

            rigidbody.AddForce(Vector3.up * 2, ForceMode.Impulse);
            rigidbody.AddTorque(Random.insideUnitSphere.normalized);
        }
    }
}