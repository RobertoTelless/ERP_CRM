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
    public class OrdemServicoRepository: RepositoryBase<ORDEM_SERVICO>, IOrdemServicoRepository
    {
        public ORDEM_SERVICO CheckExist(ORDEM_SERVICO conta, Int32 idAss)
        {
            IQueryable<ORDEM_SERVICO> query = Db.ORDEM_SERVICO;
            query = query.Where(x => x.ASSI_CD_ID == idAss);
            query = query.Where(x => x.ATEN_CD_ID == conta.ATEN_CD_ID);
            query = query.Where(x => x.CLIE_CD_ID == conta.CLIE_CD_ID);
            query = query.Where(x => x.SERV_CD_ID == conta.SERV_CD_ID);
            query = query.Where(x => x.PROD_CD_ID == conta.PROD_CD_ID);
            return query.FirstOrDefault();
        }

        public ORDEM_SERVICO GetItemById(Int32 id)
        {
            IQueryable<ORDEM_SERVICO> query = Db.ORDEM_SERVICO;
            query = query.Where(x => x.ORSE_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<ORDEM_SERVICO> GetAllItens(Int32 idAss)
        {
            IQueryable<ORDEM_SERVICO> query = Db.ORDEM_SERVICO.Where(x => x.ORSE_IN_ATIVO == 1);
            query = query.Where(x => x.ASSI_CD_ID == idAss);
            query = query.Include(x => x.ORDEM_SERVICO_PRODUTO);
            query = query.Include(x => x.ORDEM_SERVICO_SERVICO);
            return query.ToList<ORDEM_SERVICO>();
        }

        public List<ORDEM_SERVICO> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<ORDEM_SERVICO> query = Db.ORDEM_SERVICO;
            query = query.Where(x => x.ASSI_CD_ID == idAss);
            return query.ToList<ORDEM_SERVICO>();
        }

        public List<ORDEM_SERVICO> ExecuteFilter(Int32? catOS, Int32? idClie, Int32? idUsu, DateTime? dtCriacao, Int32? status, Int32? idDept, Int32? idServ, Int32? idProd, Int32? idAten, Int32? idAss)
        {
            List<ORDEM_SERVICO> lista = new List<ORDEM_SERVICO>();
            IQueryable<ORDEM_SERVICO> query = Db.ORDEM_SERVICO.Where(x => x.ORSE_IN_ATIVO == 1);
            if (catOS != null)
            {
                query = query.Where(x => x.CAOS_CD_ID == catOS);
            }

            if (idClie != null)
            {
                query = query.Where(x => x.CLIE_CD_ID == idClie);
            }

            if (idUsu != null)
            {
                query = query.Where(x => x.USUA_CD_ID == idUsu);
            }

            if (status != 0)
            {
                query = query.Where(x => x.ORSE_IN_STATUS == status);
            }

            if (idDept != null)
            {
                query = query.Where(x => x.DEPT_CD_ID == idDept);
            }

            if (idServ != null)
            {
                query = query.Where(x => x.SERV_CD_ID == idServ);
            }

            if (idProd != null)
            {
                query = query.Where(x => x.PROD_CD_ID == idProd);
            }

            if (idAten != null)
            {
                query = query.Where(x => x.ATEN_CD_ID == idAten);
            }

            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderByDescending(a => a.ORSE_DT_INICIO);
                lista = query.ToList<ORDEM_SERVICO>();

                if (dtCriacao != DateTime.MinValue)
                {
                    lista = lista.Where(x => x.ORSE_DT_CRIACAO.Date == dtCriacao.Value.Date).ToList<ORDEM_SERVICO>();
                }
            }

            return lista;
        }
    }
}
