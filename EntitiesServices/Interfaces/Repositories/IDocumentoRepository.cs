using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.Repositories
{
    public interface IDocumentoRepository : IRepositoryBase<DOCUMENTO>
    {
        DOCUMENTO GetItemById(Int32 id);
        List<DOCUMENTO> GetAllItens(Int32 idAss);
        List<DOCUMENTO> GetAllItensAdm(Int32 idAss);
        List<DOCUMENTO> ExecuteFilter(Int32? grupo, Int32? classe, Int32? tipo, Int32? cliente, Int32? forn, String nome, String descricao, String numero, DateTime? data, Int32 idAss);
    }
}
