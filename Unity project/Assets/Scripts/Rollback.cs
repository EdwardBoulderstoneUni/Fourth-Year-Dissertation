using UnityEngine;
using System;
public class Rollback : DelayBased
{
    [SerializeField] public const int rollbackFrames = 8;
    private TimedQueue<InputStruct> guessedInputs;
    private TimedData<InputStruct> mostRecentInput;
    void Start(){
        delayBased = 0;
        guessedInputs = new TimedQueue<InputStruct>(rollbackFrames);
        recivedInputs = new TimedQueue<InputStruct>(delayFrames);
    }
    override public void remoteInput(TimedData<InputStruct> input)
    {
        recivedInputs.push(input);

        int frame = input.frame;
        if (frame > mostRecentInput.frame)
            mostRecentInput = input;

        try{
            if (guessedInputs.contains(frame) && guessedInputs.getFrame(frame).data != input.data)
                gameObject.GetComponent<NetcodeManager>().rollback(frame);
        }catch (IndexOutOfRangeException){
            Debug.Log("BROOOOOOOKEN");
            Debug.Log("Frame = " + frame + ", Contains = " + guessedInputs.contains(frame));
            Debug.Log(guessedInputs);
            Time.timeScale = 0;
            
        }
        
        unhaltOnFrame(frame);
    }
    override public TimedData<InputStruct> fetchRemote(int frame)
    {
        TimedData<InputStruct> remote = mostRecentInput;
        if (recivedInputs.contains(frame))
            remote = recivedInputs.getFrame(frame);

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
