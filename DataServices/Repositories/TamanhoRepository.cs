using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using EntitiesServices.Work_Classes;

namespace DataServices.Repositories
{
    public class TamanhoRepository : RepositoryBase<TAMANHO>, ITamanhoRepository
    {
        public TAMANHO CheckExist(TAMANHO conta, Int32 idAss)
        {
            IQueryable<TAMANHO> query = Db.TAMANHO;
            query = query.Where(p => p.TAMA_NM_NOME == conta.TAMA_NM_NOME);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public TAMANHO GetItemById(Int32 id)
        {
            IQueryable<TAMANHO> query = Db.TAMANHO;
            query = query.Where(p => p.TAMA_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<TAMANHO> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<TAMANHO> query = Db.TAMANHO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<TAMANHO> GetAllItens(Int32 idAss)
        {
            IQueryable<TAMANHO> query = Db.TAMANHO.Where(p => p.TAMA_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }
    }
}
 