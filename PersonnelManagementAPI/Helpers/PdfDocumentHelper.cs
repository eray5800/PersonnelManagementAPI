using PdfSharp.Pdf.IO;
using PdfSharp.Pdf;

namespace PersonnelManagementAPI.Helpers
{
    public static class PdfDocumentHelper
    {
        public static async Task<string> SavePdfDocument(string base64Pdf,string StoragePath)
        {

            if (string.IsNullOrWhiteSpace(base64Pdf))
                return null;


            const string pdfPrefix = "data:application/pdf;base64,";
            if (base64Pdf.StartsWith(pdfPrefix))
            {
                base64Pdf = base64Pdf.Substring(pdfPrefix.Length);
            }
            else
            {
                return null;
            }

            try
            {

                byte[] pdfBytes = Convert.FromBase64String(base64Pdf);


                using (var memoryStream = new MemoryStream(pdfBytes))
                {

                    if (!IsValidPdf(memoryStream))
                        return null;


                    var fileName = Guid.NewGuid().ToString() + ".pdf";
                    var filePath = Path.Combine(StoragePath, fileName);


                    Directory.CreateDirectory(StoragePath);


                    await System.IO.File.WriteAllBytesAsync(filePath, pdfBytes);

                    return fileName;
                }
            }
            catch
            {
                return null;
            }
        }

        public static bool IsValidPdf(Stream pdfStream)
        {
            try
            {

                using (PdfDocument document = PdfReader.Open(pdfStream, PdfDocumentOpenMode.ReadOnly))
                {
                    return document.PageCount > 0;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
