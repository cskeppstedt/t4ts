namespace T4TS
{
    public class DateTimeType : TypescriptType
    {
        private readonly string _dateTypeTypeName;

        public DateTimeType(string dateTypeTypeName)
        {
            if (dateTypeTypeName == "true")
                dateTypeTypeName = null;
            _dateTypeTypeName = dateTypeTypeName ?? "Date | moment.Moment | string";
        }

        public override string Name
        {
            get { return _dateTypeTypeName; }
        }
    }
}