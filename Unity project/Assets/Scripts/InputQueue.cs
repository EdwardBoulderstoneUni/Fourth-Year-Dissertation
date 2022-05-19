using System;
public class InputQueue
{
    InputStruct[] content;
    int startIndex;
    int endIndex;
    int bufferSize;
    public InputQueue(int bufferSize){
        startIndex = 0;
        endIndex = 0;
        content = new InputStruct[bufferSize];
        this.bufferSize = bufferSize;
    }

    public void push(InputStruct input){
        content[endIndex] = input;
        endIndex = (endIndex + 1) % bufferSize;
    }
    public InputStruct top(){
        if (startIndex == endIndex)
            throw new InvalidOperationException("No data in InputQueue");
        return content[(endIndex + bufferSize -1) % bufferSize];
    }
    public InputStruct pop(){
        if (startIndex == endIndex)
            throw new InvalidOperationException("No data in InputQueue");
        InputStruct poppedValue = content[startIndex];
        startIndex = (startIndex + 1) % bufferSize;
        return poppedValue;
    }

}
