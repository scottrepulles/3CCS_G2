namespace DHK.Module
{
    public static class StreamExtension
    {
        public static byte[] GetBytes(this Stream stream)
        {
            if (stream != null)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    return memoryStream.ToArray();
                }
            }

            return null;
        }
    }
}
