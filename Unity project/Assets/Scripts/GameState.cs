using UnityEngine;
using System;
public class GameState : MonoBehaviour
{
    [SerializeField] InputManager localInput;
    [SerializeField] InputManager trueRemoteInput;
    [SerializeField] private CharacterController2D[] characters = new CharacterController2D[2]; 
    public const float moveSpeed = 5f;
    public const float jumpSpeed = 5f;
    public const int rejumpPreventionFrames = 10;    
    private TimedQueue<InputStruct>[] inputQueues = new TimedQueue<InputStruct>[2];
    private TimedQueue<State> stateQueue;
    private int lastSimulatedFrame;
    private NetcodeManager netcodeManager;
    private int frameToPauseForResync;
    private bool paused;
    private bool resyncing;
    private const float frameDuration = 1/60f;

    public State getState(){
        State state = new State();
        state.player1 = characters[0].serialized();
        state.player2 = characters[1].serialized();
        state.player1.location -= (Vector2) gameObject.transform.position;
        state.player2.location -= (Vector2) gameObject.transform.position;
        return state;
    }
    void Start(){
        frameToPauseForResync = -1;
        paused = false;
        resyncing = false;
        netcodeManager = gameObject.GetComponentInParent<NetcodeManager>();
        for(int character = 0; character < 2; character ++){
            inputQueues[character] = new TimedQueue<InputStruct>(Math.Max(netcodeManager.getRollbackFrames(), netcodeManager.getDelayFrames()) *3);
        }
        stateQueue = new TimedQueue<State>(netcodeManager.getRollbackFrames() + 1);
    }

    void FixedUpdate() {
        tickHaltingFrames();
        if(!paused && !resyncing){
            readLocalInput();
            if (!paused){
                readRemoteInput();
                if (!paused){
                    updateGame();
                }
            }
        }

    }

    void tickHaltingFrames(){
        if (frameToPauseForResync < 1)
            resyncing = false;
        else{
            resyncing = true; 
            frameToPauseForResync -= 1;
        }
        
    }

    void readLocalInput() {
        TimedData<InputStruct> userInput = new TimedData<InputStruct>();
        userInput.data = localInput.getInput();
        userInput.frame = lastSimulatedFrame;
        inputQueues[0].push(userInput);
    }

    void readRemoteInput() {
        inputQueues[1].push(netcodeManager.fetchRemoteInputOnFrame(getFrame()));
    }
    void updateGame() {
        int delayedFrame = lastSimulatedFrame - netcodeManager.getDelayFrames();
        if (delayedFrame >= 0){
            updateObjectStates(delayedFrame);
            Physics2D.Simulate(frameDuration);
            saveState();
        }
        lastSimulatedFrame += 1;
    }
    void updateObjectStates(int targetFrame) {
        for(int character = 0; character < 2; character ++){
            if(characters[character].doInputsMatter())
                characters[character].update(inputQueues[character].getFrame(targetFrame).data);
        }
    }

    public void saveState() {
        TimedData<State> state = new TimedData<State>();
        state.data = getState();
        state.frame = lastSimulatedFrame;
        stateQueue.push(state);
    }
    public void pauseGame(){
        paused = true;
    }

    public void resumeGame(){
        paused = false;
    }

    public void rollback(int destinationFrame){
        pauseGame();
        int lastRealTimeFrame = lastSimulatedFrame;
        loadState(stateQueue.getFrame(destinationFrame));
        simulateToFrame(lastRealTimeFrame);
        resumeGame();
    }
    private void loadState(TimedData<State> timedState){
        var localState = timedState.data;
        characters[0].loadState(localState.player1);
        characters[1].loadState(localState.player2);
        lastSimulatedFrame = timedState.frame;
    }
    private void simulateToFrame (int destinationFrame){
        while (lastSimulatedFrame < destinationFrame)
            updateGame();
    }

    public int getFrame(){
        return lastSimulatedFrame;
    }
    public void haltForFrames(int frames){
        frameToPauseForResync = frames;
    }
}
