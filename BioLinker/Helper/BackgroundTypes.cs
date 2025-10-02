namespace BioLinker.Helper
{
    public class BackgroundTypes
    {
        public const string Image = "image";
        public const string Color = "color";
        public const string Gradient = "gradient";
        public const string Upload = "upload";

        public static readonly List<string> All = new()
        {
            Image, Color, Gradient, Upload
        };
    }
}
