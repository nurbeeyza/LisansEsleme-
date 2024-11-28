using LisansEşlemeUyg.Helpers;
using LisansEşlemeUyg.Middleware;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System;

public class LicenseValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LicenseValidationMiddleware> _logger;
    private readonly IMemoryCache _cache;

    public LicenseValidationMiddleware(RequestDelegate next, ILogger<LicenseValidationMiddleware> logger, IMemoryCache cache)
    {
        _next = next;
        _logger = logger;
        _cache = cache;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/License/Hatali") ||
            context.Request.Path.StartsWithSegments("/License/Gecersiz") ||
            context.Request.Path.StartsWithSegments("/License/Basarili"))
        {
            await _next(context);
            return;
        }

        LicenseValidationResult validationResult;
        if (!_cache.TryGetValue("LicenseStatus", out validationResult))
        {
            string encryptedData = KeyHelper.ReadEncryptedData();
            _logger.LogInformation($"Encrypted Data Length: {encryptedData.Length} bytes");

            string expectedDecryptedData = "beyza";
            var validation = RSAHelper.ValidateData(encryptedData, expectedDecryptedData, _logger);

            if (validation.isValid)
            {
                string filePath = "C:\\Anahtar\\encryptedData.txt";
                
                string computedHash = HashHelper.ComputeFileHash(filePath);

                
                if (RegeditHelper.KeyExists("FileHash"))
                {
                    string storedHash = RegeditHelper.ReadKey("FileHash");
                    if (!HashHelper.CompareHash(computedHash, storedHash))
                    {
                        _logger.LogError("Dosya hash eşleşmedi.");
                        validationResult = LicenseValidationResult.InvalidLicense;
                    }
                    else
                    {
                        validationResult = LicenseValidationResult.ValidLicense;
                    }
                }
                else
                {
                    
                    RegeditHelper.SaveKey("FileHash", computedHash);
                    validationResult = LicenseValidationResult.ValidLicense;
                }

                // encryptedData ve privateKey'i Registry'ye kaydetme
                string privateKey = KeyHelper.ReadPrivateKey();
                RegeditHelper.SaveKey("EncryptedData", encryptedData); // Encrypted data'yı kaydediyoruz
                RegeditHelper.SaveKey("PublicKey", privateKey); // Private key'i kaydediyoruz
            }
            else if (validation.isKeyMismatch)
            {
                validationResult = LicenseValidationResult.InvalidKey;
            }
            else
            {
                validationResult = LicenseValidationResult.InvalidLicense;
            }

            _cache.Set("LicenseStatus", validationResult, TimeSpan.FromMinutes(60));
        }

        switch (validationResult)
        {
            case LicenseValidationResult.ValidLicense:
                await _next(context);
                break;
            case LicenseValidationResult.InvalidKey:
                context.Response.Redirect("/License/Hatali");
                break;
            case LicenseValidationResult.InvalidLicense:
                context.Response.Redirect("/License/Gecersiz");
                break;
        }
    }
}
    
