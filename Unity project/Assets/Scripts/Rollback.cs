using UnityEngine;
public class Rollback : DelayBased
{
    [SerializeField] [OnChangedCall("rollbackFramesChange")] public int rollbackFrames = 8;
    private TimedQueue<InputStruct> guessedInputs;
    private TimedData<InputStruct> mostRecentInput;
    void Start(){
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
        //Debug.Log("Reciving true remote current Frame = " + frame + ", actual input = " + input.data + "\n Received Inputs = " + receivedInputs + "\n Guessed Inputs = " + guessedInputs);
        if (guessedInputs.contains(frame) && guessedInputs.getFrame(frame).data != input.data){
            //Debug.Log("ROLLBACK BABEE");
            gameObject.GetComponent<NetcodeManager>().rollback(frame);
            //Debug.Log("Rollback over");
        }
        
        unhaltOnFrame(frame);
    }
    override public TimedData<InputStruct> fetchRemote(int frame)
    {
        //bool guessed = false;
        TimedData<InputStruct> remote = new TimedData<InputStruct>();
        if (receivedInputs.contains(frame)){
            remote = receivedInputs.getFrame(frame);
        }
        else{
            if (frame - mostRecentInput.frame > rollbackFrames){
                //Debug.Log("HALTING !!!!!");
                haltForFrame(frame);
            }
            
            else{
                //guessed = true;
                remote = mostRecentInput;
                remote.frame = frame;
                guessedInputs.push(remote);
            }
                
        }
        //Debug.Log("Requesting remote Frame = " + frame + ", guessed = " + guessed + "\n Received Inputs = " + receivedInputs + "\n Guessed Inputs = " + guessedInputs);
        return remote;
    }
}
