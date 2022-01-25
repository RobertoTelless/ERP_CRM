using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;
using ApplicationServices.Interfaces;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;

namespace ApplicationServices.Services
{
    public class PedidoVendaParcelaAppService : AppServiceBase<PEDIDO_VENDA_PARCELA>, IPedidoVendaParcelaAppService
    {
        private readonly IPedidoVendaParcelaService _baseService;

        public PedidoVendaParcelaAppService(IPedidoVendaParcelaService baseService) : base(baseService)
        {
            _baseService = baseService;
        }

        public Int32 ValidateCreate(PEDIDO_VENDA_PARCELA item, USUARIO usuario)
        {
            try
            {
                // Completa objeto
                item.PVPC_IN_ATIVO = 1;

                // Persiste
                Int32 volta = _baseService.Create(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
