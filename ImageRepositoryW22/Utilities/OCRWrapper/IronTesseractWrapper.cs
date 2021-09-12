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
            var imageText = Trim((await tesseract.ReadAsync(ocrInput)).Text);
            return imageText;
        }

        private string Trim(string s)
        {
            return new string(s.Where(c => c!='\n' && c!='\t' && c!='\r').ToArray());
        }
    }
}
