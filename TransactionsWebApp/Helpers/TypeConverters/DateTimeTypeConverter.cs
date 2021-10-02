using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace TransactionsWebApp.Helpers.TypeConverters
{
    public class DateTimeTypeConverter<DateTime> : DefaultTypeConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            string s = text.Replace("\"", "");
            System.DateTime date = System.DateTime.ParseExact(s, "dd/MM/yyyy hh:mm:ss", null);
            return date;
        }

        public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            return value.ToString();
        }
    }
}
