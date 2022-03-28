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
    public class CRMComercialRepository : RepositoryBase<CRM_COMERCIAL>, ICRMComercialRepository
    {
        public CRM_COMERCIAL CheckExist(CRM_COMERCIAL tarefa, Int32 idUsu, Int32 idAss)
        {
            IQueryable<CRM_COMERCIAL> query = Db.CRM_COMERCIAL;
            query = query.Where(p => p.CRMC_NM_NOME == tarefa.CRMC_NM_NOME);
            query = query.Where(p => p.USUA_CD_ID == idUsu);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public List<CRM_COMERCIAL> GetByDate(DateTime data, Int32 idAss)
        {
            IQueryable<CRM_COMERCIAL> query = Db.CRM_COMERCIAL.Where(p => p.CRMC_IN_ATIVO == 1);
            query = query.Where(p => p.CRMC_DT_CRIACAO == data);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<CRM_COMERCIAL> GetByUser(Int32 user)
        {
            IQueryable<CRM_COMERCIAL> query = Db.CRM_COMERCIAL.Where(p => p.CRMC_IN_ATIVO == 1);
            query = query.Where(p => p.USUA_CD_ID == user);
            return query.ToList();
        }

        public List<CRM_COMERCIAL> GetTarefaStatus(Int32 tipo, Int32 idAss)
        {
            IQueryable<CRM_COMERCIAL> query = Db.CRM_COMERCIAL.Where(p => p.CRMC_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.CRMC_IN_STATUS == tipo);
            return query.ToList();
        }

        public CRM_COMERCIAL GetItemById(Int32 id)
        {
            IQueryable<CRM_COMERCIAL> query = Db.CRM_COMERCIAL;
            query = query.Where(p => p.CRMC_CD_ID == id);
            query = query.Include(p => p.CRM_COMERCIAL_COMENTARIO_NOVA);
            query = query.Include(p => p.USUARIO);
            query = query.Include(p => p.CLIENTE);
            query = query.Include(p => p.CLIENTE.UF);
            return query.FirstOrDefault();
        }

        public List<CRM_COMERCIAL> GetAllItens(Int32 idUsu)
        {
            IQueryable<CRM_COMERCIAL> query = Db.CRM_COMERCIAL.Where(p => p.CRMC_IN_ATIVO == 1 || p.CRMC_IN_ATIVO == 5);
            query = query.Where(p => p.ASSI_CD_ID == idUsu);
            return query.ToList();
        }

        public List<CRM_COMERCIAL> GetAllItensAdm(Int32 idUsu)
        {
            IQueryable<CRM_COMERCIAL> query = Db.CRM_COMERCIAL;
            query = query.Where(p => p.ASSI_CD_ID == idUsu);
            return query.ToList();
        }

        public List<CRM_COMERCIAL> ExecuteFilter(Int32? status, DateTime? inicio, DateTime? final, Int32? origem, Int32? adic, String nome, String busca, Int32? estrela, Int32 idAss)
        {
            List<CRM_COMERCIAL> lista = new List<CRM_COMERCIAL>();
            IQueryable<CRM_COMERCIAL> query = Db.CRM_COMERCIAL;
            if (status > 0)
            {
                query = query.Where(p => p.CRMC_IN_STATUS == status);
            }
            if (origem != null)
            {
                query = query.Where(p => p.CROR_CD_ID == origem);
            }
            if (estrela != null)
            {
                query = query.Where(p => p.CRMC_IN_ESTRELA == estrela);
            }
            if (adic != null)
            {
                if (adic == 1)
                {
                    query = query.Where(p => p.CRMC_IN_ATIVO == 1);
                }
                else if (adic == 2)
                {
                    query = query.Where(p => p.CRMC_IN_ATIVO == 2);
                }
                else if (adic == 3)
                {
                    query = query.Where(p => p.CRMC_IN_ATIVO == 3);
                }
                else if (adic == 4)
                {
                    query = query.Where(p => p.CRMC_IN_ATIVO == 4);
                }
                else if (adic == 5)
                {
                    query = query.Where(p => p.CRMC_IN_ATIVO == 5);
                }
            }
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.CRMC_NM_NOME.Contains(nome) || p.CRMC_DS_DESCRICAO.Contains(nome));        
            }
            if (!String.IsNullOrEmpty(busca))
            {
                query = query.Where(p => p.CLIENTE.CLIE_NM_NOME.Contains(busca) || p.CLIENTE.CLIE_NM_RAZAO.Contains(busca) || p.CLIENTE.CLIE_NR_CPF.Contains(busca));
            }
            if (inicio != null)
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.CRM_COMERCIAL_ACAO.Where(m => m.CRCA_IN_ATIVO == 1).OrderByDescending(m => m.CRCA_DT_PREVISTA).FirstOrDefault().CRCA_DT_PREVISTA) >= DbFunctions.TruncateTime(inicio));
            }
            if (final != null)
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.CRM_COMERCIAL_ACAO.Where(m => m.CRCA_IN_ATIVO == 1).OrderByDescending(m => m.CRCA_DT_PREVISTA).FirstOrDefault().CRCA_DT_PREVISTA) <= DbFunctions.TruncateTime(final));
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderBy(a => a.CRMC_DT_CRIACAO);
                lista = query.ToList<CRM_COMERCIAL>();
            }
            return lista;
        }

        public List<CRM_COMERCIAL> GetAllItensAdmUser(Int32 id, Int32 idAss)
        {
            IQueryable<CRM_COMERCIAL> query = Db.CRM_COMERCIAL;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.USUA_CD_ID == id);
            query = query.Include(p => p.USUARIO);
            return query.ToList();
        }

        public List<CRM_COMERCIAL> GetAtrasados(Int32 idAss)
        {
            IQueryable<CRM_COMERCIAL> query = Db.CRM_COMERCIAL.Where(p => p.CRMC_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.CRMC_DT_PREVISTA < DateTime.Today.Date);
            query = query.Include(p => p.USUARIO);
            return query.ToList();
        }

        public List<CRM_COMERCIAL> GetEncerrados(Int32 idAss)
        {
            IQueryable<CRM_COMERCIAL> query = Db.CRM_COMERCIAL.Where(p => p.CRMC_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.CRMC_IN_STATUS == 5);
            query = query.Include(p => p.USUARIO);
            return query.ToList();
        }

        public List<CRM_COMERCIAL> GetCancelados(Int32 idAss)
        {
            IQueryable<CRM_COMERCIAL> query = Db.CRM_COMERCIAL.Where(p => p.CRMC_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.CRMC_IN_STATUS == 6);
            query = query.Include(p => p.USUARIO);
            return query.ToList();
        }

        public List<CRM_COMERCIAL> ExecuteFilterDash(String nmr, DateTime? dtFinal, String nome, Int32? usu, Int32? status, Int32 idAss)
        {
            List<CRM_COMERCIAL> lista = new List<CRM_COMERCIAL>();
            IQueryable<CRM_COMERCIAL> query = Db.CRM_COMERCIAL;
            if (status != null)
            {
                query = query.Where(x => x.CRMC_IN_STATUS == status);
            }
            if (!String.IsNullOrEmpty(nmr))
            {
                query = query.Where(p => p.CRMC_NR_NUMERO == nmr);
            }
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.CRMC_NM_NOME.Contains(nome));
            }
            if (usu != null && usu != 0)
            {
                query = query.Where(p => p.USUA_CD_ID == usu);
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.Where(p => p.CRMC_IN_ATIVO == 1);
                query = query.OrderBy(a => a.CRMC_DT_CRIACAO).ThenBy(a => a.CRMC_IN_STATUS);
                query = query.Include(p => p.USUARIO);
                lista = query.ToList<CRM_COMERCIAL>();

                if (dtFinal != null)
                {
                    lista = lista.Where(x => x.CRMC_DT_ENCERRAMENTO == dtFinal).ToList<CRM_COMERCIAL>();
                }
                if (status == null)
                {
                    lista = lista.Where(p => p.CRMC_DT_PREVISTA < DateTime.Today.Date && p.CRMC_IN_STATUS != 7 && p.CRMC_IN_STATUS != 8).ToList<CRM_COMERCIAL>();
                }
            }
            return lista;
        }

    }
}
 