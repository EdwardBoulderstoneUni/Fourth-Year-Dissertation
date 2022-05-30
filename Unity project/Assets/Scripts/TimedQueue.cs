using System;
[Serializable] public class TimedQueue<T>
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

    public void readPacket(Packet<T> input){
        if (bufferSize != 0){
            for (int index = 0; index < input.framesCount; index++){
                push(input.data[index]);
            }
        }
    }

    public Packet<T> getPacket(int frame, int framesCount){
        var packet = new Packet<T>();
        if (bufferSize == 0)
            return packet;
        packet.framesCount = framesCount;
        packet.data = new TimedData<T>[framesCount];
        
        for (int index = 0; index < framesCount; index++){
            packet.data[index] = getFrame(frame-index);
        }
        return packet;
    }
    public void push(TimedData<T> input){
        if (bufferSize != 0){
            if (contents[input.frame % bufferSize].frame < input.frame){
                contents[input.frame % bufferSize] = input;
            }
        }
    }

    public TimedData<T> pop(int frame){
        var data = new TimedData<T>(); 
        data.frame = -1;
        if (bufferSize != 0 && contents[frame % bufferSize].frame == frame){
            var temp = data;
            data = contents[frame % bufferSize];
            contents[frame % bufferSize] = temp;
        }
        return data;
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
