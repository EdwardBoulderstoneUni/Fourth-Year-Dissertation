using System;
public class TimedQueue<T>
{
    TimedData<T>[] contents;
    int bufferSize;

    public TimedQueue(int bufferSize){
        contents = new TimedData<T>[bufferSize];
        for (int index = 0; index < bufferSize; index++){
            contents[index] = new TimedData<T>();
            contents[index].frame = -1;
        }
        this.bufferSize = bufferSize;
    }

    public void push(TimedData<T> input){
        if (bufferSize != 0){
            if (contents[input.frame % bufferSize].frame < input.frame){
                contents[input.frame % bufferSize] = input;
            }
        }
    }
    public bool contains(int frame){
        if (bufferSize == 0)
            return false;
        return contents[frame % bufferSize].frame == frame;
    }
    public TimedData<T> getFrame(int frame){
        if (bufferSize == 0)
            return new TimedData<T>();

        var frameInput = contents[frame % bufferSize];
        if (frameInput.frame == frame)
            return frameInput;
        throw new IndexOutOfRangeException("Missing data for frame");
    }

    public void increaseBufferSizeTo(int newBufferSize){
        if (newBufferSize <= bufferSize)
            return;
        newBufferSize = Math.Max(bufferSize*2, newBufferSize);
        var newContents = new TimedData<T>[newBufferSize];
        var written = new bool[newBufferSize];
        for (int index = 0; index < newBufferSize; index++){
            int newIndex = contents[index].frame % newBufferSize;
            newContents[newIndex] = contents[index];
            written[newIndex] = true;
            if (!written[index]){
                newContents[index] = new TimedData<T>();
                newContents[index].frame = -1;
            }
        }
        this.contents = newContents;
        this.bufferSize = newBufferSize;
    }

    public override string ToString(){
        string output = "{ ";
        foreach(var data in contents){
            output += "{ " + data.ToString() + " }, ";
        }
        return output + "bufferSize = " + bufferSize + " }";
    }
}
