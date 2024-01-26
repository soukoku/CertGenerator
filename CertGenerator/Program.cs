
using CertGenerator;
using System.Diagnostics;

if (args.Length == 0)
{
    Console.WriteLine("Call this utility with the host name to generate self-signed certificate files for use in https.");
    Console.WriteLine("e.g.");
    using var proc = Process.GetCurrentProcess();
    Console.WriteLine($"\t{proc.ProcessName} localhost");
    Console.WriteLine("or");
    Console.WriteLine($"\t{proc.ProcessName} host.domain.com");
}
else
{
    try
    {
        var name = args[0];
        var pubCert = $"{name}.crt";
        var pairCert = $"{name}.pfx";
        using (var cert = CertificateTool.CreateSelfSigned(name))
        {
            CertificateTool.SavePublicPart(cert, pubCert);
            File.WriteAllBytes(pairCert, cert.Export(System.Security.Cryptography.X509Certificates.X509ContentType.Pfx));
        }
        Console.WriteLine("Generated these certificates:");
        Console.WriteLine($"\t{pubCert} - contains public key");
        Console.WriteLine($"\t{pairCert} - contains both public and private keys");
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine(ex.ToString());
    }
}