using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesyncDetector : MonoBehaviour
{
    private const int bufferSize = 50;
    private int[] written;
    private State[,] gameStates;
    private GameState[] gameEngines;
    private readonly int[] primes = {2, 3};
    void Start()
    {
        gameEngines = gameObject.GetComponent<NetcodeManager>().gameStates;
        gameStates = new State[bufferSize, 2];
        written = new int[bufferSize];
        for (int index = 0; index < bufferSize; index++)
            written[index] = 1;
    }
    void FixedUpdate(){
        desyncDetect();
    }

    public void saveState(int gameIndex)
    {
        var gameEngine = gameEngines[gameIndex];
        var state = gameEngine.getState();
        var frameIndex = gameEngine.frame % bufferSize;
        if (written[frameIndex]%primes[gameIndex] != 0){
            written[frameIndex] = written[frameIndex]*primes[gameIndex]; 
            gameStates[frameIndex, gameIndex] = state;
        }
        
    }
    public void desyncDetect(){
        for (int index = 0; index < bufferSize; index++){
            if (written[index] == 6){
                if (gameStates[index, 0] != gameStates[index, 1]){
                    Debug.Log("DESYNC DESYNC DESYNC!");
                    Debug.Log("Game state 0 = " + gameStates[index, 0]);
                    Debug.Log("Game state 1 = " + gameStates[index, 1]);
                    Time.timeScale = 0;
                }
                written[index] = 1;

            } 
        }
    }
}
