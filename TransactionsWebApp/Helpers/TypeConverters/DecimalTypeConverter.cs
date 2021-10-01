using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace TransactionsWebApp.Helpers.TypeConverters
{
    public class DecimalTypeConverter<Decimal> : DefaultTypeConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            string s = text.Replace("\"", "");
            string str = s.Replace(" ", "");
            return decimal.Parse(str);
        }

        public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            return value.ToString();
        }
    }
}
