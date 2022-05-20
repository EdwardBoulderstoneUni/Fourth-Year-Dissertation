using UnityEngine;
public struct State{
    public SerializedPlayer player1;
    public SerializedPlayer player2;
}
public class GameState : MonoBehaviour
{
    public const float MoveSpeed = 4f;
    public const float JumpSpeed = 10f;
    public const float DeadZone = 0.1f;
    public const float DistanceToGround = 0.1f;

    [SerializeField] LocalInput localInput;
    [SerializeField] private int localPlayer;
    [SerializeField] private CharacterController2D[] characters = new CharacterController2D[2]; 

    private TimedQueue<InputStruct>[] inputQueues = new TimedQueue<InputStruct>[2];
    private TimedQueue<State> stateQueue;
    private TimedData<InputStruct> remoteInput;
    private int frame;
    private NetcodeManager netcodeManager;
    private bool paused;

    public State getState(){
        State state = new State();
        state.player1 = characters[0].serialized();
        state.player2 = characters[1].serialized();
        return state;
    }
    void Start(){
        paused = false;
        netcodeManager = gameObject.GetComponentInParent<NetcodeManager>();
        int delayFrames = netcodeManager.getDelayFrames(localPlayer) * 2;
        for(int character = 0; character < 2; character ++){
            inputQueues[character] = new TimedQueue<InputStruct>(delayFrames);
        }
        stateQueue = new TimedQueue<State>(delayFrames);
    }

    void FixedUpdate()
    {
        readLocalInput();
        sendLocalInput();
        if (!paused){
            readRemoteInput();
            if (!paused){
                updateGame();
                frame += 1;
            }
        }
    }
    void readLocalInput(){
        TimedData<InputStruct> userInput = new TimedData<InputStruct>();
        userInput.data = localInput.getInput();
        userInput.frame = frame;
        inputQueues[localPlayer].push(userInput);

    }
    void sendLocalInput(){
        netcodeManager.remoteInput(inputQueues[localPlayer].getFrame(frame), localPlayer);
    }
    void readRemoteInput(){
        inputQueues[(localPlayer + 1) % 2].push(netcodeManager.fetchRemote(localPlayer, frame));

    }
    void updateGame(){
        if (frame >= netcodeManager.getDelayFrames(localPlayer)){
            for(int character = 0; character < 2; character ++){
                characters[character].update(inputQueues[character].getFrame(frame - netcodeManager.getDelayFrames(localPlayer)).data);
            }
        }
    }

    public void pauseGame(){
        if (!paused){
            characters[0].pause();
            characters[1].pause();
            paused = true;
        }
    }

    public void resumeGame(){
        if(paused){
            characters[0].resume();
            characters[1].resume();
            paused = false;
        }
    }

    public void rollback(int frame){
        int currentFrame = frame;
        loadState(stateQueue.getFrame(frame));
        simulateToFrame(currentFrame);
    }
    private void loadState(TimedData<State> timedState){
        characters[0].loadState(timedState.data.player1);
        characters[1].loadState(timedState.data.player2);
        frame = timedState.frame;
    }
    private void simulateToFrame (int destFrame){
        Time.timeScale = 0;
        for (; frame < destFrame; frame++){
            for(int character = 0; character < 2; character ++){
                characters[character].update(inputQueues[character].getFrame(frame - netcodeManager.getDelayFrames(localPlayer)).data);
            }
            Physics.Simulate(Time.fixedDeltaTime);
        }
        Time.timeScale = 1;
    }
}
