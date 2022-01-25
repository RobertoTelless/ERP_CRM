using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;
using CrossCutting;

namespace DataServices.Repositories
{
    public class PedidoVendaRepository : RepositoryBase<PEDIDO_VENDA>, IPedidoVendaRepository
    {
        public PEDIDO_VENDA CheckExist(PEDIDO_VENDA conta, Int32 idAss)
        {
            IQueryable<PEDIDO_VENDA> query = Db.PEDIDO_VENDA;
            query = query.Where(p => p.PEVE_NM_NOME == conta.PEVE_NM_NOME);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public PEDIDO_VENDA GetByNome(String nome, Int32 idAss)
        {
            IQueryable<PEDIDO_VENDA> query = Db.PEDIDO_VENDA.Where(p => p.PEVE_IN_ATIVO == 1);
            query = query.Where(p => p.PEVE_NM_NOME == nome);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Include(p => p.FILIAL);
            query = query.Include(p => p.FORMA_PAGAMENTO);
            query = query.Include(p => p.ITEM_PEDIDO_VENDA);
            query = query.Include(p => p.PEDIDO_VENDA_ANEXO);
            query = query.Include(p => p.USUARIO);
            return query.FirstOrDefault();
        }

        public PEDIDO_VENDA GetItemById(Int32 id)
        {
            IQueryable<PEDIDO_VENDA> query = Db.PEDIDO_VENDA;
            query = query.Where(p => p.PEVE_CD_ID == id);
            query = query.Include(p => p.FILIAL);
            query = query.Include(p => p.FORMA_PAGAMENTO);
            query = query.Include(p => p.ITEM_PEDIDO_VENDA);
            query = query.Include(p => p.PEDIDO_VENDA_ANEXO);
            query = query.Include(p => p.USUARIO);
            return query.FirstOrDefault();
        }

        public List<PEDIDO_VENDA> GetByUser(Int32 id)
        {
            IQueryable<PEDIDO_VENDA> query = Db.PEDIDO_VENDA;
            query = query.Where(p => p.USUA_CD_ID == id);
            query = query.Include(p => p.FILIAL);
            query = query.Include(p => p.FORMA_PAGAMENTO);
            query = query.Include(p => p.ITEM_PEDIDO_VENDA);
            query = query.Include(p => p.PEDIDO_VENDA_ANEXO);
            query = query.Include(p => p.USUARIO);
            return query.ToList();
        }

        public List<PEDIDO_VENDA> GetAllItens(Int32 idAss)
        {
            IQueryable<PEDIDO_VENDA> query = Db.PEDIDO_VENDA.Where(p => p.PEVE_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Include(p => p.USUARIO);
            return query.ToList();
        }

        public List<PEDIDO_VENDA> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<PEDIDO_VENDA> query = Db.PEDIDO_VENDA;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Include(p => p.USUARIO);
            return query.ToList();
        }

        public List<PEDIDO_VENDA> GetAllItensAdmUser(Int32 id, Int32 idAss)
        {
            IQueryable<PEDIDO_VENDA> query = Db.PEDIDO_VENDA;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.USUA_CD_ID == id);
            query = query.Include(p => p.USUARIO);
            return query.ToList();
        }

        public List<PEDIDO_VENDA> GetAtrasados(Int32 idAss)
        {
            IQueryable<PEDIDO_VENDA> query = Db.PEDIDO_VENDA.Where(p => p.PEVE_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.PEVE_DT_PREVISTA < DateTime.Today.Date);
            query = query.Include(p => p.USUARIO);
            return query.ToList();
        }

        public List<PEDIDO_VENDA> GetEncerrados(Int32 idAss)
        {
            IQueryable<PEDIDO_VENDA> query = Db.PEDIDO_VENDA.Where(p => p.PEVE_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.PEVE_IN_STATUS == 5);
            query = query.Include(p => p.USUARIO);
            return query.ToList();
        }

        public List<PEDIDO_VENDA> GetCancelados(Int32 idAss)
        {
            IQueryable<PEDIDO_VENDA> query = Db.PEDIDO_VENDA.Where(p => p.PEVE_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.PEVE_IN_STATUS == 6);
            query = query.Include(p => p.USUARIO);
            return query.ToList();
        }

        public List<PEDIDO_VENDA> ExecuteFilter(Int32? usuaId, String nome, String numero, DateTime? data, Int32? status, Int32 idAss)
        {
            List<PEDIDO_VENDA> lista = new List<PEDIDO_VENDA>();
            IQueryable<PEDIDO_VENDA> query = Db.PEDIDO_VENDA;
            if (usuaId != 0)
            {
                query = query.Where(p => p.USUA_CD_ID == usuaId);
            }
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.PEVE_NM_NOME.Contains(nome));
            }
            if (!String.IsNullOrEmpty(numero))
            {
                query = query.Where(p => p.PEVE_NR_NUMERO.Contains(numero));
            }
            if (status != 0)
            {
                query = query.Where(p => p.PEVE_IN_STATUS == status);
            }
            if (data != DateTime.MinValue)
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.PEVE_DT_DATA) == DbFunctions.TruncateTime(data));
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderBy(a => a.PEVE_DT_DATA).ThenBy(a => a.PEVE_IN_STATUS);
                query = query.Include(p => p.USUARIO);
                lista = query.ToList<PEDIDO_VENDA>();
            }
            return lista;
        }
    }
}
