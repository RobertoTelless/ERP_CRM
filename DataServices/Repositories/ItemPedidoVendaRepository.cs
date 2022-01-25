using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class ItemPedidoVendaRepository : RepositoryBase<ITEM_PEDIDO_VENDA>, IItemPedidoVendaRepository
    {
        public List<ITEM_PEDIDO_VENDA> GetAllItens()
        {
            return Db.ITEM_PEDIDO_VENDA.ToList();
        }

        public ITEM_PEDIDO_VENDA GetItemById(Int32 id)
        {
            IQueryable<ITEM_PEDIDO_VENDA> query = Db.ITEM_PEDIDO_VENDA.Where(p => p.ITPE_CD_ID == id);
            return query.FirstOrDefault();
        }

    }
}
