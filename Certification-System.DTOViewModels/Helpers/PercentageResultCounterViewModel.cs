namespace Certification_System.DTOViewModels
{
    public class PercentageResultCounterViewModel
    {
        public int UsersQuantity { get; set; }
        public double MaxAmountOfPointsToEarn { get; set; }

        public PercentageResultCounterViewModel(int userQuantity, double maxAmountOfPointsToEarn)
        {
            this.UsersQuantity = userQuantity;
            this.MaxAmountOfPointsToEarn = maxAmountOfPointsToEarn;
        }
    }
}
