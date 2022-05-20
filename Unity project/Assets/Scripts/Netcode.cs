using UnityEngine;
public abstract class Netcode : MonoBehaviour
{
    [SerializeField] public int delayFrames = 1;
    TimedQueue<InputStruct> guessedInputs;
    public abstract void remoteInput(TimedData<InputStruct> input);
    public abstract TimedData<InputStruct> fetchRemote(int frame);
}
