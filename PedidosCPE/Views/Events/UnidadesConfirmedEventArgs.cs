namespace PedidosCPE.Views.Events
{
    public class UnidadesConfirmedEventArgs : EventArgs
    {
        public double Unidades { get; set; }
        public bool Quitar { get; set; } = false;
    }
}
