namespace Client.Classes
{
    public static class ApplicationConstants
    {
        //public static readonly string PathLog = @"C:\dev\ProjetoPI\AccessLog\Client\Resources\access.txt";
        public static readonly string PathLog = GetPath();
        public static string GetPath()
        {
            string path = System.AppDomain.CurrentDomain.BaseDirectory.ToString();
            string[] splitPath = path.Split("bin");
            path = splitPath[0].ToString() + "Resources\\access.txt";

            return path;
        }
    }
}
