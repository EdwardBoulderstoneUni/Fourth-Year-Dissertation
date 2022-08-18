using UnityEngine;
public class Rollback : DelayBased
{
    private NetcodeManager netcodeManager;
    [SerializeField] [OnChangedCall("rollbackFramesChange")] public int rollbackFrames = 8;
    [SerializeField] public bool resync = true;
    private TimedQueue<InputStruct> guessedInputs;
    private TimedData<InputStruct> mostRecentInput;
    void Start(){
        guessedInputs = new TimedQueue<InputStruct>(rollbackFrames);
        receivedInputs = new TimedQueue<InputStruct>(delayFrames + 1);
        netcodeManager = GetComponent<NetcodeManager>();
    }
    public void rollbackFramesChange(){
        if (guessedInputs != null)
            guessedInputs.increaseBufferSizeTo(rollbackFrames);
    }
    override public void remoteInput(Packet<InputStruct> input)
    {
        receivedInputs.readPacket(input);
        if (input.data[0].frame > mostRecentInput.frame){
            var rollbacked = false;
            for (int frameIndex = 0; frameIndex < input.framesCount && !rollbacked; frameIndex ++){
                var frame = input.data[frameIndex].frame;
                if (guessedInputs.contains(frame) && guessedInputs.getFrame(frame).data != input.data[frameIndex].data){
                    netcodeManager.rollback(frame);
                    rollbacked = true;
                }
            }
            
            mostRecentInput = input.data[0];
            unhaltOnPacket(input);
            haltForOneSidedRollback();
        }
    }
    private void haltForOneSidedRollback(){
        var localFrame = netcodeManager.getLocalFrame() + delayFrames;
        var networkDelta = mostRecentInput.frame - localFrame;
        if (networkDelta > 0){
            netcodeManager.haltGameForFrames(networkDelta);
        }
    }
    override public TimedData<InputStruct> fetchRemote(int frame)
    {
        TimedData<InputStruct> remote = new TimedData<InputStruct>();
        if (receivedInputs.contains(frame)){
            remote = receivedInputs.getFrame(frame);
            if (guessedInputs.contains(frame))
                guessedInputs.pop(frame);
        }
        else{
            if (frame - mostRecentInput.frame > rollbackFrames){
                haltForFrame(frame);
            }
            
            else{
                remote = mostRecentInput;
                remote.frame = frame;
                guessedInputs.push(remote);
            }
                
        }
        return remote;
    }
}
