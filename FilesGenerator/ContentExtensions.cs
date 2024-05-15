namespace FilesGenerator
{
    public static class ContentExtensions
    {
        public static string RandomString(int length)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ abcdefghijklmnopqrstuvwxyz0123456789";
            var sb = new System.Text.StringBuilder();

            for (int i = 0; i < length; i++)
            {
                sb.Append(chars[Random.Shared.Next(1, chars.Length)]);
            }

            return sb.ToString();
        }
    }
}
