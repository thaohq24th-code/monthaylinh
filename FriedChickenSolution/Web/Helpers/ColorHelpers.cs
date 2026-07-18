namespace Web.Helpers
{
    public static class ColorHelpers
    {
        /// <summary>Darkens a hex color by 15%</summary>
        public static string Darken(string hexColor, double factor = 0.15)
        {
            try
            {
                hexColor = hexColor.TrimStart('#');
                if (hexColor.Length != 6) return "#6f4a2f";
                int r = Convert.ToInt32(hexColor.Substring(0, 2), 16);
                int g = Convert.ToInt32(hexColor.Substring(2, 2), 16);
                int b = Convert.ToInt32(hexColor.Substring(4, 2), 16);
                r = (int)(r * (1 - factor));
                g = (int)(g * (1 - factor));
                b = (int)(b * (1 - factor));
                return $"#{r:X2}{g:X2}{b:X2}";
            }
            catch
            {
                return "#6f4a2f";
            }
        }
    }
}
