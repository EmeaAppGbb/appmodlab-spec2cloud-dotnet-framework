namespace RiverdalePermitSystem.Domain.Entities;

public class Contractor
{
    public int ContractorId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public DateTime? InsuranceExpiry { get; set; }
    public decimal? Rating { get; set; }
    public string ContactEmail { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public ICollection<Permit> Permits { get; set; } = new List<Permit>();
}
