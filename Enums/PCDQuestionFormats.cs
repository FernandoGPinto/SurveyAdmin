using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SurveyAdmin.Enums
{
    /// <summary>
    /// PostCountData Question Formats.
    /// </summary>
    public enum PCDQuestionFormats
    {
        [Description("ShortText")]
        ShortText,

        [Description("LongText")]
        LongText,

        [Description("Number")]
        Number,

        [Description("Date")]
        Date,

        [Description("Time")]
        Time,

        [Description("YesNo")]
        YesNo,

        [Description("1-5")]
        OneToFive,

        [Description("1-10")]
        OneToTen,

        [Description("Checkbox")]
        Checkbox
    }

    public static class EnumExtensionMethods
    {
        public static List<string> GetEnumDescriptions(this Type type)
        {
            var descs = new List<string>();
            var names = Enum.GetNames(type);
            foreach (var name in names)
            {
                var field = type.GetField(name);
                var fds = field.GetCustomAttributes(typeof(DescriptionAttribute), true);
                foreach (DescriptionAttribute fd in fds)
                {
                    descs.Add(fd.Description);
                }
            }
            return descs;
        }
    }
}
