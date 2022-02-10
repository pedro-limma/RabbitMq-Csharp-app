namespace WorkerRecebedorDeMensagens
{
    public record ParametroExecucao
    {
        public string ConnectionString { get; set; } = "localhost";
        public string Queue { get; set; } = "Lancamento";
    }
}