using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rollback
{
    State[] recordedStates;
    public int currentIndex;
    public Rollback() {
        currentIndex = 0;
        recordedStates = new State[Netcode.rollbackFrames];
    }
    public void addState(State state){
        recordedStates[currentIndex] = state;
        currentIndex = (currentIndex + 1) % Netcode.rollbackFrames;
    }

    public InputStruct guessInput(){
        return new InputStruct();
    }
    public void update()
    {
        
    }
}
