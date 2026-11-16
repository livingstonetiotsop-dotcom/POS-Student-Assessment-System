namespace POS.Domain.ValueObjects;

public sealed class FinalResult
{
    public decimal Value { get; }

    public FinalResult(decimal value)
    {
        if (value < 0 || value > 10)
            throw new ArgumentOutOfRangeException(
                nameof(value), "Final result must be between 0 and 10.");

        Value = decimal.Round(value, 2, MidpointRounding.AwayFromZero);
    }

    public override string ToString() => Value.ToString("F2");
}
