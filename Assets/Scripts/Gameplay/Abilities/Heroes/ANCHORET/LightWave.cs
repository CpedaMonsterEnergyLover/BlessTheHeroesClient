using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Pooling;
using Gameplay.Cards;
using Gameplay.GameField;
using Gameplay.Interaction;
using Gameplay.Tokens;
using UnityEngine;
using Util;
using Util.Patterns;

namespace Gameplay.Abilities
{
    public class LightWave : ActiveAbility
    {
        [SerializeField] private int startingDamage;
        [SerializeField] private int damagePerUnit;
        [SerializeField] private int numberOfUnits;
        [SerializeField] private ParticleSystem castParticles;
        [SerializeField] private ParticleSystem waveParticles;
        [SerializeField] private ParticleSystem fogParticles;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private ParticleSystemLineRenderer sparksLineRenderer;

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
                PatternSearch.IteratePlus(currentUnit.Card.GridPosition, 1, pos =>
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
                    t.Heal(GlobalDefinitions.HolyDamageType, currentDamage, Caster, false);
                else t.Damage(GlobalDefinitions.HolyDamageType, currentDamage, Caster, false, 0);
                currentDamage += damagePerUnit;
            }
            
            Mesh m = sparksLineRenderer.UpdateMesh(waveParticles);
            sparksLineRenderer.UpdateMesh(fogParticles, m);
            waveParticles.Play();

            await UniTask.WaitUntil(() => !waveParticles.isPlaying);
        }

        public override void OnCastStart()
        {
            castParticles.Play();
        }

        public override void OnCastEnd()
        {
            castParticles.Stop();
        }

        public override bool ValidateTarget(IInteractable target)
        {
            return target is IToken token &&
                   token.Card.Equals(Caster.Card) &&
                   !ReferenceEquals(Caster, target) &&
                   PatternSearch.CheckPlus(Caster.Card.GridPosition, token.Card.GridPosition, 1);
        }
    }
}