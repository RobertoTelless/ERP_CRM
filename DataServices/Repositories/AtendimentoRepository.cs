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
using CrossCutting;

namespace DataServices.Repositories
{
    public class AtendimentoRepository : RepositoryBase<ATENDIMENTO>, IAtendimentoRepository
    {
        public ATENDIMENTO CheckExist(ATENDIMENTO conta)
        {
            Int32? idAss = SessionMocks.IdAssinante;
            IQueryable<ATENDIMENTO> query = Db.ATENDIMENTO;
            query = query.Where(p => p.CLIE_CD_ID == conta.CLIE_CD_ID);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.ATEN_DT_INICIO == conta.ATEN_DT_INICIO);
            query = query.Where(p => p.ATEN_NR_NUMERO == conta.ATEN_NR_NUMERO);
            return query.FirstOrDefault();
        }

        public ATENDIMENTO GetItemById(Int32 id)
        {
            IQueryable<ATENDIMENTO> query = Db.ATENDIMENTO;
            query = query.Where(p => p.ATEN_CD_ID == id);
            query = query.Include(p => p.ORDEM_SERVICO);
            return query.FirstOrDefault();
        }

        public List<ATENDIMENTO> GetAllItens()
        {
            Int32? idAss = SessionMocks.IdAssinante;
            IQueryable<ATENDIMENTO> query = Db.ATENDIMENTO.Where(p => p.ATEN_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<ATENDIMENTO> GetByCliente(Int32 id)
        {
            Int32? idAss = SessionMocks.IdAssinante;
            IQueryable<ATENDIMENTO> query = Db.ATENDIMENTO.Where(p => p.ATEN_IN_ATIVO == 1);
            query = query.Where(p => p.CLIE_CD_ID == id);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<ATENDIMENTO> GetAllItensAdm()
        {
            Int32? idAss = SessionMocks.IdAssinante;
            IQueryable<ATENDIMENTO> query = Db.ATENDIMENTO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<ATENDIMENTO> ExecuteFilter(Int32? idCat, Int32? cliente, Int32? produto, DateTime? data, Int32? status, String descricao, Int32? depto, Int32? prioridade, Int32? idUsua, Int32? idServico, Int32? sla)
        {
            Int32? idAss = SessionMocks.IdAssinante;
            List<ATENDIMENTO> lista = new List<ATENDIMENTO>();
            IQueryable<ATENDIMENTO> query = Db.ATENDIMENTO;
            if (cliente != null)
            {
                query = query.Where(p => p.CLIE_CD_ID == cliente);
            }
            if (produto != null)
            {
                query = query.Where(p => p.PROD_CD_ID == produto);
            }
            if (idCat > 0)
            {
                query = query.Where(p => p.CAAT_CD_ID == idCat);
            }
            if (idUsua > 0)
            {
                query = query.Where(p => p.USUA_CD_ID == idUsua);
            }
            if (idServico != null)
            {
                query = query.Where(p => p.SERV_CD_ID == idServico);
            }
            if (status != 0)
            {
                query = query.Where(p => p.ATEN_IN_STATUS == status);
            }
            if (!String.IsNullOrEmpty(descricao))
            {
                query = query.Where(p => p.ATEN_NM_ASSUNTO.Contains(descricao));
            }
            if (depto != null)
            {
                query = query.Where(p => p.DEPT_CD_ID == depto);
            }
            if (prioridade != null)
            {
                query = query.Where(p => p.ATEN_IN_PRIORIDADE == prioridade);
            }
            if (sla != null)
            {
                if (sla == 1)
                {
                    query = query.Where(x => x.ATEN_DT_PREVISTA != null && DbFunctions.TruncateTime(DateTime.Now) <= DbFunctions.TruncateTime(x.ATEN_DT_PREVISTA.Value));
                }
                else if (sla == 0)
                {
                    query = query.Where(x => x.ATEN_DT_PREVISTA != null && DbFunctions.TruncateTime(DateTime.Now) > DbFunctions.TruncateTime(x.ATEN_DT_PREVISTA.Value));
                }
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderByDescending(a => a.ATEN_DT_INICIO);
                lista = query.ToList<ATENDIMENTO>();

                if (data != null)
                {
                    lista = lista.Where(p => p.ATEN_DT_INICIO != null && p.ATEN_DT_INICIO.Value.Date == data.Value.Date).ToList<ATENDIMENTO>();
                }
            }
            return lista;
        }
    }
}
 