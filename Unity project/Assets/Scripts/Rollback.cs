using UnityEngine;
public class Rollback : DelayBased
{
    [SerializeField] [OnChangedCall("rollbackFramesChange")] public int rollbackFrames = 8;
    private TimedQueue<InputStruct> guessedInputs;
    private TimedData<InputStruct> mostRecentInput;
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
        Debug.Log("Rollback: Input received for frame " + input.frame + " = " + input.data);
        receivedInputs.push(input);

        int frame = input.frame;
        if (frame > mostRecentInput.frame)
            mostRecentInput = input;

        if (guessedInputs.contains(frame) && guessedInputs.getFrame(frame).data != input.data){
            Debug.Log("BAD BAD BAD BAD BAD BAD BAD BAD BAD ");
            Debug.Log("BAD BAD BAD BAD BAD BAD BAD BAD BAD ");
            Debug.Log("BAD BAD BAD BAD BAD BAD BAD BAD BAD ");
            Debug.Log("BAD BAD BAD BAD BAD BAD BAD BAD BAD ");
            Debug.Log("BAD BAD BAD BAD BAD BAD BAD BAD BAD ");
            gameObject.GetComponent<NetcodeManager>().rollback(frame);
        }
        
        unhaltOnFrame(frame);
    }
    override public TimedData<InputStruct> fetchRemote(int frame)
    {
        TimedData<InputStruct> remote = new TimedData<InputStruct>();
        if (receivedInputs.contains(frame))
            remote = receivedInputs.getFrame(frame);
        else{
            if (frame - mostRecentInput.frame > rollbackFrames)
                haltForFrame(frame);
            
            else{
                remote = mostRecentInput;
                guessedInputs.push(remote);
                remote.frame = frame;
            }
                
        }
        Debug.Log("Rollback: Input sent to local for frame " + frame + " (" + remote.frame +") = " + remote.data);
        return remote;
    }
}
