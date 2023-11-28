using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gameplay.GameField;
using Gameplay.Tokens;
using UnityEngine;

namespace Util.Tokens
{
    public class TokenLayout : MonoBehaviour
    {
        [SerializeField] private int width;
        [SerializeField] private float tokenSize;
        [SerializeField] private CardSide cardSide;
        [SerializeField] private Card card;

        private float fullRowOffset;
        private float halfTokenSize;
        private bool playingAnimation;
        public bool IsPlayingAnimation => playingAnimation;


        
        
        private enum CardSide
        {
            Creatures = -1,
            Heroes = 1
        }
        
        private void Awake() => Prepare();

        private void Prepare()
        {
            halfTokenSize = tokenSize / 2f;
            fullRowOffset = width * tokenSize / -2f + halfTokenSize;
        }

        public void UpdateLayout(Transform toIgnore = null, bool instantly = true)
        {
            int childCount = transform.childCount;
            if (childCount == 0) return;
            
            int rows = Mathf.FloorToInt(childCount / (float) width);
            int lastRow = childCount % width;
            
            if (instantly)
            {
                foreach (Transform child in transform)
                {
                    if(child == toIgnore) continue;
                    child.localPosition = GetChildPosition(child.GetSiblingIndex(), rows, lastRow);
                }
            }
            else
            {
                var initialPositions = new List<Vector3>();
                var toAnimate = new List<(Transform, Vector3)>();
                foreach (Transform child in transform)
                {
                    if(child == toIgnore) continue;
                    initialPositions.Add(child.localPosition);
                    toAnimate.Add((child, GetChildPosition(child.GetSiblingIndex(), rows, lastRow)));
                }
                AnimatePositionChanges(toAnimate, initialPositions);
            }
        }

        private void AnimatePositionChanges(IReadOnlyList<(Transform, Vector3)> final, IReadOnlyList<Vector3> initialPositions)
        {
            playingAnimation = true;
            for (var i = 0; i < final.Count; i++)
            {
                var (t, pos) = final[i];
                if (Mathf.Abs(initialPositions[i].z - pos.z) >= 0.25f)
                    t.DOLocalJump(pos, 0.25f, 1, 0.5f);
                else
                    t.DOLocalMove(pos, 0.25f);
            }
            
            WaitUntilAnimationEnds().Forget();
        }

        private async UniTask WaitUntilAnimationEnds()
        {
            await UniTask.WaitUntil(() => !transform.Cast<Transform>().Any(child => DOTween.IsTweening(child, true)));
            card.OnTokenLayoutAnimationEnded(this);
            playingAnimation = false;
        }

        private Vector3 GetChildPosition(int siblingIndex, int rows, int lastRow)
        {
            if (cardSide is CardSide.Creatures && card.HasBoss) return BossPosition;
            
            int col = siblingIndex % width;
            int row = Mathf.FloorToInt(siblingIndex / (float) width);
            float xOffset = row == rows ? fullRowOffset + (width - lastRow) * tokenSize / 2f : fullRowOffset;
            float x = col * tokenSize;
            float y = row * tokenSize;
            return new Vector3(x + xOffset, 0, y * (int) cardSide); 
        }

        public Vector3 GetLastChildPosition()
        {
            if (cardSide is CardSide.Creatures && card.HasBoss) return BossPosition;

            int childCount = transform.childCount;
            int rows = Mathf.FloorToInt(childCount / (float) width);
            int lastRow = childCount % width;
            return GetChildPosition(childCount - 1, rows, lastRow); 
        }

        public Vector3 BossPosition => new(0, 0, -0.235f);

        public void AttachToken(IToken token, bool resetPosition, bool ignore = false, bool instantly = true)
        {
            Transform t = token.TokenTransform;
            t.SetParent(transform, !resetPosition);
            if(resetPosition) t.localPosition = Vector3.zero;
            UpdateLayout(ignore ? token.TokenTransform : null, instantly);
        }



        private void OnValidate()
        {
            Prepare();
            UpdateLayout();
        }
    }
}