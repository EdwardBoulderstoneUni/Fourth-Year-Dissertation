public class DelayBased : Netcode
{
    protected int haltingFrame = -1;
    void Start(){
        receivedInputs = new TimedQueue<InputStruct>(delayFrames + 1);
    }
    override public void delayFramesChange(){
        if (receivedInputs != null)
            receivedInputs.increaseBufferSizeTo(delayFrames);
    }
    override public void remoteInput(Packet<InputStruct> input)
    {
        receivedInputs.readPacket(input);
        unhaltOnPacket(input);
    }
    override public TimedData<InputStruct> fetchRemote(int frame)
    {
        TimedData<InputStruct> remote = new TimedData<InputStruct>();
        if (receivedInputs.contains(frame))
            remote = receivedInputs.getFrame(frame);
        else
            haltForFrame(frame);
        return remote;
    }
    protected void unhaltOnPacket(Packet<InputStruct> packet){
        if (haltingFrame > 0 && receivedInputs.contains(haltingFrame)){
            haltingFrame = -1;
            gameObject.GetComponent<NetcodeManager>().resumeGame();
        }
    }
    protected void haltForFrame(int frame){
        haltingFrame = frame;
        gameObject.GetComponent<NetcodeManager>().pauseGame();
    }
}
