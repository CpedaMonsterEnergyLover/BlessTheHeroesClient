﻿using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Gameplay.GameField;
using Gameplay.Tokens;
using UI.Browsers;
using UnityEngine;

namespace Gameplay.GameCycle
{
    public class TurnManager : MonoBehaviour
    {
        [SerializeField] private TurnBrowser turnBrowser;

        private int turnsCounter;
        private bool bossesAlive = false;

        public delegate void TurnEvent();
        public static event TurnEvent OnPlayersTurnStarted;
        public static event TurnEvent OnMonstersTurnStarted;
        public static event TurnEvent OnBossesTurnStarted;

        public static TurnStage CurrentStage { get; private set; }

        
        
        private void Awake()
        {
            turnsCounter = 1;
            CurrentStage = TurnStage.PlayersTurn;
            StartPlayersTurn();
        }

        public void PassTurn()
        {
            // if(currentStage != TurnStage.PlayersTurn) return;
            NextTurn();
        }
        
        private void NextTurn()
        {
            int newStage = (int) CurrentStage + 1;
            if (newStage == 3)
            {
                turnsCounter++;
                newStage = 0;
            }
            CurrentStage = (TurnStage) newStage;

            switch (CurrentStage)
            {
                case TurnStage.PlayersTurn:
                    StartPlayersTurn();
                    break;
                case TurnStage.MonstersTurn:
                    StartMonstersTurn().Forget();
                    break;
                case TurnStage.BossesTurn:
                    StartBossesTurn();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            turnBrowser.UpdateText(turnsCounter, CurrentStage);
        }

        private void StartPlayersTurn()
        {
            Debug.Log("Players turn has started!");
            OnPlayersTurnStarted?.Invoke();
            turnBrowser.SetPassButtonEnabled(true);
        }

        private async UniTask StartMonstersTurn()
        {
            Debug.Log("Monsters turn has started!");
            await UniTask.Delay(TimeSpan.FromSeconds(1f));
            OnMonstersTurnStarted?.Invoke();
            turnBrowser.SetPassButtonEnabled(false);
            foreach (var creature in FieldManager.GetAllCreatures().Where(creature => !creature.Dead))
            {
                await creature.MakeTurn();
                await UniTask.Delay(TimeSpan.FromMilliseconds(200));
            }
            await UniTask.Delay(TimeSpan.FromSeconds(1f));
            NextTurn();
        }

        private void StartBossesTurn()
        {
            Debug.Log("Bosses Turn has started!");
            turnBrowser.SetPassButtonEnabled(false);
            if(!bossesAlive) NextTurn();
            
            OnBossesTurnStarted?.Invoke();
        }
    }
}