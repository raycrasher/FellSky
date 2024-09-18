using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace FellSky.Sim;

public class SwapBuffer<T>
    where T : struct
{
    T[] readBuffer, writeBuffer;

    public int Count { get; private set; }

    public SwapBuffer(int capacity)
    {
        readBuffer = new T[capacity];
        writeBuffer = new T[capacity];
    }

    public T this[int index]
    {
        get => readBuffer[index];
        set => writeBuffer[index] = value;
    }

    public void SwapBuffers()
    {
        var tmp = readBuffer;
        readBuffer = writeBuffer;
        writeBuffer = tmp;
    }
}