using UnityEngine;
public abstract class Netcode : MonoBehaviour
{
    [SerializeField] [OnChangedCall("delayFramesChange")] public int delayFrames = 1;
    protected int delayBased;
    protected TimedQueue<InputStruct> recivedInputs;
    public abstract void remoteInput(TimedData<InputStruct> input);
    public abstract TimedData<InputStruct> fetchRemote(int frame);
    public abstract void delayFramesChange();
}
