using UnityEngine;
public class Rollback : DelayBased
{
    [SerializeField] [OnChangedCall("rollbackFramesChange")] public int rollbackFrames = 8;
    public TimedQueue<InputStruct> guessedInputs;
    public TimedData<InputStruct> mostRecentInput;
    void Start(){
        delayBased = 0;
        guessedInputs = new TimedQueue<InputStruct>(rollbackFrames);
        receivedInputs = new TimedQueue<InputStruct>(delayFrames + 1);
    }
    public void rollbackFramesChange(){
        if (guessedInputs != null)
            guessedInputs.increaseBufferSizeTo(rollbackFrames);
    }
    override public void remoteInput(TimedData<InputStruct> input)
    {
        receivedInputs.push(input);

        int frame = input.frame;
        if (frame > mostRecentInput.frame)
            mostRecentInput = input;
        Debug.Log("Current Frame = " + frame);
        Debug.Log("Actual input = " + input.data);
        Debug.Log("Received Inputs = " + receivedInputs);
        Debug.Log("Guessed inputs = "+ guessedInputs);
        if (guessedInputs.contains(frame) && guessedInputs.getFrame(frame).data != input.data){
            Debug.Log("ROLLBACK BABEE");
            gameObject.GetComponent<NetcodeManager>().rollback(frame);
            Debug.Log("Rollback over");
        }
        
        unhaltOnFrame(frame);
    }
    override public TimedData<InputStruct> fetchRemote(int frame)
    {
        TimedData<InputStruct> remote = new TimedData<InputStruct>();
        if (receivedInputs.contains(frame)){
            remote = receivedInputs.getFrame(frame);
        }
        else{
            if (frame - mostRecentInput.frame > rollbackFrames){
                haltForFrame(frame);
            }
            
            else{
                remote = mostRecentInput;
                guessedInputs.push(remote);
                remote.frame = frame;
            }
                
        }
        return remote;
    }
}
