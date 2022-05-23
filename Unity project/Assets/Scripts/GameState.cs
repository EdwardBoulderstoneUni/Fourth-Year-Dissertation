using UnityEngine;
public struct State{
    public SerializedPlayer player1;
    public SerializedPlayer player2;
    public override string ToString(){
        return "Player1: {" + player1.ToString() + "}, " + "Player2: {" + player2.ToString() + "}";
    }
}
public class GameState : MonoBehaviour
{
    [SerializeField] LocalInput localInput;
    [SerializeField] private int localPlayer;
    [SerializeField] private CharacterController2D[] characters = new CharacterController2D[2]; 

    private TimedQueue<InputStruct>[] inputQueues = new TimedQueue<InputStruct>[2];
    private TimedQueue<State> stateQueue;
    private TimedData<InputStruct> remoteInput;
    public int frame;
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
        for(int character = 0; character < 2; character ++){
            inputQueues[character] = new TimedQueue<InputStruct>(netcodeManager.getRollbackFrames() + 1);
        }
        stateQueue = new TimedQueue<State>(netcodeManager.getRollbackFrames() + 1);
    }

    void FixedUpdate()
    {
        readLocalInput();
        sendLocalInput();
        if (!paused){
            readRemoteInput();
            if (!paused){
                updateGame();
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
        int delayedFrame = frame - netcodeManager.getDelayFrames(localPlayer);
        if (delayedFrame >= 0){
            Debug.Log("Local player = " + localPlayer + " Inputs for frame " + frame + " = { " + inputQueues[0].getFrame(delayedFrame).data + ", " + inputQueues[1].getFrame(delayedFrame).data + " }");
            for(int character = 0; character < 2; character ++){
                characters[character].update(inputQueues[character].getFrame(delayedFrame).data);
            }
        }
        saveState();
        frame += 1;
    }

    private void saveState(){
        TimedData<State> state = new TimedData<State>();
        state.data = getState();
        state.frame = frame;
        stateQueue.push(state);
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
        while (frame < destFrame){
            updateGame();
            Physics.Simulate(Time.fixedDeltaTime);
            // TODO only want to simulate for one game
        }
    }
}
