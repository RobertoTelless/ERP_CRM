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
    public class ContaPagarRepository : RepositoryBase<CONTA_PAGAR>, IContaPagarRepository
    {
        public CONTA_PAGAR GetItemById(Int32 id)
        {
            IQueryable<CONTA_PAGAR> query = Db.CONTA_PAGAR;
            query = query.Where(p => p.CAPA_CD_ID == id);
            query = query.Include(p => p.FORNECEDOR);
            query = query.Include(p => p.CONTA_BANCO);
            query = query.Include(p => p.CONTA_PAGAR_ANEXO);
            query = query.Include(p => p.CONTA_PAGAR_PARCELA);
            query = query.Include(p => p.CONTA_PAGAR_RATEIO);
            query = query.Include(p => p.USUARIO);
            query = query.Include(p => p.CONTA_PAGAR_PARCELA);
            query = query.Include(p => p.CONTA_PAGAR_TAG);
            return query.FirstOrDefault();
        }

        public List<CONTA_PAGAR> GetItensAtraso(Int32 idAss)
        {
            IQueryable<CONTA_PAGAR> query = Db.CONTA_PAGAR.Where(p => p.CAPA_IN_ATIVO == 1);
            query = query.Where(p => DbFunctions.TruncateTime(p.CAPA_DT_VENCIMENTO) < DateTime.Today.Date);
            query = query.Where(p => p.CAPA_NR_ATRASO > 0);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public Decimal GetTotalPagoMes(DateTime mes, Int32 idAss)
        {
            IQueryable<CONTA_PAGAR> query = Db.CONTA_PAGAR.Where(p => p.CAPA_IN_ATIVO == 1);
            query = query.Where(p => DbFunctions.TruncateTime(p.CAPA_DT_LIQUIDACAO).Value.Month == mes.Month);
            query = query.Where(p => p.CAPA_IN_LIQUIDADA == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            List<CONTA_PAGAR> lista = query.ToList();
            Decimal soma = lista.Sum(p => p.CAPA_VL_VALOR_PAGO).Value;
            return soma;
        }

        public Decimal GetTotalAPagarMes(DateTime mes, Int32 idAss)
        {
            IQueryable<CONTA_PAGAR> query = Db.CONTA_PAGAR.Where(p => p.CAPA_IN_ATIVO == 1);
            query = query.Where(p => DbFunctions.TruncateTime(p.CAPA_DT_VENCIMENTO).Value.Month == mes.Month);
            query = query.Where(p => p.CAPA_IN_LIQUIDADA == 0);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            List<CONTA_PAGAR> lista = query.ToList();
            Decimal soma = lista.Sum(p => p.CAPA_VL_VALOR).Value;
            return soma;
        }

        public List<CONTA_PAGAR> GetPagamentosMes(DateTime mes, Int32 idAss)
        {
            IQueryable<CONTA_PAGAR> query = Db.CONTA_PAGAR.Where(p => p.CAPA_IN_ATIVO == 1);
            query = query.Where(p => DbFunctions.TruncateTime(p.CAPA_DT_LIQUIDACAO).Value.Month == mes.Month);
            query = query.Where(p => p.CAPA_IN_LIQUIDADA == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<CONTA_PAGAR> GetAPagarMes(DateTime mes, Int32 idAss)
        {
            IQueryable<CONTA_PAGAR> query = Db.CONTA_PAGAR.Where(p => p.CAPA_IN_ATIVO == 1);
            query = query.Where(p => DbFunctions.TruncateTime(p.CAPA_DT_VENCIMENTO).Value.Month == mes.Month);
            query = query.Where(p => p.CAPA_IN_LIQUIDADA == 0);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<CONTA_PAGAR> GetAllItens(Int32 idAss)
        {
            IQueryable<CONTA_PAGAR> query = Db.CONTA_PAGAR.Where(p => p.CAPA_IN_ATIVO == 1);
            query = query.Include(p => p.CENTRO_CUSTO);
            query = query.Include(p => p.FORNECEDOR);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<CONTA_PAGAR> GetItensAtrasoFornecedor(Int32 idAss)
        {
            IQueryable<CONTA_PAGAR> query = Db.CONTA_PAGAR.Where(p => p.CAPA_IN_ATIVO == 1);
            query = query.Where(p => DbFunctions.TruncateTime(p.CAPA_DT_VENCIMENTO) < DateTime.Today.Date);
            query = query.Where(p => p.CAPA_NR_ATRASO > 0);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Include(p => p.FORNECEDOR);
            query = query.Include(p => p.CONTA_BANCO);
            query = query.Include(p => p.CONTA_PAGAR_ANEXO);
            query = query.Include(p => p.CONTA_PAGAR_RATEIO);
            query = query.Include(p => p.TIPO_FAVORECIDO);
            query = query.Include(p => p.USUARIO);
            query = query.Include(p => p.CONTA_PAGAR_PARCELA);
            query = query.Include(p => p.CONTA_PAGAR_TAG);
            return query.ToList();
        }

        public List<CONTA_PAGAR> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<CONTA_PAGAR> query = Db.CONTA_PAGAR;
            query = query.Include(p => p.CENTRO_CUSTO);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Include(p => p.FORNECEDOR);
            return query.ToList();
        }

        public List<CONTA_PAGAR> ExecuteFilter(Int32? forId, Int32? ccId, DateTime? data, String descricao, Int32? aberto, DateTime? vencimento, DateTime? vencFinal, DateTime? quitacao, Int32? atraso, Int32? conta, Int32 idAss)
        {
            List<CONTA_PAGAR> lista = new List<CONTA_PAGAR>();
            IQueryable<CONTA_PAGAR> query = Db.CONTA_PAGAR;
            if (forId != null)
            {
                query = query.Where(p => p.FORN_CD_ID == forId);
            }
            if (ccId != null)
            {
                query = query.Where(p => p.CECU_CD_ID == ccId);
            }
            if (data != null)
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.CAPA_DT_LANCAMENTO) == DbFunctions.TruncateTime(data));
            }
            if (aberto == 1)
            {
                query = query.Where(p => p.CAPA_IN_LIQUIDADA == 0);
            }
            else if (aberto == 2)
            {
                query = query.Where(p => p.CAPA_IN_LIQUIDADA == 1);
            }
            if (!String.IsNullOrEmpty(descricao))
            {
                query = query.Where(p => p.CAPA_DS_DESCRICAO.Contains(descricao));
            }
            if (vencimento != null)
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.CAPA_DT_VENCIMENTO) >= DbFunctions.TruncateTime(vencimento));
            }
            if (vencFinal != null)
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.CAPA_DT_VENCIMENTO) <= DbFunctions.TruncateTime(vencFinal));
            }
            if (quitacao != null)
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.CAPA_DT_LIQUIDACAO) == DbFunctions.TruncateTime(quitacao));
            }
            if (conta != null)
            {
                query = query.Where(p => p.FORMA_PAGAMENTO.COBA_CD_ID == conta);
            }
            if (atraso != null)
            {
                if (atraso == 1)
                {
                    query = query.Where(p => p.CAPA_NR_ATRASO != null && p.CAPA_NR_ATRASO > 0);
                }
                else if (atraso == 2)
                {
                    query = query.Where(p => p.CAPA_NR_ATRASO == null || p.CAPA_NR_ATRASO == 0);
                }
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.Where(p => p.CAPA_IN_ATIVO == 1);
                query = query.Include(p => p.CENTRO_CUSTO);
                query = query.Include(p => p.FORNECEDOR);
                query = query.OrderBy(a => a.CAPA_DT_VENCIMENTO);
                lista = query.ToList<CONTA_PAGAR>();
            }
            return lista;
        }

        public List<CONTA_PAGAR> ExecuteFilterAtraso(String nome, DateTime? vencimento, Int32 idAss)
        {
            List<CONTA_PAGAR> lista = new List<CONTA_PAGAR>();
            IQueryable<CONTA_PAGAR> query = Db.CONTA_PAGAR;
            if (nome != null)
            {
                query = query.Where(x => x.FORNECEDOR.FORN_NM_NOME.Contains(nome));
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.Where(p => p.CAPA_IN_ATIVO == 1);
                lista = query.ToList<CONTA_PAGAR>();

                if (vencimento != null)
                {
                    lista = lista.Where(x => x.CAPA_DT_VENCIMENTO == vencimento).ToList();
                }

                lista = lista.GroupBy(x => x.FORN_CD_ID).Select(x => x.First()).ToList();
            }
            return lista;
        }

    }
}
 