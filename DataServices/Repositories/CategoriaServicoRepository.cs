using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class CategoriaServicoRepository : RepositoryBase<CATEGORIA_SERVICO>, ICategoriaServicoRepository
    {
        public CATEGORIA_SERVICO CheckExist(CATEGORIA_SERVICO conta, Int32 idAss)
        {
            IQueryable<CATEGORIA_SERVICO> query = Db.CATEGORIA_SERVICO;
            query = query.Where(p => p.CASE_NM_NOME == conta.CASE_NM_NOME);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }
        
        public CATEGORIA_SERVICO GetItemById(Int32 id)
        {
            IQueryable<CATEGORIA_SERVICO> query = Db.CATEGORIA_SERVICO;
            query = query.Where(p => p.CASE_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<CATEGORIA_SERVICO> GetAllItens(Int32 idAss)
        {
            IQueryable<CATEGORIA_SERVICO> query = Db.CATEGORIA_SERVICO.Where(p => p.CASE_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<CATEGORIA_SERVICO> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<CATEGORIA_SERVICO> query = Db.CATEGORIA_SERVICO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }
    }
}
