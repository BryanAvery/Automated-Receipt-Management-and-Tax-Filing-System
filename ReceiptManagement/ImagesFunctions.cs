using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker;
using Tesseract;

namespace ReceiptManagement
{
    public class ImagesFunctions
    {     
        [Function("ExtractTextFromImage")]
        public async Task<IActionResult> Run(
         [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
        FunctionContext context)
        {
            ILogger log = context.GetLogger("ExtractTextFromImage");

            log.LogInformation("ExtractTextFromImage HTTP trigger function processed a request.");

            // Check content type
            if (!req.HasFormContentType)
            {
                return new UnsupportedMediaTypeResult();  // Returns a 415 Unsupported Media Type
            }

            // Read image from the request body
            if (!req.Form.Files.Any())
            {
                return new BadRequestObjectResult("Please upload an image file.");
            }

            var formFile = req.Form.Files["image"];
            if (formFile.Length > 0)
            {
                using var ms = new MemoryStream();
                formFile.CopyTo(ms);
                var imageBytes = ms.ToArray();
                try
                {
                    using var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default);
                    using var image = Pix.LoadFromMemory(imageBytes);
                    using var page = engine.Process(image);

                    string text = page.GetText();
                    return new OkObjectResult(text);
                }
                catch (Exception ex)
                {
                    log.LogError("Error processing image with Tesseract: " + ex.Message);
                    return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                }
            }

            return new BadRequestObjectResult("Invalid image data.");
        }
    }
}
