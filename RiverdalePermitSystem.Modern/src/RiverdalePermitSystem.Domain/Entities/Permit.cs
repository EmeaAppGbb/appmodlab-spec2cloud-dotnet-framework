using RiverdalePermitSystem.Domain.Enums;

namespace RiverdalePermitSystem.Domain.Entities;

public class Permit
{
    public string PermitId { get; set; } = string.Empty;
    public DateTime ApplicationDate { get; set; }
    public string PropertyAddress { get; set; } = string.Empty;
    public string ParcelNumber { get; set; } = string.Empty;
    public PermitType PermitType { get; set; }
    public PermitStatus Status { get; set; }
    public decimal EstimatedCost { get; set; }
    public int? SquareFootage { get; set; }
    public ZoningDistrict? ZoningDistrict { get; set; }
    public string ProjectDescription { get; set; } = string.Empty;
    public DateTime? IssuedDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
    public byte[]? RowVersion { get; set; }

    public int? ApplicantId { get; set; }
    public Applicant? Applicant { get; set; }
    public int? ContractorId { get; set; }
    public Contractor? Contractor { get; set; }

    public ICollection<PlanReview> PlanReviews { get; set; } = new List<PlanReview>();
    public ICollection<Inspection> Inspections { get; set; } = new List<Inspection>();
    public ICollection<Fee> Fees { get; set; } = new List<Fee>();

    public static Permit Create(PermitType type, string address, string parcelNumber,
        string description, decimal estimatedCost, int? squareFootage, ZoningDistrict? zoning, string createdBy)
    {
        return new Permit
        {
            PermitId = $"PERM-{DateTime.UtcNow:yyyy}-{DateTime.UtcNow.Ticks % 100000:D5}",
            ApplicationDate = DateTime.UtcNow,
            PropertyAddress = address,
            ParcelNumber = parcelNumber,
            PermitType = type,
            Status = PermitStatus.Submitted,
            EstimatedCost = estimatedCost,
            SquareFootage = squareFootage,
            ZoningDistrict = zoning,
            ProjectDescription = description,
            CreatedBy = createdBy,
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow
        };
    }
}
