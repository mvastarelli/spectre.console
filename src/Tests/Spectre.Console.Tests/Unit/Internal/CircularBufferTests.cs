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
    public void Should_Remove_Items_With_Predicate()
    {
        // Given
        var list = new CircularBuffer<int>(4) { 1, 2, 3, 4 };

        // When
        list.Remove(x => x < 3);

        // Then
        list.Count.ShouldBe(2);
        list[0].ShouldBe(3);
        list[1].ShouldBe(4);
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
}