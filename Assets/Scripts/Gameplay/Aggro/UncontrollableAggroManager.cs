using System.Collections.Generic;
using System.Linq;
using Gameplay.Cards;
using Gameplay.GameField;
using Gameplay.Tokens;
using UnityEngine;

namespace Gameplay.Aggro
{
    public class UncontrollableAggroManager : AggroManager<IUncontrollableToken, IControllableToken>
    {
        private readonly Dictionary<IControllableToken, float> cluster = new();



        public bool TryReaggro(out Card redirect)
        {
            redirect = null;
            Card tokenCard = Token.TokenCard;
            if (cluster.Count == 0) return false;
            if (Token.TokenCard.HeroesAmount == 0)
            {
                redirect = cluster.OrderByDescending(e => e.Key.AggroManager.AggroLevel).First().Key.TokenCard;
            }
            else
            {
                var maxFromCluster = cluster.OrderByDescending(e => e.Value).First();
                var maxFromCard = tokenCard.Heroes.OrderByDescending(h => h.AggroManager.AggroLevel).First();
                if (maxFromCluster.Value > maxFromCard.AggroManager.AggroLevel)
                    redirect = maxFromCard.TokenCard;
            }

            return redirect is not null && !redirect.Equals(tokenCard) && redirect.HasSpaceForCreature();
        }
        
        public override void AddAggro(int amount, IControllableToken source)
        {
            if(amount <= 0) return;
            
            Card current = Token.TokenCard;
            Card target = source.TokenCard;
            if(current.Equals(target) || (current.GridPosition - target.GridPosition).sqrMagnitude != 1) return;
            
            if (cluster.ContainsKey(source))
                cluster[source] += amount;
            else
            {
                cluster.Add(source, amount);
                source.OnDeath += UnlinkAggro;
                source.OnMove += OnAggroSourceMove;
            }
        }

        public override void ClearAggro()
        {
            foreach (var (t, a) in cluster) 
                t.AggroManager.RemoveAggro(a);
            cluster.Clear();
        }

        protected override void OnMove(IToken t, Card card)
        {
            ClearAggro();
        }

        private void OnAggroSourceMove(IToken t, Card c)
        {
            UnlinkAggro(t);
        }

        private void UnlinkAggro(IToken t)
        {
            IControllableToken ut = (IControllableToken) t;
            if (cluster.ContainsKey(ut)) 
                cluster.Remove(ut);
            
            t.OnDeath -= UnlinkAggro;
            t.OnMove -= OnAggroSourceMove;
        }
        
        private void OnDrawGizmos()
        {
            if(Token is null || cluster.Count == 0) return;
            Vector3 tPos = Token.TokenTransform.position;
            Gizmos.color = Color.red;
            foreach (var (t, a) in cluster) 
                Gizmos.DrawLine(tPos, t.TokenTransform.position);
        }
    }
}