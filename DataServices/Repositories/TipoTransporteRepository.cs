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
    public class TipoTransporteRepository : RepositoryBase<TIPO_TRANSPORTE>, ITipoTransporteRepository
    {
        public TIPO_TRANSPORTE GetItemById(Int32 id)
        {
            IQueryable<TIPO_TRANSPORTE> query = Db.TIPO_TRANSPORTE;
            query = query.Where(p => p.TITR_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<TIPO_TRANSPORTE> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<TIPO_TRANSPORTE> query = Db.TIPO_TRANSPORTE;
            return query.ToList();
        }

        public List<TIPO_TRANSPORTE> GetAllItens(Int32 idAss)
        {
            IQueryable<TIPO_TRANSPORTE> query = Db.TIPO_TRANSPORTE.Where(p => p.TITR_IN_ATIVO == 1);
            return query.ToList();
        }
    }
}
 