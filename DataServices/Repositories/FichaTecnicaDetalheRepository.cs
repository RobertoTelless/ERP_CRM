using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class FichaTecnicaDetalheRepository : RepositoryBase<FICHA_TECNICA_DETALHE>, IFichaTecnicaDetalheRepository
    {
        public List<FICHA_TECNICA_DETALHE> GetAllItens()
        {
            return Db.FICHA_TECNICA_DETALHE.ToList();
        }

        public FICHA_TECNICA_DETALHE GetItemById(Int32 id)
        {
            IQueryable<FICHA_TECNICA_DETALHE> query = Db.FICHA_TECNICA_DETALHE.Where(p => p.FITD_CD_ID == id);
            return query.FirstOrDefault();
        }

    }
}
