using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class ItemPedidoCompraRepository : RepositoryBase<ITEM_PEDIDO_COMPRA>, IItemPedidoCompraRepository
    {
        public List<ITEM_PEDIDO_COMPRA> GetAllItens()
        {
            return Db.ITEM_PEDIDO_COMPRA.ToList();
        }

        public ITEM_PEDIDO_COMPRA GetItemById(Int32 id)
        {
            IQueryable<ITEM_PEDIDO_COMPRA> query = Db.ITEM_PEDIDO_COMPRA.Where(p => p.ITPC_CD_ID == id);
            return query.FirstOrDefault();
        }

        public ITEM_PEDIDO_COMPRA GetItemByProduto(Int32 id)
        {
            IQueryable<ITEM_PEDIDO_COMPRA> query = Db.ITEM_PEDIDO_COMPRA.Where(p => p.PROD_CD_ID == id);
            return query.FirstOrDefault();
        }

    }
}
