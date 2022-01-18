using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class FichaTecnicaRepository : RepositoryBase<FICHA_TECNICA>, IFichaTecnicaRepository
    {
        public FICHA_TECNICA CheckExist(FICHA_TECNICA conta, Int32 idAss)
        {
            IQueryable<FICHA_TECNICA> query = Db.FICHA_TECNICA;
            query = query.Where(p => p.FITE_NM_NOME == conta.FITE_NM_NOME);
            query = query.Where(p => p.PROD_CD_ID == conta.PROD_CD_ID);
            query = query.Where(p => p.ASSI_CD_ID == conta.ASSI_CD_ID);
            return query.FirstOrDefault();
        }

        public FICHA_TECNICA GetByNome(String nome, Int32 idAss)
        {
            IQueryable<FICHA_TECNICA> query = Db.FICHA_TECNICA.Where(p => p.FITE_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.FITE_NM_NOME == nome);
            query = query.Include(p => p.FICHA_TECNICA_DETALHE);
            query = query.Include(p => p.PRODUTO);
            return query.FirstOrDefault();
        }

        public FICHA_TECNICA GetItemById(Int32 id)
        {
            IQueryable<FICHA_TECNICA> query = Db.FICHA_TECNICA;
            query = query.Where(p => p.FITE_CD_ID == id);
            query = query.Include(p => p.FICHA_TECNICA_DETALHE);
            query = query.Include(p => p.PRODUTO);
            return query.FirstOrDefault();
        }

        public List<FICHA_TECNICA> GetAllItens(Int32 idAss)
        {
            IQueryable<FICHA_TECNICA> query = Db.FICHA_TECNICA.Where(p => p.FITE_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<FICHA_TECNICA> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<FICHA_TECNICA> query = Db.FICHA_TECNICA;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<FICHA_TECNICA> ExecuteFilter(Int32? prodId, Int32? cat, String descricao, Int32 idAss)
        {
            List<FICHA_TECNICA> lista = new List<FICHA_TECNICA>();
            IQueryable<FICHA_TECNICA> query = Db.FICHA_TECNICA.Where(x => x.PRODUTO.PROD_IN_COMPOSTO == 1);
            if (prodId > 0)
            {
                query = query.Where(p => p.PRODUTO.PROD_CD_ID == prodId);
            }
            if (cat != null)
            {
                query = query.Where(p => p.PRODUTO.CAPR_CD_ID == cat);
            }
            if (!String.IsNullOrEmpty(descricao))
            {
                query = query.Where(p => p.FITE_S_DESCRICAO.Contains(descricao));
            }

            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderBy(a => a.FITE_NM_NOME);
                lista = query.ToList<FICHA_TECNICA>();
            }
            return lista;
        }
    }
}
