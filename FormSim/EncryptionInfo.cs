using System.Security.Authentication;

namespace FormSim
{
    /// <summary>
    /// Class EncryptionInfo. Represents the encryption format and key that the device is using.
    /// </summary>
    public class EncryptionInfo
    {
        /// <summary>
        /// Gets or sets the DUKPT format. Defaults to <see cref="CipherAlgorithmType.None"/> if not provided.
        /// </summary>
        /// <value>The DUKPT format.</value>
        public CipherAlgorithmType DukptFormatType { get; set; } = CipherAlgorithmType.None;

        /// <summary>
        /// Gets or sets the DUKPT key.
        /// </summary>
        /// <value>The DUKPT key.</value>
        public string DukptKeyType { get; set; } = "None";
    }
}
