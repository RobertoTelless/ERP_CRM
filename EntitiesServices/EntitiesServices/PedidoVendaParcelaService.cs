using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;
using ModelServices.Interfaces.Repositories;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;
using System.Data.Entity;
using System.Data;

namespace ModelServices.EntitiesServices
{
    public class PedidoVendaParcelaService : ServiceBase<PEDIDO_VENDA_PARCELA>, IPedidoVendaParcelaService
    {
        private readonly IPedidoVendaParcelaRepository _baseRepository;
        protected DUO_DatabaseEntities Db = new DUO_DatabaseEntities();

        public PedidoVendaParcelaService(IPedidoVendaParcelaRepository baseRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
        }

        public Int32 Create(PEDIDO_VENDA_PARCELA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _baseRepository.Add(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }
    }
}
