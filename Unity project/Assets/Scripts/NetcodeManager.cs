using UnityEngine;
public class NetcodeManager : MonoBehaviour
{
    // Player 0 receives rollback inputs and player 1 receives delay based inputs
    [SerializeField] private NetworkInterference interference;
    [SerializeField] private Rollback rollbackNetcode;
    [SerializeField] private DelayBased delayBasedNetcode;
    [SerializeField] public GameState[] gameStates = new GameState[2];
    public void remoteInput(TimedData<InputStruct> input, int delayBased){
        interference.interfere(getNetcode((delayBased + 1) % 2), input);
    }
    public TimedData<InputStruct> fetchRemote(int delayBased, int frame){
        return getNetcode(delayBased).fetchRemote(frame);
    }

    public int getDelayFrames(int delayBased){
        return getNetcode(delayBased).delayFrames;
    }
    public int getRollbackFrames(){
        return rollbackNetcode.delayFrames + rollbackNetcode.rollbackFrames;
    }
    public void pauseGame(int delayBased){
        gameStates[delayBased].pauseGame();
    }

    public void resumeGame(int delayBased){
        gameStates[delayBased].resumeGame();
    }

    public void rollback(int frame){
        gameStates[0].rollback(frame);
    }

    private Netcode getNetcode(int delayBased){
        return delayBased == 1 ? delayBasedNetcode : rollbackNetcode;
    }
}
