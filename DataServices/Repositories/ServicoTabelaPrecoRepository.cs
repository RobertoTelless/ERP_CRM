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
    public class ServicoTabelaPrecoRepository : RepositoryBase<SERVICO_TABELA_PRECO>, IServicoTabelaPrecoRepository
    {
        public SERVICO_TABELA_PRECO CheckExist(Int32 fili, Int32 servico, Int32 idAss)
        {
            IQueryable<SERVICO_TABELA_PRECO> query = Db.SERVICO_TABELA_PRECO;
            query = query.Where(x => x.FILI_CD_ID == fili);
            query = query.Where(x => x.SERV_CD_ID == servico);
            return query.FirstOrDefault();
        }

        public SERVICO_TABELA_PRECO GetByServFilial(Int32 id, Int32 fili, Int32 idAss)
        {
            IQueryable<SERVICO_TABELA_PRECO> query = Db.SERVICO_TABELA_PRECO;
            query = query.Where(x => x.FILI_CD_ID == fili);
            query = query.Where(x => x.SERV_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
