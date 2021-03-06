﻿//-----------------------------------------------------------------------------
// Filename: DtlsUtilsUnitTest.cs
//
// Description: Unit tests for the DtlsUtils class.
//
// History:
// 06 Jul 2020	Aaron Clauson	Created.
//
// License: 
// BSD 3-Clause "New" or "Revised" License, see included LICENSE.md file.
//-----------------------------------------------------------------------------

using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;
using Xunit;

namespace SIPSorcery.Net.UnitTests
{
    [Trait("Category", "unit")]
    public class DtlsUtilsUnitTest
    {
        private Microsoft.Extensions.Logging.ILogger logger = null;

        public DtlsUtilsUnitTest(Xunit.Abstractions.ITestOutputHelper output)
        {
            logger = SIPSorcery.UnitTests.TestLogHelper.InitTestLogger(output);
        }

        /// <summary>
        /// Tests that creating a new self signed certificate works correctly.
        /// </summary>
        [Fact]
        public void CreateSelfSignedCertifcateUnitTest()
        {
            logger.LogDebug("--> " + System.Reflection.MethodBase.GetCurrentMethod().Name);
            logger.BeginScope(System.Reflection.MethodBase.GetCurrentMethod().Name);

            var cert = DtlsUtils.CreateSelfSignedCert();
            Assert.NotNull(cert);
        }

        /// <summary>
        /// Tests that getting a fingerprint for a certificate works correctly.
        /// </summary>
        [Fact]
        public void GetCertifcateFingerprintUnitTest()
        {
            logger.LogDebug("--> " + System.Reflection.MethodBase.GetCurrentMethod().Name);
            logger.BeginScope(System.Reflection.MethodBase.GetCurrentMethod().Name);

            var cert = DtlsUtils.CreateSelfSignedCert();
            Assert.NotNull(cert);

            var fingerprint = DtlsUtils.Fingerprint(cert);
            logger.LogDebug($"Fingerprint {fingerprint}.");

            Assert.NotNull(fingerprint.algorithm);
            Assert.NotNull(fingerprint.value);
        }

        /// <summary>
        /// Tests that the secret key can be loaded from a pfx certificate archive file.
        /// </summary>
        /// <remarks>
        /// Fails on macosx, see https://github.com/dotnet/runtime/issues/23635. Fixed in .NET Core 5, 
        /// see https://github.com/dotnet/corefx/pull/42226.
        /// </remarks>
        [Fact]
        public void LoadSecretFromArchiveUnitTest()
        {
            logger.LogDebug("--> " + System.Reflection.MethodBase.GetCurrentMethod().Name);
            logger.BeginScope(System.Reflection.MethodBase.GetCurrentMethod().Name);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                logger.LogDebug("Test skipped as MacOS is not able to load certificates from a .pfx file pre .NET Core 5.0.");
            }
            else
            {
                var cert = new X509Certificate2("certs/localhost.pfx", (string)null, X509KeyStorageFlags.Exportable);
                Assert.NotNull(cert);

                //var rsaParams = ((RSA)cert.PrivateKey).ExportParameters(true);

                var key = DtlsUtils.LoadPrivateKeyResource(cert);
                Assert.NotNull(key);
            }
        }
    }
}
