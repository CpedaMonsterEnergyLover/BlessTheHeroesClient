using System.Collections.Generic;
using System.Linq;
using Camera;
using Cysharp.Threading.Tasks;
using Scriptable;
using Simulation;
using UnityEngine;

namespace Gameplay.Dice
{
    public class DiceManager : MonoBehaviour
    {
        private static DiceManager Instance { get; set; }
        
        [SerializeField] private DiceSet attackDiceSet;
        [SerializeField] private DiceSet magicDiceSet;
        [SerializeField] private DiceSet defenseDiceSet;
        [SerializeField] private EventDiceSet eventDiceSet;

        [Header("Debug"), Range(1, 3)]
        public int debug_AmountToThrow;
        public int[] debug_SidesToThrow = new int[3];
        public DiceSet debug_DiceToThrow;
        
        public static DiceSet AttackDiceSet => Instance.attackDiceSet;
        public static DiceSet MagicDiceSet => Instance.magicDiceSet;
        public static DiceSet DefenceDiseSet => Instance.defenseDiceSet;
        public static EventDiceSet EventDiceSet => Instance.eventDiceSet;
        
        private static readonly Dictionary<DiceSet, Dice[]> diceDict = new ();


        
        private DiceManager() => Instance = this;
        
        // Class methods
        public static async UniTask ThrowReplay(DiceSet diceSet, int amount, int[] sidesToThrow, 
            DiceSet against = null, int againstAmount = 0)
        {
            var dices = InstantiateDicesOrGetExisting(diceSet, amount);
            if (against is not null)
                dices = dices.Concat(InstantiateDicesOrGetExisting(against, againstAmount)).ToArray();

            int sumAmount = amount + againstAmount;
            PrepareDices(amount, againstAmount, dices, out var initialPositions, out var initialRotations);
            
            var forces = GenerateForces(amount, againstAmount);
            var torques = GenerateTorques(sumAmount);

            var replays = SimulationManager.SimulateDiceRoll(dices, sumAmount, forces, torques);
            if (replays is null) return;
            
            ResetDicePositions(sumAmount, dices, initialPositions, initialRotations);
            int[] results = replays.Select(replay => replay.Result).ToArray();
            RepaintDices(amount, againstAmount, diceSet, against, dices, sidesToThrow, results);
            await ReplayDices(sumAmount, dices, replays);
        }

        private static void PrepareDices(int amount, int againstAmount, Dice[] dices, out Vector3[] positions, out Quaternion[] rotations)
        {
            int sumAmount = amount + againstAmount;
            for (var i = 0; i < sumAmount; i++) 
                dices[i].gameObject.SetActive(false);

            positions = new Vector3[sumAmount];
            rotations = new Quaternion[sumAmount];
            Vector3 cameraPos = MainCamera.Instance.transform.position;
            Vector3 spawnPos = new(cameraPos.x, 2, cameraPos.z - 2);
            float spawnWidth = sumAmount * 0.5f;
            float offset = 0.25f * (sumAmount - 1);
            int currentAmount = sumAmount;
            for (var i = 0; i < sumAmount; i++)
            {
                if (i == amount)
                {
                    spawnPos += new Vector3(0, 0, 7.5f);
                    currentAmount = againstAmount;
                    spawnWidth = currentAmount * 0.5f;
                    offset = 0.25f * (currentAmount - 1);
                }
                Dice dice = dices[i];
                dice.gameObject.SetActive(true);
                var initialPos = spawnPos + new Vector3((i >= amount ? i - amount : i) * spawnWidth / currentAmount - offset, 0, 0);
                positions[i] = initialPos;
                rotations[i] = dice.Rigidbody.rotation;
                dice.Rigidbody.position = initialPos;
            }
        }

        private static void RepaintDices(int amount, int againstAmount, DiceSet diceSet, DiceSet againstSet, Dice[] dices, int[] sidesToThrow, int[] sidesOnTop)
        {
            for (int i = 0; i < amount; i++)
            {
                Dice dice = dices[i];
                dice.Repaint(diceSet, i,sidesToThrow[i] - sidesOnTop[i]);
            }
            
            if(againstSet is null) return;
            for (int i = amount; i < amount + againstAmount; i++)
            {
                Dice dice = dices[i];
                dice.Repaint(againstSet, i - amount,sidesToThrow[i] - sidesOnTop[i]);
            }
        }

        private static void ResetDicePositions(int amount, Dice[] dices, Vector3[] positions, Quaternion[] rotations)
        {
            for (int i = 0; i < amount; i++)
            {
                Dice dice = dices[i];
                dice.Rigidbody.position = positions[i];
                dice.Rigidbody.rotation = rotations[i];
            }
        }

        private static async UniTask ReplayDices(int amount, Dice[] dices, DiceRollReplay[] replays)
        {
            var tasks = new UniTask[amount];
            for (int i = 0; i < amount; i++)
                tasks[i] = dices[i].ReplayAsync(replays[i]);

            await UniTask.WhenAll(tasks);
        }

        private static Vector3[] GenerateForces(int amount, int againstAmount)
        {
            int sum = amount + againstAmount;
            var forces = new Vector3[sum];
            var direction = Vector3.forward;
            for (int i = 0; i < sum; i++)
            {
                if (i == amount) direction *= -1;
                forces[i] = direction * Random.Range(4f, 5f) +
                            new Vector3(Random.value * 2 - 1f, 0, 0);
            }
            return forces;
        }

        private static Vector3[] GenerateTorques(int sumAmount)
        {
            var torques = new Vector3[sumAmount];
            for (int i = 0; i < sumAmount; i++)
                torques[i] = Random.insideUnitSphere.normalized;
            return torques;
        }
        
        private static Dice[] InstantiateDicesOrGetExisting(DiceSet diceSet, int amount)
        {
            Dice[] dices = new Dice[amount];
            if (diceDict.ContainsKey(diceSet))
            {
                var existing = diceDict[diceSet];
                for (int i = 0; i < amount; i++) dices[i] = existing[i];
            }
            else
            {
                int diceInSet = diceSet.DiceAmount;
                Dice[] diceList = new Dice[diceInSet];
                for (int i = 0; i < diceInSet; i++)
                {
                    Dice dice = Instantiate(diceSet.Prefab, Instance.transform);
                    dice.gameObject.name = $"{diceSet.name}#{i}";
                    dice.gameObject.SetActive(false);
                    dice.Rigidbody.position = Vector3.zero;
                    diceList[i] = dice;
                }
                diceDict.Add(diceSet, diceList);
                dices = diceList.Take(amount).ToArray();
            }

            return dices;
        }
    }
}