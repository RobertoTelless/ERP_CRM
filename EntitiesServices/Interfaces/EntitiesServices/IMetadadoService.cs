using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IMetadadoService : IServiceBase<METADADO>
    {
        Int32 Create(METADADO perfil, LOG log);
        Int32 Create(METADADO perfil);
        Int32 Edit(METADADO perfil, LOG log);
        Int32 Edit(METADADO perfil);
        Int32 Delete(METADADO perfil, LOG log);

        METADADO CheckExist(METADADO item, Int32 idAss);
        METADADO GetBySigla(String sigla);
        METADADO GetItemById(Int32 id);
        List<METADADO> GetAllItens(Int32 idAss);
        List<METADADO> GetAllItensAdm(Int32 idAss);
        List<METADADO> ExecuteFilter(Int32? tipo, String nome, String descricao, String sigla, Int32 idAss);

        List<TIPO_METADADO> GetAllTipos(Int32 idAss);
    }
}
