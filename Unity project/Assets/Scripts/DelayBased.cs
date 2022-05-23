using UnityEngine;
public class DelayBased : Netcode
{
    // Not accounting for packetLoss or desync
    protected int haltingFrame = -1;
    void Start(){
        delayBased = 1;
        receivedInputs = new TimedQueue<InputStruct>(delayFrames + 1);
    }
    override public void delayFramesChange(){
        if (receivedInputs != null)
            receivedInputs.increaseBufferSizeTo(delayFrames);
    }
    override public void remoteInput(TimedData<InputStruct> input)
    {
        receivedInputs.push(input);
        Debug.Log("Delay based: Input recived for frame " + input.frame + " = " + input.data);
        unhaltOnFrame(input.frame);
    }
    override public TimedData<InputStruct> fetchRemote(int frame)
    {
        TimedData<InputStruct> remote = new TimedData<InputStruct>();
        if (receivedInputs.contains(frame))
            remote = receivedInputs.getFrame(frame);
        else
            haltForFrame(frame);

        Debug.Log("Delay based: Input sent to local for frame " + frame + " (" + remote.frame +") = " + remote.data);
        return remote;
    }
    protected void unhaltOnFrame(int frame){
        if (haltingFrame == frame){
            haltingFrame = -1;
            gameObject.GetComponent<NetcodeManager>().resumeGame(delayBased);
        }
    }
    protected void haltForFrame(int frame){
        haltingFrame = frame;
        gameObject.GetComponent<NetcodeManager>().pauseGame(delayBased);
    }

}
