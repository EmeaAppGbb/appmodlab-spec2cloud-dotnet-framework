namespace RiverdalePermitSystem.Domain.Entities;

public class Fee
{
    public int FeeId { get; set; }
    public string PermitId { get; set; } = string.Empty;
    public string FeeType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime? PaidDate { get; set; }
    public string? PaymentMethod { get; set; }
    public string? TransactionId { get; set; }
    public DateTime CreatedDate { get; set; }

    public Permit Permit { get; set; } = null!;
}
