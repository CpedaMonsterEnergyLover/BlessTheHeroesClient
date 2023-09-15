using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Effects;
using Gameplay.GameField;
using Gameplay.Interaction;
using Gameplay.Tokens;
using UnityEngine;
using Util.Patterns;

namespace Gameplay.Abilities
{
    public class LightWave : ActiveAbility
    {
        [SerializeField] private int startingDamage;
        [SerializeField] private int damagePerUnit;
        [SerializeField] private int numberOfUnits;
        [SerializeField] private ParticleSystem waveParticles;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private ParticleSystemLineRenderer particleSystemLineRenderer;

        public override bool RequiresAct => false;


        protected override void OnTokenSet(IToken token)
        {
        }

        public override async UniTask Cast(IInteractable target)
        {
            if(target is not IToken token) return;
            
            int jumpCounter = 0;
            var targets = new List<IToken>
            {
                Caster,
                token
            };
            IToken currentUnit = token;
            while (jumpCounter < numberOfUnits)
            {
                jumpCounter++;
                var iTargets = new List<IToken>();
                PatternSearch.IteratePlus(currentUnit.TokenCard.GridPosition, 1, pos =>
                {
                    if (FieldManager.GetCard(pos, out Card card) && card.IsOpened)
                    {
                        iTargets.AddRange(card.Creatures);
                        iTargets.AddRange(card.Heroes);
                    }
                });

                IToken unit = null;
                foreach (IToken iTarget in iTargets.OrderBy(_ => Random.value).ToList())
                {
                    if (!targets.Contains(iTarget))
                    {
                        unit = iTarget;
                        break;
                    }
                }
                if (unit is null) break;

                currentUnit = unit;
                targets.Add(currentUnit);
            }

            waveParticles.transform.position = Vector3.zero;
            int targetsCount = targets.Count;
            lineRenderer.positionCount = targetsCount;

            int currentDamage = startingDamage;
            for (int i = 0; i < targetsCount; i++)
            {
                var t = targets[i];
                var pos = t.TokenTransform.position;
                pos.y = 0.3f;
                lineRenderer.SetPosition(i, pos);

                if (t is IControllableToken)
                    t.Heal(currentDamage);
                else t.Damage(currentDamage);
                currentDamage += damagePerUnit;
            }
            
            particleSystemLineRenderer.UpdateMesh();
            waveParticles.Play();

            await UniTask.WaitUntil(() => !waveParticles.isPlaying);
        }

        public override void OnCastStart()
        {
        }

        public override void OnCastEnd()
        {
        }

        public override bool ValidateTarget(IInteractable target)
        {
            return target is IToken token &&
                   !ReferenceEquals(Caster, target) &&
                   PatternSearch.CheckPlus(Caster.TokenCard.GridPosition, token.TokenCard.GridPosition, 1);
        }
    }
}