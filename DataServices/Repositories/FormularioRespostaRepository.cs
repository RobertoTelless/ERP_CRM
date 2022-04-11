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
    public class FormularioRespostaRepository : RepositoryBase<FORMULARIO_RESPOSTA>, IFormularioRespostaRepository
    {
        public FORMULARIO_RESPOSTA GetItemById(Int32 id)
        {
            IQueryable<FORMULARIO_RESPOSTA> query = Db.FORMULARIO_RESPOSTA;
            query = query.Where(p => p.FORE_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<FORMULARIO_RESPOSTA> GetAllItens()
        {
            IQueryable<FORMULARIO_RESPOSTA> query = Db.FORMULARIO_RESPOSTA.Where(p => p.FORE_IN_ATIVO == 1);
            query = query.OrderBy(a => a.FORE_DT_CADASTRO);
            return query.ToList();
        }

        public List<FORMULARIO_RESPOSTA> ExecuteFilter(Int32? status, String nome, String email, String celular, String cidade, Int32? uf)
        {
            List<FORMULARIO_RESPOSTA> lista = new List<FORMULARIO_RESPOSTA>();
            IQueryable<FORMULARIO_RESPOSTA> query = Db.FORMULARIO_RESPOSTA;
            if (status > 0)
            {
                query = query.Where(p => p.FORE_IN_STATUS == status);
            }
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.FORE_NM_NOME.Contains(nome));
            }
            if (!String.IsNullOrEmpty(email))
            {
                query = query.Where(p => p.FORE_NM_EMAIL.Contains(email));
            }
            if (!String.IsNullOrEmpty(celular))
            {
                query = query.Where(p => p.FORE_NR_CELULAR.Contains(celular));
            }
            if (!String.IsNullOrEmpty(cidade))
            {
                query = query.Where(p => p.FORE_NM_CIDADE.Contains(cidade));
            }
            if (uf != null)
            {
                query = query.Where(p => p.UF_CD_ID == uf);
            }
            if (query != null)
            {
                query = query.OrderBy(a => a.FORE_NM_NOME);
                lista = query.ToList<FORMULARIO_RESPOSTA>();
            }
            return lista;
        }
    }
}
 