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
    private int frame;
    private NetcodeManager netcodeManager;
    private bool paused;
    private bool halted;
    private const float frameDuration = 1/60f;
    private CameraClearFlags cameraClearFlags;
    private int cameraCullingMask;

    public State getState(){
        State state = new State();
        state.player1 = characters[0].serialized();
        state.player2 = characters[1].serialized();
        state.player1.location -= (Vector2) gameObject.transform.position;
        state.player2.location -= (Vector2) gameObject.transform.position;
        return state;
    }
    void Start(){
        cameraClearFlags = Camera.main.clearFlags;
        cameraCullingMask = Camera.main.cullingMask;
        halted = false;
        paused = false;
        netcodeManager = gameObject.GetComponentInParent<NetcodeManager>();
        for(int character = 0; character < 2; character ++){
            inputQueues[character] = new TimedQueue<InputStruct>(Math.Max(netcodeManager.getRollbackFrames(), netcodeManager.getDelayFrames()) *3);
        }
        stateQueue = new TimedQueue<State>(netcodeManager.getRollbackFrames() + 1);
    }

    void Update(){
        localInput.ping();
        trueRemoteInput.ping();
    }

    void FixedUpdate() {
        if(!halted){
            //Debug.Log("GameState frame = " + frame);
            readLocalInput();
            if (!paused){
                readRemoteInput();
                if (!paused){
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
        //Debug.Log("Local player = " + 0 + " Data for frame " + targetFrame + " : " + inputQueues[0] + ", " + inputQueues[1]);
        //Debug.Log("Local player = " + 0 + " Inputs for frame " + targetFrame + " = { " + inputQueues[0].getFrame(targetFrame).data + ", " + inputQueues[1].getFrame(targetFrame).data + " }");
        for(int character = 0; character < 2; character ++){
            if(characters[character].doInputsMatter())
                characters[character].update(inputQueues[character].getFrame(targetFrame).data);
        }
    }

    public void saveState() {
        TimedData<State> state = new TimedData<State>();
        state.data = getState();
        state.frame = frame;
        stateQueue.push(state);
    }
    private void haltGame(){
        pauseGame();
        halted = true;
    }

    private void unhaltGame(){
        resumeGame();
        halted = false;
    }

    public void pauseGame(){
        pauseCamera();
        paused = true;
    }

    public void resumeGame(){
        resumeCamera();
        paused = false;
    }

    private void pauseCamera(){
        Camera.main.clearFlags = CameraClearFlags.Nothing;
        Camera.main.cullingMask = 0;
    }
    private void resumeCamera(){
        Camera.main.clearFlags = cameraClearFlags;
        Camera.main.cullingMask = cameraCullingMask;
    }
    public void rollback(int destFrame){
        haltGame();
        //Debug.Log("Rolling back from frame " + frame + "to frame " + destFrame);
        int last_frame = frame;
        loadState(stateQueue.getFrame(destFrame));
        simulateToFrame(last_frame);
        unhaltGame();
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
        return frame;
    }
}
