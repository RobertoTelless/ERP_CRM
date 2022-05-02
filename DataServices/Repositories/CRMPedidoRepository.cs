using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class CRMPedidoRepository : RepositoryBase<CRM_PEDIDO_VENDA>, ICRMPedidoRepository
    {
        public List<CRM_PEDIDO_VENDA> GetAllItens(Int32 idUsu)
        {
            IQueryable<CRM_PEDIDO_VENDA> query = Db.CRM_PEDIDO_VENDA.Where(p => p.CRPV_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idUsu);
            return query.ToList();
        }

        public CRM_PEDIDO_VENDA GetItemById(Int32 id)
        {
            IQueryable<CRM_PEDIDO_VENDA> query = Db.CRM_PEDIDO_VENDA.Where(p => p.CRPV_CD_ID == id);
            return query.FirstOrDefault();
        }

        public CRM_PEDIDO_VENDA GetItemByNumero(String num, Int32 idAss)
        {
            IQueryable<CRM_PEDIDO_VENDA> query = Db.CRM_PEDIDO_VENDA.Where(p => p.CRPV_NR_NUMERO == num);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }
    }
}
 