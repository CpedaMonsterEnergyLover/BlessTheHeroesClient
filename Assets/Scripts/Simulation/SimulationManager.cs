using System.Collections.Generic;
using System.Linq;
using Gameplay.Dice;
using Gameplay.Simulation;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Simulation
{
    public class SimulationManager : MonoBehaviour
    {
        [SerializeField] private SimulationCube simulationCube;
        
        private static Scene simulationScene;
        private static bool simulating;
        private static readonly Dictionary<Dice, SimulationCube> simulationCubes = new ();

        private static SimulationCube SimulationCubePrefab { get; set; }

        private void Awake()
        {
            SceneManager.LoadScene(1, LoadSceneMode.Additive);
            simulationScene = SceneManager.GetSceneAt(1);
            SimulationCubePrefab = simulationCube;
        }

        private static SimulationCube GetSimulationCubeForDice(Dice dice)
        {
            if (simulationCubes.TryGetValue(dice, out SimulationCube cube)) return cube;
            
            cube = Instantiate(SimulationCubePrefab);
            cube.DiceReference = dice;
            cube.gameObject.SetActive(false);
            SceneManager.MoveGameObjectToScene(cube.gameObject, simulationScene);
            simulationCubes.Add(dice, cube);
            return cube;
        }

        public static DiceRollReplay[] SimulateDiceRoll(Dice[] dices, int amount, Vector3[] forces, Vector3[] torques)
        {
            if (simulating)
            {
                Debug.LogError("Couldn't start the simulation");
                return null;
            }
            
            ActualizeCubePositions();
            PrepareSimulation(dices, amount, forces,torques);
            bool success = Simulate(amount, dices, out var replays);

            if (!success)
            {
                Debug.LogError("Simulation aborted because it took too long");
                return null;
            }
            
            for(int i = 0; i < amount; i++) 
                replays[i].GetResult(simulationCubes[dices[i]]);
            EndSimulation();
            Dice.PrintDiceRollResult(replays.Select(replay => replay.Result).ToArray());
            return replays;
        }

        private static bool Simulate(int amount,Dice[] dices, out DiceRollReplay[] replays)
        {
            replays = new DiceRollReplay[amount];
            for (int i = 0; i < amount; i++) replays[i] = new DiceRollReplay();
            
            int counter = 1000;
            float velocity = float.MaxValue;
            while (velocity > 0 && counter > 0)
            {
                counter--;
                float max = float.NegativeInfinity;
                Physics.Simulate(Time.fixedDeltaTime);
                for (int i = 0; i < amount; i++)
                {
                    IDice cube = simulationCubes[dices[i]];
                    replays[i].Stamp(cube.Rigidbody);
                    float cubeVelocity = cube.Rigidbody.velocity.sqrMagnitude;
                    if (cubeVelocity > max) max = cubeVelocity;
                }

                velocity = max;
            }

            return counter > 0;
        }
        
        private static void PrepareSimulation(Dice[] dices, int amount, Vector3[] forces, Vector3[] torques)
        {
            simulating = true;
            Physics.autoSimulation = false;
            for (var i = 0; i < amount; i++)
            {
                Dice dice = dices[i];
                SimulationCube cube = GetSimulationCubeForDice(dice);
                cube.gameObject.SetActive(true);
                cube.Rigidbody.rotation = dice.Rigidbody.rotation;
                cube.Rigidbody.position = dice.Rigidbody.position;
                ((IDice)cube).Throw(forces[i], torques[i]);
            }
        }

        private static void ActualizeCubePositions()
        {
            foreach (SimulationCube cube in simulationCubes.Values) 
                cube.ActualizePosition();
        }

        private static void EndSimulation()
        {
            simulating = false;
            Physics.autoSimulation = true;
        }
    }
}