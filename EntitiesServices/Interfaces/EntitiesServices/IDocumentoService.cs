using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IDocumentoService : IServiceBase<DOCUMENTO>
    {
        Int32 Create(DOCUMENTO perfil, LOG log);
        Int32 Create(DOCUMENTO perfil);
        Int32 Edit(DOCUMENTO perfil, LOG log);
        Int32 Edit(DOCUMENTO perfil);
        Int32 Delete(DOCUMENTO perfil, LOG log);

        DOCUMENTO GetItemById(Int32 id);
        List<DOCUMENTO> GetAllItens(Int32 idAss);
        List<DOCUMENTO> GetAllItensAdm(Int32 idAss);
        List<DOCUMENTO> ExecuteFilter(Int32? grupo, Int32? classe, Int32? tipo, Int32? cliente, Int32? forn, String nome, String descricao, String numero, DateTime? data, Int32 idAss);

        List<GRUPO_DOCUMENTO> GetAllGrupos(Int32 idAss);
        List<TIPO_DOCUMENTO> GetAllTipos(Int32 idAss);
    }
}
