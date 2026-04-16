using System.ComponentModel.DataAnnotations;
using RiverdalePermitSystem.Domain.Enums;

namespace RiverdalePermitSystem.Application.DTOs.Permits;

public class PermitApplicationDto
{
    [Required(ErrorMessage = "Property address is required")]
    [StringLength(200)]
    public string PropertyAddress { get; set; } = string.Empty;

    [Required(ErrorMessage = "Parcel number is required")]
    [StringLength(50)]
    public string ParcelNumber { get; set; } = string.Empty;

    public ZoningDistrict? ZoningDistrict { get; set; }

    [Required(ErrorMessage = "Applicant name is required")]
    [StringLength(100)]
    public string ApplicantName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email address is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required")]
    [Phone(ErrorMessage = "Invalid phone number")]
    public string Phone { get; set; } = string.Empty;

    public string? Company { get; set; }
    public string? LicenseNumber { get; set; }

    [Required(ErrorMessage = "Permit type is required")]
    public PermitType PermitType { get; set; }

    [Required(ErrorMessage = "Project description is required")]
    [StringLength(4000)]
    public string ProjectDescription { get; set; } = string.Empty;

    [Required(ErrorMessage = "Estimated cost is required")]
    [Range(0.01, 100_000_000, ErrorMessage = "Estimated cost must be greater than $0")]
    public decimal EstimatedCost { get; set; }

    [Range(0, 10_000_000, ErrorMessage = "Square footage must be positive")]
    public int? SquareFootage { get; set; }
}
