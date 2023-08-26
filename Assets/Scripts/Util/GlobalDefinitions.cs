using Gameplay.Tokens;
using UnityEngine;

namespace Util
{
    public class GlobalDefinitions : MonoBehaviour
    {
        private static GlobalDefinitions Instance { get; set; }
        GlobalDefinitions() => Instance = this;

        [SerializeField] private CreatureToken creatureTokenPrefab;
        [SerializeField] private HeroToken heroTokenPrefab;
        [SerializeField] private Vector3 tokenOutlineWidth;
        [SerializeField] private Vector3 cardOutlineWidth;

        [SerializeField, ColorUsage(true, true)] private Color tokenOutlineGreenColor;
        [SerializeField, ColorUsage(true, true)] private Color tokenOutlineYellowColor;
        [SerializeField, ColorUsage(true, true)] private Color tokenOutlineRedColor;
        
        public static Vector3 TokenOutlineWidth => Instance.tokenOutlineWidth;
        public static Vector3 CardOutlineWidth => Instance.cardOutlineWidth;
        public static Vector4 TokenOutlineGreenColor => Instance.tokenOutlineGreenColor;
        public static Vector4 TokenOutlineYellowColor => Instance.tokenOutlineYellowColor;
        public static Vector4 TokenOutlineRedColor => Instance.tokenOutlineRedColor;
        
        
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
    }
}