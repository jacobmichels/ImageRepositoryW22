using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IronOcr;

namespace ImageRepositoryW22.Utilities.OCRWrapper
{
    public class IronTesseractWrapper : IOCRWrapper
    {
        public async Task<string> GetTextFromImage(string path)
        {
            var tesseract = new IronTesseract();
            tesseract.Configuration.ReadBarCodes = false;

            var ocrInput = new OcrInput(path);
            ocrInput.Binarize();
            var imageText = RemoveWhiteSpace((await tesseract.ReadAsync(ocrInput)).Text);
            return imageText;
        }

        private string RemoveWhiteSpace(string s)
        {
            return new string(s.Where(c => !char.IsWhiteSpace(c)).ToArray());
        }
    }
}
