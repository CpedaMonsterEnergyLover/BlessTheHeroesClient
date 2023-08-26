using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gameplay.Dice;
using Gameplay.GameField;
using Gameplay.Interaction;
using Gameplay.Tokens;
using UnityEngine;
using UnityEngine.Serialization;
using Util;
using Random = UnityEngine.Random;

namespace Gameplay.Abilities
{
    public class FireStorm : FireAbility
    {
        [Header("FireStorm Fields")]
        [SerializeField] private int diceAmount;
        [SerializeField] private ParticleSystem stormParticles;
        [SerializeField] private Light stormLight;

        

        public override bool ValidateTarget(IInteractable target)
        {
            return target is Card { IsOpened: true };
        }

        public override async UniTask Cast(IInteractable target)
        {
            if (target is not Card {IsOpened: true} card) return;
            AnimateCast(true);
            
            // Throw dice
            DiceUtil.CaclulateMagicDiceThrow(diceAmount, DiceManager.MagicDiceSet, out int damage, out int[] sides);
            damage = Mathf.Clamp(damage, 1, 10);
            await DiceManager.ThrowReplay(DiceManager.MagicDiceSet, diceAmount, sides);
            AnimateCast(false);

            // Animate storm
            var emission = stormParticles.emission;
            emission.rateOverTime = damage / 2f;
            StartDamageTask(card, 2f / damage, damage).Forget();
            stormParticles.transform.position = card.transform.position + new Vector3(0, 0.1f, 0);
            stormParticles.Play();

            DOTween.Sequence()
                .Append(stormLight.DOIntensity(castBallDefaultIntensity, 1f))
                .AppendInterval(1.5f)
                .Append(stormLight.DOIntensity(0, 1f));
            
            await UniTask.WaitUntil(() => !stormParticles.isPlaying);
        }

        private async UniTaskVoid StartDamageTask(Card card, float delay, int loops)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            for (int loop = 0; loop < loops; loop++)
            {
                var creatures = card.Creatures.Where(token => !token.Dead).OrderBy(_ => Random.value).ToArray();
                int count = creatures.Length;
                if(count == 0) return;
                await UniTask.Delay(TimeSpan.FromSeconds(delay));
                creatures[0].Damage(1, 0);
            }
        }
    }
}