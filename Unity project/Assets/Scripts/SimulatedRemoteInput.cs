public class SimulatedRemoteInput : InputManager
{
    TimedQueue<InputStruct> remoteInputs;
    int simulatedFrame = 0;
    void Start()
    {
        inputBuffer = new bool[inputs];
        remoteInputs = new TimedQueue<InputStruct>(NetcodeManager.packetFrameSize * 2);
    }

    public override InputStruct getInput() {
        readInputs();
        reset();
        simulatedFrame += 1;
        var remoteInput = new TimedData<InputStruct>();
        remoteInput.frame = simulatedFrame;
        remoteInput.data = input;
        remoteInputs.push(remoteInput);
        return input;
    }

    public Packet<InputStruct> getRemoteInput(int frame){
        getInput();
        return remoteInputs.getPacket(frame, NetcodeManager.packetFrameSize);
    }
}
