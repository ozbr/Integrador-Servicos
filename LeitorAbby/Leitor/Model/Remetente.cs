namespace Leitor.Model
{
    public class Remetente
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Emails { get; set; }
        public string Assuntos { get; set; }
        public string RgxLink { get; set; }
        public string RgxSecundario { get; set; }
        public string Parametro { get; set; }
        public bool ArquivoNoCorpo { get; set; }
    }
}
