/// <summary>
/// 常量类
/// </summary>
/// <typeparam name="T"></typeparam>
class Const<T>
{
    public T Value { get; private set; }

    public Const() { }

    public Const(T value)
        : this()
    {
        this.Value = value;
    }
}