using System;
public struct TimedData<T>{
    public int frame;
    public T data;
    public override string ToString(){
        return "Frame: " + frame + ", " + data.ToString();
    }
}

public class TimedQueue<T>
{
    TimedData<T>[] content;
    int bufferSize;

    public TimedQueue(int bufferSize){
        content = new TimedData<T>[bufferSize];
        this.bufferSize = bufferSize;
    }

    public void push(TimedData<T> input){
        content[input.frame % bufferSize] = input;
    }
    public bool contains(int frame){
        return content[frame % bufferSize].frame == frame;
    }
    public TimedData<T> getFrame(int frame){
        var frameInput = content[frame % bufferSize];
        if (frameInput.frame == frame)
            return frameInput;
        throw new IndexOutOfRangeException("Missing data for frame");
    }

    public override string ToString(){
        string output = "{ ";
        foreach(var data in content){
            output += "{ " + data.ToString() + " }, ";
        }
        return output + " }";
    }
}
