public class DelayBased : Netcode
{
    // Not accounting for packetLoss or desync
    protected int haltingFrame = -1;
    void Start(){
        delayBased = 1;
        recivedInputs = new TimedQueue<InputStruct>(delayFrames);
    }
    override public void remoteInput(TimedData<InputStruct> input)
    {
        recivedInputs.push(input);
        unhaltOnFrame(input.frame);
    }
    override public TimedData<InputStruct> fetchRemote(int frame)
    {
        TimedData<InputStruct> remote = new TimedData<InputStruct>();
        if (recivedInputs.contains(frame))
            remote = recivedInputs.getFrame(frame);
        else
            haltForFrame(frame);

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
