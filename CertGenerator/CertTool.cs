using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace CertGenerator
{
    /// <summary>
    /// Manages certificates used by this app.
    /// </summary>
    static class CertificateTool
    {
        public static X509Certificate2 CreateSelfSigned(string dnsName, int years = 10)
        {
            using var rsa = RSA.Create(2048);
            var req = new CertificateRequest($"CN={dnsName}", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            AddHttpsUsage(req, dnsName);
            var cert = req.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddYears(years));
            return cert;
        }
        static void AddHttpsUsage(CertificateRequest req, string dnsName)
        {
            //req.CertificateExtensions.Add(
            //    new X509BasicConstraintsExtension(false, false, 0, false));

            req.CertificateExtensions.Add(
                new X509KeyUsageExtension(
                    X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.DataEncipherment | X509KeyUsageFlags.KeyEncipherment,
                    false));

            req.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(req.PublicKey, false));

            req.CertificateExtensions.Add(new X509EnhancedKeyUsageExtension([new Oid("1.3.6.1.5.5.7.3.1")], false));

            // SAN is required for modern browsers
            var san = new SubjectAlternativeNameBuilder();
            if (string.Equals(dnsName, "localhost"))
            {
                san.AddIpAddress(IPAddress.Loopback);
                san.AddIpAddress(IPAddress.IPv6Loopback);
            }
            san.AddDnsName(dnsName);
            req.CertificateExtensions.Add(san.Build());
        }


        public static void SavePublicPart(X509Certificate2 cert, string filePath)
        {
            var pubCertBytes = cert.Export(X509ContentType.Cert);
            // Create Base 64 encoded public cert file
            File.WriteAllText(filePath,
                "-----BEGIN CERTIFICATE-----\r\n"
                + Convert.ToBase64String(pubCertBytes, Base64FormattingOptions.InsertLineBreaks)
                + "\r\n-----END CERTIFICATE-----");
        }
    }
}
