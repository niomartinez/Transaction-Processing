using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace TransactionsWebApp.Helpers.TypeConverters
{
    public class DecimalTypeConverter<Decimal> : DefaultTypeConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            string s = text.Replace("\"", string.Empty).Trim();
            string str = s.Replace(" ", string.Empty).Trim();
            return decimal.Parse(str);
        }

        public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            return value.ToString();
        }
    }
}
