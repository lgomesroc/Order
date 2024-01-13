namespace Order.Responses
{
    public class ClientResponse
    {
        public string ClientId { get; set; }
        public string Name { get; set; }
        // Adicione outras propriedades relacionadas ao cliente, se necessário
        public List<ReportItem> Report { get; set; }
    }
}
