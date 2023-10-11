using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Imaging;

namespace net_il_mio_fotoalbum.ValidationAttributes
{
    public class ValidateImgFile : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            try
            {
                if (value == null)
                    return ValidationResult.Success;

                if (value is not byte[] imageBytes)
                    throw new ArgumentException("The value must be a byte array representing an image.");


                return ValidationResult.Success;
            }
            catch (ArgumentException ex)
            {
                return new ValidationResult(ex.Message);
            }
            catch (Exception)
            {
                return new ValidationResult("Invalid image format.");
            }
        }
    }
}
