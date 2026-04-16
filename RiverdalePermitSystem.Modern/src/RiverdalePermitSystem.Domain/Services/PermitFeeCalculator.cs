using RiverdalePermitSystem.Domain.Enums;

namespace RiverdalePermitSystem.Domain.Services;

public class PermitFeeCalculator
{
    public decimal Calculate(PermitType permitType, decimal estimatedCost, int? squareFootage, ZoningDistrict? zoningDistrict)
    {
        var (baseFee, percentage) = permitType switch
        {
            PermitType.NewConstruction => (500m, 0.03m),
            PermitType.Addition => (300m, 0.025m),
            PermitType.Electrical => (150m, 0.015m),
            PermitType.Plumbing => (150m, 0.015m),
            PermitType.Mechanical => (150m, 0.015m),
            PermitType.Demolition => (250m, 0.01m),
            _ => (100m, 0.02m)
        };

        decimal fee = baseFee + (estimatedCost * percentage);

        // Square footage surcharge for construction/addition
        if (squareFootage.HasValue && (permitType == PermitType.NewConstruction || permitType == PermitType.Addition))
        {
            fee += squareFootage.Value * 0.50m;
        }

        // Zoning surcharge for commercial/industrial
        if (zoningDistrict is ZoningDistrict.C1 or ZoningDistrict.I1)
        {
            fee += 200m;
        }

        return fee;
    }
}
