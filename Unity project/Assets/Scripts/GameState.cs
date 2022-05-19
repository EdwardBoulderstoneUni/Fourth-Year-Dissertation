using UnityEngine;
public struct State{
    public SerializedPlayer player1;
    public SerializedPlayer player2;
    public int frameCount;
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

    private InputQueue[] inputQueues = new InputQueue[2];
    private InputStruct remoteInput;
    private int frameCount;
    private NetcodeManager netcodeManager;

    public State getState(){
        State state = new State();
        state.player1 = characters[0].serialized();
        state.player2 = characters[1].serialized();
        state.frameCount = frameCount;
        return state;
    }
    void Start(){
        netcodeManager = gameObject.GetComponentInParent<NetcodeManager>();
        for(int character = 0; character < 2; character ++){
            inputQueues[character] = new InputQueue(netcodeManager.delayFrames[localPlayer] * 2);
        }
    }
    public void loadState(State state){
        characters[0].loadState(state.player1);
        characters[1].loadState(state.player2);
        frameCount = state.frameCount;
    }
    public void simulateFrames(InputStruct[][] input, int frames){
        // TODO validate this works the way I expect
        Time.timeScale = 0;
        for (int frame = 0; frame < frames; frame++){
            for(int character = 0; character < 2; character ++){
                characters[character].update(input[frame][character]);
            }
            Physics.Simulate(Time.fixedDeltaTime);
            frameCount += 1;
        }
        Time.timeScale = 1;
        
    }
    void readLocalInput(){
        InputStruct userInput = localInput.getInput();
        userInput.frameCount = frameCount;
        inputQueues[localPlayer].push(userInput);

    }
    void sendLocalInput(){
        netcodeManager.update(inputQueues[localPlayer].getFrame(frameCount), localPlayer);
    }
    void readRemoteInput(){
        inputQueues[(localPlayer + 1) % 2].push(netcodeManager.getRemoteInput(localPlayer));
    }
    void updateGame(){
        if (frameCount >= netcodeManager.delayFrames[localPlayer]){
            for(int character = 0; character < 2; character ++){
                characters[character].update(inputQueues[character].getFrame(frameCount - netcodeManager.delayFrames[localPlayer]));
            }
        }
    }

    void FixedUpdate()
    {
        readLocalInput();
        sendLocalInput();
        readRemoteInput();
        updateGame();
        frameCount += 1;
    }
}
