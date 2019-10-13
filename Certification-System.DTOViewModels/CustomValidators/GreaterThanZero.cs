using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class GreaterThanZero : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var x = (double)value;
            return x > 0;
        }
    }
}
