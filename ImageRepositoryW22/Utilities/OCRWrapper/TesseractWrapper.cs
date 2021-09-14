using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tesseract;

namespace ImageRepositoryW22.Utilities.OCRWrapper
{
    public class TesseractWrapper : IOCRWrapper
    {
        public string GetTextFromImage(string path)
        {
            using (var engine  = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
            {
                using (var img = Pix.LoadFromFile(path))
                {
                    using (var page = engine.Process(img))
                    {
                        var text = Trim(page.GetText());
                        return text;
                    }
                }
            }
        }

        private string Trim(string s)
        {
            return new string(s.Where(c => c!='\n' && c!='\t' && c!='\r').ToArray());
        }
    }
}
