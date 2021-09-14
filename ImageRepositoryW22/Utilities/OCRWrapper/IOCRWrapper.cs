using System.Threading.Tasks;

namespace ImageRepositoryW22.Utilities.OCRWrapper
{
    public interface IOCRWrapper
    {
        public string GetTextFromImage(string path);
    }
}