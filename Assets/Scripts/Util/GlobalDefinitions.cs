using Gameplay.Tokens;
using MyBox;
using Scriptable.AttackVariations;
using UnityEngine;

namespace Util
{
    public class GlobalDefinitions : MonoBehaviour
    {
        private static GlobalDefinitions Instance { get; set; }
        GlobalDefinitions() => Instance = this;

        [SerializeField] private BaseAttackVariation defaultAttackVariation;
        [Separator("Prefabs")]
        [SerializeField] private HeroToken heroTokenPrefab;
        [SerializeField] private CompanionToken companionTokenPrefab;
        [SerializeField] private CreatureToken creatureTokenPrefab;
        [SerializeField] private BossToken bossTokenPrefab;
        [Separator("Damage Sprites")]
        [SerializeField] private Sprite damageAnimationSprite;
        [SerializeField] private Sprite poisonDamageAnimationSprite;
        [SerializeField] private Sprite defensedDamageAnimationSprite;
        [SerializeField] private Sprite healingAnimationSprite;
        [Separator("Outlines")]
        [SerializeField] private Vector3 tokenOutlineWidth;
        [SerializeField] private Vector3 cardOutlineWidth;
        [SerializeField, ColorUsage(true, true)] private Color tokenOutlineGreenColor;
        [SerializeField, ColorUsage(true, true)] private Color tokenOutlineYellowColor;
        [SerializeField, ColorUsage(true, true)] private Color tokenOutlineRedColor;
        
        
        public static BaseAttackVariation BaseAttackVariation => Instance.defaultAttackVariation;
        public static Vector3 TokenOutlineWidth => Instance.tokenOutlineWidth;
        public static Vector3 CardOutlineWidth => Instance.cardOutlineWidth;
        public static Vector4 TokenOutlineGreenColor => Instance.tokenOutlineGreenColor;
        public static Vector4 TokenOutlineYellowColor => Instance.tokenOutlineYellowColor;
        public static Vector4 TokenOutlineRedColor => Instance.tokenOutlineRedColor;
        public static Sprite DamageAnimationSprite => Instance.damageAnimationSprite;
        public static Sprite DefensedDamageAnimationSprite => Instance.defensedDamageAnimationSprite;
        public static Sprite PoisonDamageAnimationSprite => Instance.poisonDamageAnimationSprite;
        public static Sprite HealingAnimationSprite => Instance.healingAnimationSprite;


        public static BossToken CreateBossToken(Scriptable.Boss boss)
        {
            var token = Instantiate(Instance.bossTokenPrefab);
            token.SetScriptable(boss);
            return token;
        }
        
        public static CreatureToken CreateCreatureToken(Scriptable.Creature creature)
        {
            var token = Instantiate(Instance.creatureTokenPrefab);
            token.SetScriptable(creature);
            return token;
        }
        
        public static HeroToken CreateHeroToken(Scriptable.Hero hero)
        {
            var token = Instantiate(Instance.heroTokenPrefab);
            token.SetScriptable(hero);
            return token;
        }
        
        public static CompanionToken CreateCompanionToken(Scriptable.Creature creature)
        {
            var token = Instantiate(Instance.companionTokenPrefab);
            token.SetScriptable(creature);
            return token;
        }
    }
}