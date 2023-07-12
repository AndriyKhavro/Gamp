namespace Gamp.Dto;

internal class ExchangeRate
{
    public required string Txt { get; init; }
    public required decimal Rate { get; init; }
    public required string Cc { get; init; }
    public decimal Value => Rate;
}