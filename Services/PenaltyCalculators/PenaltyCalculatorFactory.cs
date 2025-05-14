namespace LibraryManagement.Services.PenaltyCalculators
{
    public class PenaltyCalculatorFactory : IPenaltyCalculatorFactory
    {
        public IPenaltyCalculator GetCalculator(string violationType)
        {
            return violationType.ToLower() switch
            {
                "late" => new LatePenaltyCalculator(),
                "damaged" => new DamagedBookPenaltyCalculator(),
                "lost" => new LostBookPenaltyCalculator(),
                _ => throw new ArgumentException("Invalid violation type")
            };
        }
    }
}
