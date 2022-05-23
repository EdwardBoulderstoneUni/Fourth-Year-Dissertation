using UnityEngine;
using System;
public class Rollback : DelayBased
{
    [SerializeField] [OnChangedCall("rollbackFramesChange")] public int rollbackFrames = 8;
    private TimedQueue<InputStruct> guessedInputs;
    private TimedData<InputStruct> mostRecentInput;
    void Start(){
        delayBased = 0;
        guessedInputs = new TimedQueue<InputStruct>(rollbackFrames + 1);
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

        if (guessedInputs.contains(frame) && guessedInputs.getFrame(frame).data != input.data)
            gameObject.GetComponent<NetcodeManager>().rollback(frame);
        
        unhaltOnFrame(frame);
    }
    override public TimedData<InputStruct> fetchRemote(int frame)
    {
        TimedData<InputStruct> remote = mostRecentInput;
        if (receivedInputs.contains(frame))
            remote = receivedInputs.getFrame(frame);

        else{
            remote.frame = frame;
            if (frame - mostRecentInput.frame <= rollbackFrames)
                guessedInputs.push(remote);
            
            else
                haltForFrame(frame);
        }
        return remote;
    }
}
