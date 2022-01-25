using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class PedidoVendaAnexoRepository : RepositoryBase<PEDIDO_VENDA_ANEXO>, IPedidoVendaAnexoRepository
    {
        public List<PEDIDO_VENDA_ANEXO> GetAllItens()
        {
            return Db.PEDIDO_VENDA_ANEXO.ToList();
        }

        public PEDIDO_VENDA_ANEXO GetItemById(Int32 id)
        {
            IQueryable<PEDIDO_VENDA_ANEXO> query = Db.PEDIDO_VENDA_ANEXO.Where(p => p.PEVA_CD_ID == id);
            return query.FirstOrDefault();
        }

    }
}
