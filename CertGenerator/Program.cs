
using CertGenerator;

if (args.Length == 0)
{
    Console.WriteLine("Call this with the cert host name to generate it.");
}
else
{
    var name = args[0];
    using (var cert = CertificateTool.CreateSelfSigned(name))
    {
        CertificateTool.SavePublicPart(cert, $"{name}.crt");
        File.WriteAllBytes($"{name}.pfx", cert.Export(System.Security.Cryptography.X509Certificates.X509ContentType.Pfx));
    }
    Console.WriteLine("Generated!");
}