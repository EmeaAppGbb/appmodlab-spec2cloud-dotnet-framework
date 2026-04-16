namespace RiverdalePermitSystem.Domain.ValueObjects;

public record DateRange(DateTime Start, DateTime End)
{
    public bool Contains(DateTime date) => date >= Start && date <= End;
    public int DurationInDays => (End - Start).Days;
}
