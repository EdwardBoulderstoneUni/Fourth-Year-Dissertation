using UnityEngine;
using System;
public class GameState : MonoBehaviour
{
    [SerializeField] InputManager localInput;
    [SerializeField] private CharacterController2D[] characters = new CharacterController2D[2]; 
    [SerializeField] public LayerMask floor;
    [SerializeField] public float distanceToGround;
    [SerializeField] public float moveSpeed;
    [SerializeField] public float jumpSpeed;
    [SerializeField] public int rejumpPreventionFrames;    
    private TimedQueue<InputStruct>[] inputQueues = new TimedQueue<InputStruct>[2];
    private TimedQueue<State> stateQueue;
    public int frame;
    private NetcodeManager netcodeManager;
    private int last_frame;
    private bool paused;
    private bool halted;
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
        last_frame = 0;
        halted = false;
        paused = false;
        netcodeManager = gameObject.GetComponentInParent<NetcodeManager>();
        for(int character = 0; character < 2; character ++){
            inputQueues[character] = new TimedQueue<InputStruct>(Math.Max(netcodeManager.getRollbackFrames(), netcodeManager.getDelayFrames()) *3);
        }
        stateQueue = new TimedQueue<State>(netcodeManager.getRollbackFrames() + 1);
    }

    void FixedUpdate() {
        if(!halted){
            Debug.Log("GameState frame = " + frame);
            readLocalInput();
            if (!paused){
                readRemoteInput();
                if (!paused && !halted){
                    updateGame();
                }
            }
        }
    }

    void readLocalInput() {
        TimedData<InputStruct> userInput = new TimedData<InputStruct>();
        userInput.data = localInput.getInput();
        userInput.frame = frame;
        inputQueues[0].push(userInput);
    }

    void readRemoteInput() {
        inputQueues[1].push(netcodeManager.fetchRemote(getFrame()));
    }
    void updateGame() {
        int delayedFrame = frame - netcodeManager.getDelayFrames();
        if (delayedFrame >= 0){
            updateObjectStates(delayedFrame);
            Physics2D.Simulate(frameDuration);
            saveState();
        }
        frame += 1;
    }
    void updateObjectStates(int targetFrame) {
        Debug.Log("Local player = " + 0 + " Data for frame " + targetFrame + " : " + inputQueues[0] + ", " + inputQueues[1]);
        Debug.Log("Local player = " + 0 + " Inputs for frame " + targetFrame + " = { " + inputQueues[0].getFrame(targetFrame).data + ", " + inputQueues[1].getFrame(targetFrame).data + " }");
        for(int character = 0; character < 2; character ++){
            characters[character].update(inputQueues[character].getFrame(targetFrame).data);
        }
    }

    public void saveState() {
        TimedData<State> state = new TimedData<State>();
        state.data = getState();
        state.frame = frame;
        stateQueue.push(state);
    }

    public void pauseGame(){
        paused = true;
    }

    public void resumeGame(){
        paused = false;
    }

    public void rollback(int destFrame){
        halted = true;
        Debug.Log("Rolling back from frame " + frame + "to frame " + destFrame);
        last_frame = frame;
        loadState(stateQueue.getFrame(destFrame));
        simulateToFrame(last_frame);
        halted = false;
    }
    private void loadState(TimedData<State> timedState){
        var localState = timedState.data;
        characters[0].loadState(localState.player1);
        characters[1].loadState(localState.player2);
        frame = timedState.frame;
    }
    private void simulateToFrame (int destFrame){
        while (frame < destFrame){
            updateGame();
        }
    }

    public int getFrame(){
        return halted ? last_frame : frame;
    }
}
