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
    public class ContaReceberRepository : RepositoryBase<CONTA_RECEBER>, IContaReceberRepository
    {
        public CONTA_RECEBER GetItemById(Int32 id)
        {
            IQueryable<CONTA_RECEBER> query = Db.CONTA_RECEBER;
            query = query.Where(p => p.CARE_CD_ID == id);
            query = query.Include(p => p.CLIENTE);
            query = query.Include(p => p.CONTA_BANCO);
            query = query.Include(p => p.CONTA_RECEBER_ANEXO);
            query = query.Include(p => p.FILIAL);
            query = query.Include(p => p.TIPO_FAVORECIDO);
            query = query.Include(p => p.USUARIO);
            query = query.Include(p => p.CONTA_RECEBER_PARCELA);
            query = query.Include(p => p.CONTA_RECEBER_TAG);
            return query.FirstOrDefault();
        }

        public Decimal GetTotalRecebimentosMes(DateTime mes, Int32 idAss)
        {
            IQueryable<CONTA_RECEBER> query = Db.CONTA_RECEBER.Where(p => p.CARE_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => DbFunctions.TruncateTime(p.CARE_DT_DATA_LIQUIDACAO).Value.Month == mes.Month);
            query = query.Where(p => p.CARE_IN_LIQUIDADA == 1);
            List<CONTA_RECEBER> lista = query.ToList();
            Decimal soma = lista.Sum(p => p.CARE_VL_VALOR_LIQUIDADO).Value;
            return soma;
        }

        public List<CONTA_RECEBER> GetRecebimentosMes(DateTime mes, Int32 idAss)
        {
            IQueryable<CONTA_RECEBER> query = Db.CONTA_RECEBER.Where(p => p.CARE_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => DbFunctions.TruncateTime(p.CARE_DT_DATA_LIQUIDACAO).Value.Month == mes.Month);
            query = query.Where(p => p.CARE_IN_LIQUIDADA == 1);
            return query.ToList();
        }

        public Decimal GetTotalAReceberMes(DateTime mes, Int32 idAss)
        {
            IQueryable<CONTA_RECEBER> query = Db.CONTA_RECEBER.Where(p => p.CARE_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => DbFunctions.TruncateTime(p.CARE_DT_VENCIMENTO).Value.Month == mes.Month);
            query = query.Where(p => p.CARE_IN_LIQUIDADA == 0);
            List<CONTA_RECEBER> lista = query.ToList();
            Decimal soma = lista.Sum(p => p.CARE_VL_VALOR);
            return soma;
        }

        public List<CONTA_RECEBER> GetAReceberMes(DateTime mes, Int32 idAss)
        {
            IQueryable<CONTA_RECEBER> query = Db.CONTA_RECEBER.Where(p => p.CARE_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => DbFunctions.TruncateTime(p.CARE_DT_VENCIMENTO).Value.Month == mes.Month);
            query = query.Where(p => p.CARE_IN_LIQUIDADA == 0);
            return query.ToList();
        }

        public List<CONTA_RECEBER> GetAllItens(Int32 idAss)
        {
            IQueryable<CONTA_RECEBER> query = Db.CONTA_RECEBER.Where(p => p.CARE_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<CONTA_RECEBER> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<CONTA_RECEBER> query = Db.CONTA_RECEBER;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<CONTA_RECEBER> GetVencimentoAtual(Int32 idAss)
        {
            IQueryable<CONTA_RECEBER> query = Db.CONTA_RECEBER.Where(p => p.CARE_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);

            List<CONTA_RECEBER> lista = query.ToList<CONTA_RECEBER>();
            lista = lista.Where(x => x.CARE_DT_VENCIMENTO != null && (x.CARE_DT_VENCIMENTO.Value.Date == DateTime.Now.Date || x.CONTA_RECEBER_PARCELA.Any(a => a.CRPA_DT_VENCIMENTO.Value.Day == DateTime.Now.Day))).ToList<CONTA_RECEBER>();

            if (lista == null || lista.Count == 0)
            {
                lista = GetAllItens(idAss);
            }

            return lista;
        }

        public List<CONTA_RECEBER> GetItensAtrasoCliente(Int32 idAss)
        {
            IQueryable<CONTA_RECEBER> query = Db.CONTA_RECEBER.Where(p => p.CARE_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => DbFunctions.TruncateTime(p.CARE_DT_VENCIMENTO) < DateTime.Today.Date);
            query = query.Where(p => p.CARE_NR_ATRASO > 0);
            return query.ToList();
        }

        public List<CONTA_RECEBER> ExecuteFilter(Int32? cliId, Int32? ccId, DateTime? dtLanc, DateTime? data, DateTime? dataFinal, String descricao, Int32? aberto, Int32? conta, Int32 idAss)
        {
            List<CONTA_RECEBER> lista = new List<CONTA_RECEBER>();
            IQueryable<CONTA_RECEBER> query = Db.CONTA_RECEBER;
            if (cliId != null)
            {
                query = query.Where(p => p.CLIE_CD_ID == cliId);
            }
            if (ccId != null)
            {
                query = query.Where(p => p.CECU_CD_ID == ccId);
            }
            if (!String.IsNullOrEmpty(descricao))
            {
                query = query.Where(p => p.CARE_DS_DESCRICAO.Contains(descricao));
            }
            if (aberto != null)
            {
                query = query.Where(p => p.CARE_IN_LIQUIDADA == 0);
            }
            if (conta != null)
            {
                query = query.Where(p => p.FORMA_PAGAMENTO.COBA_CD_ID == conta);
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderBy(a => a.CARE_DT_VENCIMENTO);
                lista = query.ToList<CONTA_RECEBER>();

                if (dtLanc != null)
                {
                    lista = lista.Where(p => p.CARE_DT_LANCAMENTO == dtLanc).ToList<CONTA_RECEBER>();
                }
                if (data != null)
                {
                    lista = lista.Where(p => p.CARE_DT_VENCIMENTO >= data).ToList<CONTA_RECEBER>();
                }
                if (dataFinal != null)
                {
                    lista = lista.Where(p => p.CARE_DT_VENCIMENTO <= dataFinal).ToList<CONTA_RECEBER>();
                }
            }
            return lista;
        }

        public List<CONTA_RECEBER> ExecuteFilterRecebimentoMes(Int32? clieId, Int32? ccId, String desc, DateTime? emissao, DateTime? venc, DateTime? liqui, Int32 idAss)
        {
            List<CONTA_RECEBER> lista = new List<CONTA_RECEBER>();
            IQueryable<CONTA_RECEBER> query = Db.CONTA_RECEBER;
            if (clieId != null)
            {
                query = query.Where(p => p.CLIE_CD_ID == clieId);
            }
            if (ccId != null)
            {
                query = query.Where(p => p.CECU_CD_ID == ccId);
            }
            if (desc != null)
            {
                query = query.Where(p => p.CARE_DS_DESCRICAO == desc);
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderBy(a => a.CARE_DT_VENCIMENTO);
                lista = query.ToList<CONTA_RECEBER>();

                lista = lista.Where(p => p.CARE_IN_LIQUIDADA == 1 && p.CARE_DT_DATA_LIQUIDACAO != null && p.CARE_DT_DATA_LIQUIDACAO.Value.Month == DateTime.Today.Date.Month).ToList<CONTA_RECEBER>();

                if (emissao != null)
                {
                    lista = lista.Where(p => p.CARE_DT_LANCAMENTO == emissao).ToList<CONTA_RECEBER>();
                }
                if (venc != null)
                {
                    lista = lista.Where(p => p.CARE_DT_VENCIMENTO == venc).ToList<CONTA_RECEBER>();
                }
                if (liqui != null)
                {
                    lista = lista.Where(p => p.CARE_DT_DATA_LIQUIDACAO == liqui).ToList<CONTA_RECEBER>();
                }
            }
            return lista;
        }

        public List<CONTA_RECEBER> ExecuteFilterAReceberMes(Int32? clieId, Int32? ccId, String desc, DateTime? emissao, DateTime? venc, Int32 idAss)
        {
            List<CONTA_RECEBER> lista = new List<CONTA_RECEBER>();
            IQueryable<CONTA_RECEBER> query = Db.CONTA_RECEBER;
            if (clieId != null)
            {
                query = query.Where(p => p.CLIE_CD_ID == clieId);
            }
            if (ccId != null)
            {
                query = query.Where(p => p.CECU_CD_ID == ccId);
            }
            if (desc != null)
            {
                query = query.Where(p => p.CARE_DS_DESCRICAO == desc);
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderBy(a => a.CARE_DT_VENCIMENTO);
                lista = query.ToList<CONTA_RECEBER>();

                lista = lista.Where(p => p.CARE_IN_LIQUIDADA == 0 && p.CARE_DT_VENCIMENTO.Value.Month == DateTime.Today.Date.Month).ToList<CONTA_RECEBER>();

                if (emissao != null)
                {
                    lista = lista.Where(p => p.CARE_DT_LANCAMENTO == emissao).ToList<CONTA_RECEBER>();
                }
                if (venc != null)
                {
                    lista = lista.Where(p => p.CARE_DT_VENCIMENTO == venc).ToList<CONTA_RECEBER>();
                }
            }
            return lista;
        }

        public List<CONTA_RECEBER> ExecuteFilterCRAtrasos(Int32? clieId, Int32? ccId, String desc, DateTime? emissao, DateTime? venc, Int32 idAss)
        {
            List<CONTA_RECEBER> lista = new List<CONTA_RECEBER>();
            IQueryable<CONTA_RECEBER> query = Db.CONTA_RECEBER;
            if (clieId != null)
            {
                query = query.Where(p => p.CLIE_CD_ID == clieId);
            }
            if (ccId != null)
            {
                query = query.Where(p => p.CECU_CD_ID == ccId);
            }
            if (desc != null)
            {
                query = query.Where(p => p.CARE_DS_DESCRICAO == desc);
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderBy(a => a.CARE_DT_VENCIMENTO);
                lista = query.ToList<CONTA_RECEBER>();

                lista = lista.Where(p => p.CARE_NR_ATRASO > 0 && p.CARE_DT_VENCIMENTO < DateTime.Today.Date).ToList<CONTA_RECEBER>();

                if (emissao != null)
                {
                    lista = lista.Where(p => p.CARE_DT_LANCAMENTO == emissao).ToList<CONTA_RECEBER>();
                }
                if (venc != null)
                {
                    lista = lista.Where(p => p.CARE_DT_VENCIMENTO == venc).ToList<CONTA_RECEBER>();
                }
            }
            return lista;
        }

        public List<CONTA_RECEBER> ExecuteFilterAtrasos(String nome, String cidade, Int32? uf, Int32 idAss)
        {
            List<CONTA_RECEBER> lista = new List<CONTA_RECEBER>();
            IQueryable<CONTA_RECEBER> query = Db.CONTA_RECEBER.Where(x => x.CARE_IN_ATIVO == 1);
            if (nome != null)
            {
                query = query.Where(p => p.CLIENTE.CLIE_NM_NOME.Contains(nome));
            }
            if (cidade != null)
            {
                query = query.Where(p => p.CLIENTE.CLIE_NM_CIDADE.Contains(cidade));
            }
            if (uf != null)
            {
                query = query.Where(p => p.CLIENTE.UF_CD_ID == uf);
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                lista = query.ToList<CONTA_RECEBER>();
                lista = lista.Where(p => p.CARE_DT_VENCIMENTO < DateTime.Now).ToList();
                lista = lista.GroupBy(prop => prop.CLIE_CD_ID).Select(prop => prop.First()).ToList();
            }
            return lista;
        }
    }
}
 