using UnityEngine;
public class NetcodeManager : MonoBehaviour
{
    // Player 0 has rollback and player 1 has delay based
    [SerializeField] private NetworkInterference interference;
    [SerializeField] private Rollback rollbackNetcode;
    [SerializeField] private DelayBased delayBasedNetcode;
    [SerializeField] private GameState[] gameStates = new GameState[2];
    public void remoteInput(TimedData<InputStruct> input, int delayBased){
        interference.interfere(getNetcode((delayBased + 1) % 2), input);
    }
    public TimedData<InputStruct> fetchRemote(int delayBased, int frame){
        return getNetcode(delayBased).fetchRemote(frame);
    }

    public int getDelayFrames(int delayBased){
        return getNetcode(delayBased).delayFrames;
    }
    public void pauseGame(int delayBased){
        getGameState(delayBased).pauseGame();
    }

    public void resumeGame(int delayBased){
        getGameState(delayBased).resumeGame();
    }

    public void rollback(int frame){
        getGameState(0).rollback(frame);
    }

    private GameState getGameState(int delayBased){
        return gameStates[delayBased];
    }

    private Netcode getNetcode(int delayBased){
        return delayBased == 1 ? delayBasedNetcode : rollbackNetcode;
    }
}
