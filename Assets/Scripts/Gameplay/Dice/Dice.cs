using System;
using System.Collections.Generic;
using System.Text;
using Camera;
using Cysharp.Threading.Tasks;
using Gameplay.Cards;
using Gameplay.Interaction;
using Scriptable;
using Simulation;
using TMPro;
using UI.Interaction;
using UnityEngine;
using Util.Interaction;
using Random = UnityEngine.Random;

namespace Gameplay.Dice
{
    public class Dice : MonoBehaviour, IDice, IInteractableOnDrag, IInteractableOnClick
    {
        [SerializeField] protected new Rigidbody rigidbody;
        [SerializeField] private TMP_Text[] texts = new TMP_Text[6];

        private bool dead;
        private bool isReplaying;
        private static readonly TimeSpan delay = TimeSpan.FromMilliseconds(1000 / 60f);

        

        private void OnEnable()
        {
            GameTable.OnDoubleClick += OnTableDoubleClick;
            Card.OnDoubleClick += OnTableDoubleClick;
        }

        private void OnDisable()
        {
            GameTable.OnDoubleClick -= OnTableDoubleClick;
            Card.OnDoubleClick -= OnTableDoubleClick;
        }

        private void OnDestroy()
        {
            dead = true;
            OnDisable();
            OnDestroyed?.Invoke(this);
        }

        private void OnTableDoubleClick(Vector3 pos)
        {
            if(isReplaying || rigidbody.velocity.sqrMagnitude >= 30 || rigidbody.transform.position.y > 2f) return;

            Vector3 direction = (transform.position - pos).normalized;
            direction.y = 0.55f;
            rigidbody.AddForce(direction * 2.5f, ForceMode.Impulse);
            rigidbody.AddTorque(new Vector3(direction.z, direction.y, -direction.x), ForceMode.Impulse);
        }

        public async UniTask ReplayAsync(DiceRollReplay replay)
        {
            isReplaying = true;
            rigidbody.useGravity = false;
            rigidbody.constraints = RigidbodyConstraints.FreezeAll;

            for (int frame = 0; frame < replay.Length; frame++)
            {
                transform.position = replay.Positions[frame];
                transform.rotation = replay.Rotations[frame];
                await UniTask.Delay(delay * Time.deltaTime);
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
        public bool Dead => dead;
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

        public void GetInteractionTargets(List<IInteractable> targets) { }


        // IInteractableOnClick
        public Vector4 OutlineColor => Vector4.zero;
        public InteractableOutline InteractableOutline => null;
        public event IInteractable.InteractableEvent OnDestroyed;
        public event IInteractable.InteractableEvent OnInitialized;
        public bool CanClick => CanInteract;
        public virtual void OnClick(InteractionResult result, int clickCount)
        {
            if(rigidbody.transform.position.y > 1.3f) return;

            rigidbody.AddForce(Vector3.up * 2, ForceMode.Impulse);
            rigidbody.AddTorque(Random.insideUnitSphere.normalized);
        }
    }
}