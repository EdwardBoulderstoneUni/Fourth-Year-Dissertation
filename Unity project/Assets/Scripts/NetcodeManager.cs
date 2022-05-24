using UnityEngine;
using System;
public class NetcodeManager : MonoBehaviour
{
    [SerializeField] private NetworkInterference interference;
    [SerializeField] private Rollback rollbackNetcode;
    [SerializeField] private DelayBased delayBasedNetcode;
    [SerializeField] public GameState game;
    [SerializeField] public bool useRollback = false;
    [SerializeField] private InputManager remoteInputManager;
    public TimedData<InputStruct> fetchRemote(int frame){
        var remoteInput = new TimedData<InputStruct>();
        remoteInput.data = remoteInputManager.getInput();
        remoteInput.frame = frame;
        interference.interfere(rollbackNetcode, remoteInput);
        interference.interfere(delayBasedNetcode, remoteInput);
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
