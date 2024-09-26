using System.Threading;

/// <summary>
/// 无锁队列
/// </summary>
/// <typeparam name="T"></typeparam>
class LockFreeQueue<T> where T : class
{
    private long mHeadPos;
    private long mTailPos;
    private T[] mElements;

    private long mQueueMaxSize;
    private long mQueueSizeMask;

    /// <summary>
    /// 2的e次方
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    private long PowerOfTwo(int e)
    {
        if (e == 0)
            return 1;

        return 2 * (PowerOfTwo(e - 1));
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="capacityPower"></param>
    public LockFreeQueue(int capacityPower)
    {
        mQueueMaxSize = PowerOfTwo(capacityPower);
        mQueueSizeMask = mQueueMaxSize - 1;

        mElements = new T[mQueueMaxSize];

        mHeadPos = 0;
        mTailPos = 0;
    }

    /// <summary>
    /// 入队
    /// </summary>
    /// <param name="newElem"></param>
    public void Push(T newElem)
    {
        long insertPos = Interlocked.Increment(ref mTailPos) - 1;

        mElements[insertPos & mQueueSizeMask] = newElem;
    }

    /// <summary>
    /// 出队
    /// </summary>
    /// <returns></returns>
    public T Pop()
    {
        T popVal = Interlocked.Exchange<T>(ref mElements[mHeadPos & mQueueSizeMask], null);

        if (popVal != null)
        {
            Interlocked.Increment(ref mHeadPos);
        }

        return popVal;
    }

    /// <summary>
    /// 获取队列大小
    /// </summary>
    /// <returns></returns>
    public long getSize()
    {
        return mTailPos - mHeadPos;
    }

    /// <summary>
    /// 是否满
    /// </summary>
    /// <returns></returns>
    public bool isFull()
    {
        return getSize() >= mQueueSizeMask;
    }
}