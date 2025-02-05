namespace DeskMarket.Common
{
    public class ValidationConstants
    {
        public const int ProductNameMinLength = 2;
        public const int ProductNameMaxLength = 60;
        public const int ProductDescriptionMinLength = 10;
        public const int ProductDescriptionMaxLength = 250;

        public const string PriceMin = "1.00";
        public const string PriceMax = "3000.00";

        public const string DateTimeFormat = "dd-MM-yyyy";

        //-------------------------------------------

        public const int CategoryNameMinLength = 3;
        public const int CategoryNameMaxLength = 20;
    }
}
