using System;

public struct CountedArray<T>
{
    T[] array;
    public int count { get; private set; }

    public CountedArray(int capacity)
    {
        array = new T[capacity];
        count = 0;
    }

    public void Add(T newItem)
    {
        if (count >= array.Length)
            throw new InvalidOperationException("Trying to add too many items to array");

        array[count] = newItem;
        count++;
    }
    public void Clone(CountedArray<T> arrayToCopy)
    {
        if (arrayToCopy.array.Length != array.Length)
            throw new InvalidOperationException("Trying to clone a counted array of a different length");

        count = arrayToCopy.count;

        for (int i = 0; i < count; i++)
        {
            SetIndexAt(i, arrayToCopy.GetAtIndex(i));
        }

    }
    //INSTEAD OF ADDING, WHICH WOULD REQUIRE CREATING A NEW 'T' OBJ AND PASSING IT IN
    //THIS WILL JUST RETURN THE NEXT 'T' ITEM IN THE ARRAY TO BE SETUP BY THE CALLER
    public ref T GetRefToNewlyCountedItem()
    {
        if (count >= array.Length)
            throw new InvalidOperationException("Asking for a new available space when none are left");

        if (array[count] == null)
            throw new InvalidOperationException("Asking for a new available space but these are reference types so there's no value here");

        int tempCount = count;
        count++;
        return ref array[tempCount];
    }
    public T GetAtIndex(int index)
    {
        if (index >= count)
            throw new IndexOutOfRangeException("Trying to access a blank element in a counted array");
        return array[index];
    }
    public void SetIndexAt(int index, T newValue)
    {
        if (index >= count)
            throw new IndexOutOfRangeException("Trying to set an element outside of the range of the counted array");
        array[index] = newValue;
    }
    public void Clear() => count = 0;
    public void SetCount(int newCount)
    {
        if (newCount < 0 || newCount > count)
            throw new ArgumentOutOfRangeException(nameof(newCount));

        count = newCount;
    }
    public bool Contains(T item)
    {
        for (int i = 0; i < count; i++)
        {
            if (item.Equals(GetAtIndex(i))) return true;
        }
        return false;
    }

    public void Overwrite(CountedArray<T> arrayToOverwriteWith)
    {
        if (arrayToOverwriteWith.count != count)
            throw new InvalidOperationException("Can't overwrite counted array of different count " + count + " " + arrayToOverwriteWith.count);

        Clear();
        for (int i = 0; i < arrayToOverwriteWith.count; i++)
        {
            SetIndexAt(i, arrayToOverwriteWith.GetAtIndex(i));
        }
    }
}
