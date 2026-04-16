using RiverdalePermitSystem.Domain.Enums;

namespace RiverdalePermitSystem.Application.DTOs.Permits;

public record PermitDetailDto(
    string PermitId,
    DateTime ApplicationDate,
    string PropertyAddress,
    string ParcelNumber,
    PermitType PermitType,
    PermitStatus Status,
    decimal EstimatedCost,
    int? SquareFootage,
    ZoningDistrict? ZoningDistrict,
    string ProjectDescription,
    string? ApplicantName,
    string? ApplicantEmail,
    DateTime? IssuedDate,
    DateTime? ExpirationDate);
