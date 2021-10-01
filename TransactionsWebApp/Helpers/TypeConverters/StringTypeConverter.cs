using CsvHelper.TypeConversion;
using CsvHelper.Configuration;
using CsvHelper;

namespace TransactionsWebApp.Helpers.TypeConverters
{
    public class StringTypeConverter<String> : DefaultTypeConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            string s = text.Replace("\"", "");
            string str = s.Replace(" ", "");
            return str;
        }

        public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            return value.ToString();
        }
    }
}
