using System.ComponentModel;
using System.Reflection;

namespace BDAS2_SEM.Model.Enum
{

    public enum TypLek
    {
        [Description("tablety")] Tablety = 0,

        [Description("sirupy")] Sirupy = 1,

        [Description("injekce")] Injekce = 2,

        [Description("ostatni")] Ostatni = 3
    }

    public static class TypLekService
    {
        public static TypLek GetTypLekById(int typLekId)
        {
            if (TypLek.IsDefined(typeof(TypLek), typLekId))
            {
                return (TypLek)typLekId;
            }
            else
            {
                throw new ArgumentException("Invalid TypLek ID");
            }
        }

        public static string GetTypLekName(TypLek typLek)
        {
            FieldInfo fi = typLek.GetType().GetField(typLek.ToString());
            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return typLek.ToString().ToLower();
        }
    }
}