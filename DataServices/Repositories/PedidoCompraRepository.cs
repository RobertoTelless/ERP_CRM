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
    public class PedidoCompraRepository : RepositoryBase<PEDIDO_COMPRA>, IPedidoCompraRepository
    {
        public PEDIDO_COMPRA CheckExist(PEDIDO_COMPRA conta, Int32 idAss)
        {
            IQueryable<PEDIDO_COMPRA> query = Db.PEDIDO_COMPRA;
            query = query.Where(p => p.PECO_NM_NOME == conta.PECO_NM_NOME);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public PEDIDO_COMPRA GetByNome(String nome, Int32 idAss)
        {
            IQueryable<PEDIDO_COMPRA> query = Db.PEDIDO_COMPRA.Where(p => p.PECO_IN_ATIVO == 1);
            query = query.Where(p => p.PECO_NM_NOME == nome);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Include(p => p.FILIAL);
            query = query.Include(p => p.FORMA_PAGAMENTO);
            query = query.Include(p => p.ITEM_PEDIDO_COMPRA);
            query = query.Include(p => p.PEDIDO_COMPRA_ANEXO);
            query = query.Include(p => p.USUARIO);
            return query.FirstOrDefault();
        }

        public PEDIDO_COMPRA GetItemById(Int32 id)
        {
            IQueryable<PEDIDO_COMPRA> query = Db.PEDIDO_COMPRA;
            query = query.Where(p => p.PECO_CD_ID == id);
            query = query.Include(p => p.FILIAL);
            query = query.Include(p => p.FORMA_PAGAMENTO);
            query = query.Include(p => p.ITEM_PEDIDO_COMPRA);
            query = query.Include(p => p.PEDIDO_COMPRA_ANEXO);
            query = query.Include(p => p.USUARIO);
            return query.FirstOrDefault();
        }

        public List<PEDIDO_COMPRA> GetByUser(Int32 id)
        {
            IQueryable<PEDIDO_COMPRA> query = Db.PEDIDO_COMPRA;
            query = query.Where(p => p.USUA_CD_ID == id);
            query = query.Include(p => p.FILIAL);
            query = query.Include(p => p.FORMA_PAGAMENTO);
            query = query.Include(p => p.ITEM_PEDIDO_COMPRA);
            query = query.Include(p => p.PEDIDO_COMPRA_ANEXO);
            query = query.Include(p => p.USUARIO);
            return query.ToList();
        }

        public List<PEDIDO_COMPRA> GetAllItens(Int32 idAss)
        {
            IQueryable<PEDIDO_COMPRA> query = Db.PEDIDO_COMPRA.Where(p => p.PECO_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Include(p => p.USUARIO);
            return query.ToList();
        }

        public List<PEDIDO_COMPRA> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<PEDIDO_COMPRA> query = Db.PEDIDO_COMPRA;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Include(p => p.USUARIO);
            return query.ToList();
        }

        public List<PEDIDO_COMPRA> GetAllItensAdmUser(Int32 id, Int32 idAss)
        {
            IQueryable<PEDIDO_COMPRA> query = Db.PEDIDO_COMPRA;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.USUA_CD_ID == id);
            query = query.Include(p => p.USUARIO);
            return query.ToList();
        }

        public List<PEDIDO_COMPRA> GetAtrasados(Int32 idAss)
        {
            IQueryable<PEDIDO_COMPRA> query = Db.PEDIDO_COMPRA.Where(p => p.PECO_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.PECO_DT_PREVISTA < DateTime.Today.Date);
            query = query.Include(p => p.USUARIO);
            return query.ToList();
        }

        public List<PEDIDO_COMPRA> GetEncerrados(Int32 idAss)
        {
            IQueryable<PEDIDO_COMPRA> query = Db.PEDIDO_COMPRA.Where(p => p.PECO_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.PECO_IN_STATUS == 5);
            query = query.Include(p => p.USUARIO);
            return query.ToList();
        }

        public List<PEDIDO_COMPRA> GetCancelados(Int32 idAss)
        {
            IQueryable<PEDIDO_COMPRA> query = Db.PEDIDO_COMPRA.Where(p => p.PECO_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.PECO_IN_STATUS == 6);
            query = query.Include(p => p.USUARIO);
            return query.ToList();
        }

        public List<PEDIDO_COMPRA> ExecuteFilter(Int32? usuaId, String nome, String numero, String nf, DateTime? data, DateTime? dataPrevista, Int32? status, Int32 idAss)
        {
            List<PEDIDO_COMPRA> lista = new List<PEDIDO_COMPRA>();
            IQueryable<PEDIDO_COMPRA> query = Db.PEDIDO_COMPRA;
            if (usuaId != 0)
            {
                query = query.Where(p => p.USUA_CD_ID == usuaId);
            }
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.PECO_NM_NOME.Contains(nome));
            }
            if (!String.IsNullOrEmpty(numero))
            {
                query = query.Where(p => p.PECO_NR_NUMERO.Contains(numero));
            }
            if (!String.IsNullOrEmpty(nf))
            {
                query = query.Where(p => p.PECO_NR_NOTA_FISCAL.Contains(nf));
            }
            if (status != null && status != 0)
            {
                query = query.Where(p => p.PECO_IN_STATUS == status);
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.Where(p => p.PECO_IN_ATIVO == 1);
                query = query.OrderBy(a => a.PECO_DT_DATA).ThenBy(a => a.PECO_IN_STATUS);
                query = query.Include(p => p.USUARIO);
                lista = query.ToList<PEDIDO_COMPRA>();

                if (data != null)
                {
                    lista = lista.Where(p => p.PECO_DT_DATA.Value.Date == data.Value.Date).ToList();
                }
                if (dataPrevista != null)
                {
                    lista = lista.Where(p => p.PECO_DT_PREVISTA.Value.Date == dataPrevista.Value.Date).ToList();
                }
            }
            return lista;
        }

        public List<PEDIDO_COMPRA> ExecuteFilterDash(String nmr, DateTime? dtFinal, String nome, Int32? usu, Int32? status, Int32 idAss)
        {
            List<PEDIDO_COMPRA> lista = new List<PEDIDO_COMPRA>();
            IQueryable<PEDIDO_COMPRA> query = Db.PEDIDO_COMPRA;
            if (status != null)
            {
                query = query.Where(x => x.PECO_IN_STATUS == status);
            }
            if (!String.IsNullOrEmpty(nmr))
            {
                query = query.Where(p => p.PECO_NR_NUMERO == nmr);
            }
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.PECO_NM_NOME.Contains(nome));
            }
            if (usu != null && usu != 0)
            {
                query = query.Where(p => p.USUA_CD_ID == usu);
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.Where(p => p.PECO_IN_ATIVO == 1);
                query = query.OrderBy(a => a.PECO_DT_DATA).ThenBy(a => a.PECO_IN_STATUS);
                query = query.Include(p => p.USUARIO);
                lista = query.ToList<PEDIDO_COMPRA>();

                if (dtFinal != null)
                {
                    lista = lista.Where(x => x.PECO_DT_FINAL == dtFinal).ToList<PEDIDO_COMPRA>();
                }
                if (status == null)
                {
                    lista = lista.Where(p => p.PECO_DT_PREVISTA < DateTime.Today.Date && p.PECO_IN_STATUS != 7 && p.PECO_IN_STATUS != 8).ToList<PEDIDO_COMPRA>();
                }
            }
            return lista;
        }
    }
}
