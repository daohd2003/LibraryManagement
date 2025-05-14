namespace LibraryManagement.Services.PenaltyCalculators
{
    public interface IPenaltyCalculatorFactory
    {
        IPenaltyCalculator GetCalculator(string violationType);
    }
}
