using UnityEngine;
using System;
public class NetcodeManager : MonoBehaviour
{
    [SerializeField] private NetworkInterference interference;
    [SerializeField] private Rollback rollbackNetcode;
    [SerializeField] private DelayBased delayBasedNetcode;
    [SerializeField] public GameState game;
    [SerializeField] public bool useRollback = false;
    [SerializeField] private SimulatedRemoteInput remoteInputManager;
    public static int packetFrameSize = 5;
    public TimedData<InputStruct> fetchRemoteInputOnFrame(int frame){
        var packet = remoteInputManager.getRemoteInput(frame);
        interference.interfere(rollbackNetcode, packet);
        interference.interfere(delayBasedNetcode, packet);
        return getNetcode().fetchRemote(frame);
    }

    public int getDelayFrames(){
        return Math.Max(rollbackNetcode.delayFrames, delayBasedNetcode.delayFrames);
    }
    public int getRollbackFrames(){
        return rollbackNetcode.delayFrames + rollbackNetcode.rollbackFrames;
    }
    public int getLocalFrame(){
        return game.getFrame();
    }
    public void pauseGame(){
        game.pauseGame();
    }

    public void resumeGame(){
        game.resumeGame();
    }
    public void haltGameForFrames(int frames){
        game.haltForFrames(frames);
    }

    public void rollback(int frame){
        game.rollback(frame);
    }

    private Netcode getNetcode(){
        return useRollback ? rollbackNetcode : delayBasedNetcode;
    }

    public NetworkStatus getStatus(){
        var networkStatus = new NetworkStatus();
        networkStatus.rollback = useRollback;
        networkStatus.rollbackFrames = rollbackNetcode.rollbackFrames;
        networkStatus.rollbackDelayFrames = rollbackNetcode.delayFrames;
        networkStatus.delayDelayFrames = delayBasedNetcode.delayFrames;
        networkStatus.ping = interference.interferenceMetrics.ping;
        networkStatus.pingDeviation = interference.interferenceMetrics.pingDeviation;
        return networkStatus;
    }
    public void swapNetcode(){
        useRollback = !useRollback;
    }

    public bool changeRollbackFrames(bool modifier){
        if (rollbackNetcode.rollbackFrames == 0 && modifier)
            return false;
        rollbackNetcode.rollbackFrames -= modifier ? 1 : -1;
        return true;
    }

    public bool changeDelayFrames(bool modifier){
        if ((useRollback && (rollbackNetcode.delayFrames == 0 && modifier)) || 
            (!useRollback && (delayBasedNetcode.delayFrames == 0 && modifier)))
            return false;
        if (useRollback)
            rollbackNetcode.delayFrames -= modifier ? 1 : -1;
        else
            delayBasedNetcode.delayFrames -= modifier ? 1 : -1;
        return true;
    }

    public bool changePing(bool modifier){
        if (interference.interferenceMetrics.ping == 0 && modifier)
            return false;
        interference.interferenceMetrics.ping -= modifier ? 1 : -1;
        return true;
    }

    public bool changePingDeviation(bool modifier){
        if (interference.interferenceMetrics.pingDeviation == 0 && modifier)
            return false;
        interference.interferenceMetrics.pingDeviation -= modifier ? 0.1f : -0.1f;
        return true;
    }

}
