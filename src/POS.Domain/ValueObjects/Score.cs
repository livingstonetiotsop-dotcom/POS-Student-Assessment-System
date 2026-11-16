namespace POS.Domain.ValueObjects;

public sealed class Score
{
    public decimal Value { get; }

    public Score(decimal value, decimal maximum)
    {
        if (maximum <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(maximum), "Maximum score must be greater than zero.");

        if (value < 0 || value > maximum)
            throw new ArgumentOutOfRangeException(
                nameof(value), $"Score must be between 0 and {maximum}.");

        Value = value;
    }
}
