using System.Globalization;

namespace VertigoWheel.Views
{
    public static class AmountFormatter
    {
        private const int Thousand = 1000;
        private const int Million = 1000000;

        public static string Format(int amount)
        {
            if (amount >= Million)
            {
                return "x" + Abbreviate(amount / (float)Million) + "M";
            }

            if (amount >= Thousand)
            {
                return "x" + Abbreviate(amount / (float)Thousand) + "K";
            }

            return "x" + amount.ToString(CultureInfo.InvariantCulture);
        }

        private static string Abbreviate(float value)
        {
            return value.ToString("0.#", CultureInfo.InvariantCulture);
        }
    }
}