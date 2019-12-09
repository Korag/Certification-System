using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class DataTableMultipleFilterScriptViewModel
    {
        public string TableIdentificator { get; set; }
        public ICollection<string> NumberOfColumn { get; set; }
        public ICollection<string> SelectId { get; set; }

        public int[] EllipsisTargets { get; set; }
        public int MaxAmountOfCharacters { get; set; }
        public bool WordBreak { get; set; }

        public DataTableMultipleFilterScriptViewModel(string tableIdentificator, ICollection<string> numberOfColumn, ICollection<string> selectId, int[] ellipsisTargets, int maxAmountOfCharacters, bool wordBreak)
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
