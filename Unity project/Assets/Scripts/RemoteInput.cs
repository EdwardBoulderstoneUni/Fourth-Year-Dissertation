public class RemoteInput : InputManager
{
    TimedQueue<InputStruct> remoteInputs;
    int remoteFrame = 0;
    // Start is called before the first frame update
    void Start()
    {
        remoteInputs = new TimedQueue<InputStruct>(NetcodeManager.packetFrameSize + 1);
    }

    public override InputStruct getInput() {
        readInputs();
        reset();
        remoteFrame += 1;
        var remoteInput = new TimedData<InputStruct>();
        remoteInput.frame = remoteFrame;
        remoteInput.data = input;
        remoteInputs.push(remoteInput);
        return input;
    }

    public Packet<InputStruct> getRemoteInput(int frame){
        getInput();
        return remoteInputs.getPacket(frame, NetcodeManager.packetFrameSize);
    }
}
