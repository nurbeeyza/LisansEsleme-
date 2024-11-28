using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using LisansEşlemeUyg.Helpers;

namespace LisansEşlemeUyg.Controllers
{
    public class LicenseController : Controller
    {
        private readonly ILogger<LicenseController> _logger;

        // ILogger<LicenseController> bağımlılığını constructor üzerinden alıyoruz
        public LicenseController(ILogger<LicenseController> logger)
        {
            _logger = logger;
        }

        // Lisans doğrulama işlemi
        public IActionResult ValidateLicense()
        {
            string encryptedData = KeyHelper.ReadEncryptedData();
            _logger.LogInformation($"Encrypted Data Read: {encryptedData}");

            string expectedDecryptedData = "beyza";
            var validationResult = RSAHelper.ValidateData(encryptedData, expectedDecryptedData, _logger);

            _logger.LogInformation($"Validation Result: isValid={validationResult.isValid}, isKeyMismatch={validationResult.isKeyMismatch}");

            
            if (validationResult.isValid)
            {
                return RedirectToAction("Basarili");
            }
            else if (validationResult.isKeyMismatch)
            {
                return RedirectToAction("Hatali");
            }
            else
            {
                return RedirectToAction("Gecersiz");
            }
        }

        
        public IActionResult Basarili()
        {
            return View();
        }

        
        public IActionResult Hatali()
        {
            return View();
        }

       
        public IActionResult Gecersiz()
        {
            return View();
        }
    }
}
