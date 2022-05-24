using UnityEngine;
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
    private int frame;
    private NetcodeManager netcodeManager;
    private bool paused;
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
        inputQueues[0].push(userInput);

    }
    void readRemoteInput(){
        inputQueues[1].push(netcodeManager.fetchRemote(frame));

    }
    void updateGame(){
        int delayedFrame = frame - netcodeManager.getDelayFrames();
        if (delayedFrame >= 0){
            updateObjectStates(delayedFrame);
            Physics2D.Simulate(frameDuration);
            saveState();
        }
        frame += 1;
    }
    void updateObjectStates(int frame){
        Debug.Log("Local player = " + 0 + " Inputs for frame " + frame + " = { " + inputQueues[0].getFrame(frame).data + ", " + inputQueues[1].getFrame(frame).data + " }");
        for(int character = 0; character < 2; character ++){
            characters[character].update(inputQueues[character].getFrame(frame).data);
        }
    }

    public void saveState(){
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

    public void rollback(int frame){
        int currentFrame = frame;
        loadState(stateQueue.getFrame(frame));
        simulateToFrame(currentFrame);
    }
    private void loadState(TimedData<State> timedState){
        Debug.Log("PLEAES NOE");
        var localState = timedState.data;
        characters[0].loadState(localState.player1);
        characters[1].loadState(localState.player2);
        frame = timedState.frame;
    }
    private void simulateToFrame (int destFrame){
        while (frame < destFrame){
            updateObjectStates(frame);
            Physics2D.Simulate(1/60f);
            saveState();
            frame += 1;
        }
    }

    public int getFrame(){
        return frame;
    }
}
