using CsvHelper.Configuration;
using System;
using Transaction_Processing.Models;
using TransactionsWebApp.Helpers.TypeConverters;

namespace TransactionsWebApp.Helpers.ClassMappers
{
    public class TransactionsMap : ClassMap<Transaction>
    {
        public TransactionsMap()
        {
            Map(m => m.TransIdentifier).TypeConverter<StringTypeConverter<String>>();
            Map(m => m.Amount).TypeConverter<DecimalTypeConverter<Decimal>>();
            Map(m => m.Currency).TypeConverter<StringTypeConverter<String>>();
            Map(m => m.TransDate).TypeConverter<DateTimeTypeConverter<DateTime>>();
            Map(m => m.Status).TypeConverter<StringTypeConverter<String>>();
        }
    }
}
