namespace RiverdalePermitSystem.Domain.Entities;

public class Applicant
{
    public int ApplicantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Company { get; set; }
    public string? LicenseNumber { get; set; }
    public DateTime CreatedDate { get; set; }

    public ICollection<Permit> Permits { get; set; } = new List<Permit>();
}
