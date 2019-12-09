namespace Certification_System.DTOViewModels
{
    public class DataTableExtendedScriptViewModel
    {
        public string TableIdentificator { get; set; }
        public int NumberOfColumn { get; set; }
        public string SelectId { get; set; }

        public int[] EllipsisTargets { get; set; }
        public int MaxAmountOfCharacters { get; set; }
        public bool WordBreak { get; set; }

        public DataTableExtendedScriptViewModel(string tableIdentificator, int numberOfColumn, string selectId, int[] ellipsisTargets, int maxAmountOfCharacters, bool wordBreak)
        {
            this.TableIdentificator = tableIdentificator;
            this.NumberOfColumn = numberOfColumn;
            this.SelectId = selectId;

            this.EllipsisTargets = ellipsisTargets;
            this.MaxAmountOfCharacters = maxAmountOfCharacters;
            this.WordBreak = wordBreak;
        }
    }
}
