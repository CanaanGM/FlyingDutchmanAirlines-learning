using System.ComponentModel.DataAnnotations;

namespace FlyingDuchmanAirlines.ControllerLayer.JsonData
{
    public class BookingData : IValidatableObject
    {
        private string _firstName;
        public string FirstName
        {
            get => _firstName;
            set => _firstName = ValidateName(value, nameof(FirstName));

        }

        private string _lastName;
        public string LastName
        {
            get => _lastName;
            set => _lastName = ValidateName(value, nameof(LastName));
        }

        private string ValidateName(string name, string propertyName)
        {
            return string.IsNullOrEmpty(name)
                ? throw new InvalidOperationException("could not set " + propertyName)
                : name;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> results = new List<ValidationResult>();

            if (FirstName is null && LastName is null)
                results.Add(new ValidationResult("All given fields are null"));
            else if (FirstName is null || LastName is null)
                results.Add(new ValidationResult("One of the given fields is null"));

            return results;
        }
    }
}
