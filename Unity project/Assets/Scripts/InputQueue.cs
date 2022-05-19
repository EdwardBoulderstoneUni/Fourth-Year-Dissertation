using System;
public class InputQueue
{
    InputStruct[] content;
    int bufferSize;
    public InputQueue(int bufferSize){
        content = new InputStruct[bufferSize];
        this.bufferSize = bufferSize;
    }

    public void push(InputStruct input){
        content[input.frameCount % bufferSize] = input;
    }
    public InputStruct getFrame(int frame){
        return content[frame % bufferSize];
    }
}
