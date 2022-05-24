using UnityEngine;
using System;
public class NetcodeManager : MonoBehaviour
{
    // Player 0 receives rollback inputs and player 1 receives delay based inputs
    [SerializeField] private NetworkInterference interference;
    [SerializeField] private Rollback rollbackNetcode;
    [SerializeField] private DelayBased delayBasedNetcode;
    [SerializeField] public GameState game;
    [SerializeField] public bool useRollback = false;
    public TimedData<InputStruct> fetchRemote(int frame){
        return getNetcode().fetchRemote(frame);
    }

    public int getDelayFrames(){
        return Math.Max(rollbackNetcode.delayFrames, delayBasedNetcode.delayFrames);
    }
    public int getRollbackFrames(){
        return rollbackNetcode.delayFrames + rollbackNetcode.rollbackFrames;
    }
    public void pauseGame(){
        game.pauseGame();
    }

    public void resumeGame(){
        game.resumeGame();
    }

    public void rollback(int frame){
        game.rollback(frame);
    }

    private Netcode getNetcode(){
        return useRollback ? rollbackNetcode : delayBasedNetcode;
    }
}
