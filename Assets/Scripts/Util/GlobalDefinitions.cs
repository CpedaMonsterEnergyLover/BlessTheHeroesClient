using Gameplay.Tokens;
using MyBox;
using Scriptable;
using UnityEngine;

namespace Util
{
    public class GlobalDefinitions : MonoBehaviour
    {
        private static GlobalDefinitions Instance { get; set; }
        private GlobalDefinitions() => Instance = this;

        [Separator("Prefabs")]
        [SerializeField] private HeroToken heroTokenPrefab;
        [SerializeField] private CompanionToken companionTokenPrefab;
        [SerializeField] private CreatureToken creatureTokenPrefab;
        [SerializeField] private BossToken bossTokenPrefab;

        [Separator("Outlines")]
        [SerializeField] private Vector3 tokenOutlineWidth;
        [SerializeField] private Vector3 cardOutlineWidth;
        [SerializeField, ColorUsage(true, true)] private Color tokenOutlineGreenColor;
        [SerializeField, ColorUsage(true, true)] private Color tokenOutlineYellowColor;
        [SerializeField, ColorUsage(true, true)] private Color tokenOutlineRedColor;
        [Separator("Damage Types")] 
        [SerializeField] private DamageType airDamageType;
        [SerializeField] private DamageType arcaneDamageType;
        [SerializeField] private DamageType bloodDamageType;
        [SerializeField] private DamageType coldDamageType;
        [SerializeField] private DamageType earthDamageType;
        [SerializeField] private DamageType fireDamageType;
        [SerializeField] private DamageType holyDamageType;
        [SerializeField] private DamageType natureDamageType;
        [SerializeField] private DamageType physicalDamageType;
        [SerializeField] private DamageType poisonDamageType;
        [SerializeField] private DamageType shadowDamageType;
        [SerializeField] private DamageType waterDamageType;
        [SerializeField] private DamageType psychicDamageType;
        
        
        
        public static DamageType AirDamageType => Instance.airDamageType;
        public static DamageType ArcaneDamageType => Instance.arcaneDamageType;
        public static DamageType BloodDamageType => Instance.bloodDamageType;
        public static DamageType ColdDamageType => Instance.coldDamageType;
        public static DamageType EarthDamageType => Instance.earthDamageType;
        public static DamageType FireDamageType => Instance.fireDamageType;
        public static DamageType HolyDamageType => Instance.holyDamageType;
        public static DamageType NatureDamageType => Instance.natureDamageType;
        public static DamageType PhysicalDamageType => Instance.physicalDamageType;
        public static DamageType PoisonDamageType => Instance.poisonDamageType;
        public static DamageType ShadowDamageType => Instance.shadowDamageType;
        public static DamageType WaterDamageType => Instance.waterDamageType;
        public static DamageType PsychicDamageType => Instance.psychicDamageType;
        public static Vector3 TokenOutlineWidth => Instance.tokenOutlineWidth;
        public static Vector3 CardOutlineWidth => Instance.cardOutlineWidth;
        public static Vector4 TokenOutlineGreenColor => Instance.tokenOutlineGreenColor;
        public static Vector4 TokenOutlineYellowColor => Instance.tokenOutlineYellowColor;
        public static Vector4 TokenOutlineRedColor => Instance.tokenOutlineRedColor;

        public static readonly int PropertyOutlineEnabled = Shader.PropertyToID("_OutlineEnabled");
        public static readonly int PropertyOutlineColor = Shader.PropertyToID("_OutlineColor");
        public static readonly int PropertyOutlineWidth = Shader.PropertyToID("_OutlineWidth");
        

        public static BossToken CreateBossToken(Boss boss)
        {
            var token = Instantiate(Instance.bossTokenPrefab);
            token.SetScriptable(boss);
            return token;
        }
        
        public static CreatureToken CreateCreatureToken(Creature creature)
        {
            var token = Instantiate(Instance.creatureTokenPrefab);
            token.SetScriptable(creature);
            return token;
        }
        
        public static HeroToken CreateHeroToken(Hero hero)
        {
            var token = Instantiate(Instance.heroTokenPrefab);
            token.SetScriptable(hero);
            return token;
        }
        
        public static CompanionToken CreateCompanionToken(Creature creature)
        {
            var token = Instantiate(Instance.companionTokenPrefab);
            token.SetScriptable(creature);
            return token;
        }
    }
}