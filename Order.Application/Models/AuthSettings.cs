namespace Order.Application.Models
{
    /// <summary>
    /// JWT settings bound from the "AuthSettings" configuration section.
    /// The secret must come from user-secrets or environment variables, never from source control.
    /// </summary>
    public class AuthSettings
    {
        public const string SectionName = "AuthSettings";

        public string Secret { get; set; }
        public string Issuer { get; set; } = "order-api";

        /// <summary>Token lifetime in hours.</summary>
        public int ExpireIn { get; set; } = 2;
    }
}
