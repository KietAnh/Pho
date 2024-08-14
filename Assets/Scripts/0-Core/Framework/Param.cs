using System;

public abstract class IParam
{

}

public class OneParam<T> : IParam
{
    public T value1;
    public OneParam() { }
    public OneParam(T value1)
    {
        this.value1 = value1;
    }
}

public class TwoParam<T1, T2> : IParam
{
    public T1 value1;
    public T2 value2;
    public TwoParam() { }
    public TwoParam(T1 value1, T2 value2)
    {
        this.value1 = value1;
        this.value2 = value2;
    }
}

public class ThreeParam<T1, T2, T3> : IParam
{
    public T1 value1;
    public T2 value2;
    public T3 value3;
    public ThreeParam() { }
    public ThreeParam(T1 value1, T2 value2, T3 value3)
    {
        this.value1 = value1;
        this.value2 = value2;
        this.value3 = value3;
    }
}

public class FourParam<T1, T2, T3, T4> : IParam
{
    public T1 value1;
    public T2 value2;
    public T3 value3;
    public T4 value4;
    public FourParam() { }
    public FourParam(T1 value1, T2 value2, T3 value3, T4 value4)
    {
        this.value1 = value1;
        this.value2 = value2;
        this.value3 = value3;
        this.value4 = value4;
    }
}

public class FiveParam<T1, T2, T3, T4, T5> : IParam
{
    public T1 value1;
    public T2 value2;
    public T3 value3;
    public T4 value4;
    public T5 value5;
    public FiveParam() { }
    public FiveParam(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5)
    {
        this.value1 = value1;
        this.value2 = value2;
        this.value3 = value3;
        this.value4 = value4;
        this.value5 = value5;
    }
}

public class SixParam<T1, T2, T3, T4, T5, T6> : IParam
{
    public T1 value1;
    public T2 value2;
    public T3 value3;
    public T4 value4;
    public T5 value5;
    public T6 value6;
    public SixParam() { }
    public SixParam(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6)
    {
        this.value1 = value1;
        this.value2 = value2;
        this.value3 = value3;
        this.value4 = value4;
        this.value5 = value5;
        this.value6 = value6;
    }
}
