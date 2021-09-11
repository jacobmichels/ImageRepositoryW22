using System.Threading.Tasks;

namespace ImageRepositoryW22.Utilities.OCRWrapper
{
    public interface IOCRWrapper
    {
        public Task<string> GetTextFromImage(string path);
    }
}