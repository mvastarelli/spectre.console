namespace Spectre.Console.Tests.Unit.Internal;

public sealed class CircularBufferTests
{
    [Fact]
    public void Should_Initialize_With_Zero_Count()
    {
        // When
        var list = new CircularBuffer<int>(capacity: 1);

        // Then
        list.Count.ShouldBe(0);
    }

    [Theory]
    [InlineData(5, 0, 0)]
    [InlineData(5, 1, 1)]
    [InlineData(5, 2, 2)]
    [InlineData(5, 3, 3)]
    [InlineData(5, 4, 4)]
    [InlineData(5, 5, 5)]
    [InlineData(5, 6, 5)]
    public void Should_Add_Items_And_Update_Count(int capacity, int items, int expected)
    {
        // Given
        var list = new CircularBuffer<int>(capacity);

        // When
        for (var i = 0; i < items; i++)
        {
            list.Add(i);
        }

        // Then
        list.Count.ShouldBe(expected);
    }

    [Fact]
    public void Should_Overwrite_Oldest_Element_On_Add()
    {
        // Given
        var list = new CircularBuffer<int>(3) { 1, 2, 3 };

        // When
        list.Add(4);

        // Then
        list[0].ShouldBe(2);
        list[1].ShouldBe(3);
        list[2].ShouldBe(4);
    }

    [Fact]
    public void Should_Find_Known_Item()
    {
        // Given
        var list = new CircularBuffer<int>(3) { 1, 2 };

        // Then
        list.Contains(1).ShouldBeTrue();
        list.Contains(2).ShouldBeTrue();
        list.Contains(3).ShouldBeFalse();
    }

    [Fact]
    public void Should_Copy_Items_To_Array()
    {
        // Given
        var list = new CircularBuffer<int>(3) { 1, 2, 3 };

        // When
        var result = new int[3];
        list.CopyTo(result, 0);

        // Then
        result.ShouldBe([1, 2, 3]);
    }

    [Fact]
    public void Should_Copy_Only_Added_Items_To_Array()
    {
        // Given
        var list = new CircularBuffer<int>(3) { 1, 2, 3, 4, 5 };

        // When
        var result = new int[3];
        list.CopyTo(result, 0);

        // Then
        result.ShouldBe([3, 4, 5]);
    }

    [Fact]
    public void Should_Not_Copy_Removed_Items_To_Array()
    {
        // Given
        var list = new CircularBuffer<int>(3) { 1, 2, 3 };
        list.Remove();

        // When
        var result = new int[2];
        list.CopyTo(result, 0);

        // Then
        result.ShouldBe([2, 3]);
    }

    [Fact]
    public void Should_Return_Index_Of_Known_Item()
    {
        // Given
        var list = new CircularBuffer<int>(3) { 1, 2, 3 };

        // Then
        list.IndexOf(1).ShouldBe(0);
        list.IndexOf(2).ShouldBe(1);
        list.IndexOf(3).ShouldBe(2);
        list.IndexOf(4).ShouldBe(-1);
    }

    [Fact]
    public void Should_Return_Index_Of_Offset_Item()
    {
        // Given
        var list = new CircularBuffer<int>(2) { 1, 2, 3 };

        // Then
        list.IndexOf(2).ShouldBe(0);
        list.IndexOf(3).ShouldBe(1);
    }

    [Fact]
    public void Should_Remove_First_Item_On_Remove()
    {
        // Given
        var list = new CircularBuffer<int>(3) { 1, 2, 3 };

        // When
        list.Remove();

        // Then
        list.Count.ShouldBe(2);
        list[0].ShouldBe(2);
        list[1].ShouldBe(3);
    }

    [Fact]
    public void Should_Remove_Multiple_Items()
    {
        // Given
        var list = new CircularBuffer<int>(5) { 1, 2, 3, 4, 5 };

        // When
        list.Remove();
        list.Remove();

        // Then
        list.Count.ShouldBe(3);
        list[0].ShouldBe(3);
        list[1].ShouldBe(4);
        list[2].ShouldBe(5);
    }

    [Fact]
    public void Should_Remove_Multiple_Items_With_Wrapping()
    {
        // Given
        var list = new CircularBuffer<int>(3) { 1, 2, 3, 4, 5 };

        // When
        list.Remove();
        list.Remove();

        // Then
        list.Count.ShouldBe(1);
        list[0].ShouldBe(5);
    }

    [Fact]
    public void Should_Enumerate_Elements_In_Circular_Order()
    {
        // Given
        var list = new CircularBuffer<int>(3)
        {
            1,
            2,
            3,
            4,
            5
        };

        // When
        var elements = list.ToArray();

        // Then
        elements.ShouldBe([3, 4, 5]);
    }

    [Fact]
    public void Should_Support_Enumerator_Pattern()
    {
        // Given
        var list = new CircularBuffer<int>(3) { 1, 2, 3 };
        var result = new List<int>();

        // When
        foreach (var item in list)
        {
            result.Add(item);
        }

        // Then
        result.ShouldBe([1, 2, 3]);
    }
}