using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class PedidoCompraAnexoRepository : RepositoryBase<PEDIDO_COMPRA_ANEXO>, IPedidoCompraAnexoRepository
    {
        public List<PEDIDO_COMPRA_ANEXO> GetAllItens()
        {
            return Db.PEDIDO_COMPRA_ANEXO.ToList();
        }

        public PEDIDO_COMPRA_ANEXO GetItemById(Int32 id)
        {
            IQueryable<PEDIDO_COMPRA_ANEXO> query = Db.PEDIDO_COMPRA_ANEXO.Where(p => p.PECA_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
