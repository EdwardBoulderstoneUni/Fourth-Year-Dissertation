using UnityEngine;
public class GameState : MonoBehaviour
{
    [SerializeField] LocalInput localInput;
    [SerializeField] private int localPlayer;
    [SerializeField] private CharacterController2D[] characters = new CharacterController2D[2]; 

    public TimedQueue<InputStruct>[] inputQueues = new TimedQueue<InputStruct>[2];
    private TimedQueue<State> stateQueue;
    public int frame;
    private NetcodeManager netcodeManager;
    public bool paused;

    public State getState(){
        State state = new State();
        state.player1 = characters[0].serialized();
        state.player2 = characters[1].serialized();
        state.player1.location -= (Vector2) gameObject.transform.position;
        state.player2.location -= (Vector2) gameObject.transform.position;
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
            updateObjectStates(delayedFrame);
            GetComponentInParent<PhysicsUpdater>().runPhysics();
        }
        frame += 1;
    }
    void updateObjectStates(int frame){
        Debug.Log("Local player = " + localPlayer + " Inputs for frame " + frame + " = { " + inputQueues[0].getFrame(frame).data + ", " + inputQueues[1].getFrame(frame).data + " }");
        for(int character = 0; character < 2; character ++){
            characters[character].update(inputQueues[character].getFrame(frame).data);
        }
    }

    public void saveState(){
        TimedData<State> state = new TimedData<State>();
        state.data = getState();
        state.frame = frame;
        stateQueue.push(state);
        gameObject.transform.parent.GetComponentInParent<DesyncDetector>().saveState(localPlayer);
    }

    public void pausePhysics(){
        characters[0].pause();
        characters[1].pause();
    }

    public void resumePhysics(){
        characters[0].resume();
        characters[1].resume();
    }

    public void pauseGame(){
        paused = true;
    }

    public void resumeGame(){
        paused = false;
    }

    public void rollback(int frame){
        int currentFrame = frame;
        loadState(stateQueue.getFrame(frame));
        simulateToFrame(currentFrame);
    }
    private void loadState(TimedData<State> timedState){
        var localState = timedState.data;
        localState.player1.location += (Vector2) gameObject.transform.position;
        localState.player2.location += (Vector2) gameObject.transform.position;
        characters[0].loadState(localState.player1);
        characters[1].loadState(localState.player2);
        frame = timedState.frame;
    }
    private void simulateToFrame (int destFrame){
        while (frame < destFrame){
            updateObjectStates(frame);
            GetComponentInParent<PhysicsUpdater>().localPhysicsUpdate(localPlayer);
            saveState();
            frame += 1;
        }
    }
}
