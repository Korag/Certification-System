namespace Certification_System.DTOViewModels
{
    public class DataTableInitializationViewModel
    {
        public string Identificator { get; set; }
        public int[] EllipsisTargets { get; set; }
        public int MaxAmountOfCharacters { get; set; }
        public bool WordBreak { get; set; }

        public DataTableInitializationViewModel(string identificator, int[] ellipsisTargets, int maxAmountOfCharacters, bool wordBreak)
        {
            this.Identificator = identificator;
            this.EllipsisTargets = ellipsisTargets;
            this.MaxAmountOfCharacters = maxAmountOfCharacters;
            this.WordBreak = wordBreak;
        }
    }
}
