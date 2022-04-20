using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IClasseService : IServiceBase<CLASSE>
    {
        Int32 Create(CLASSE perfil, LOG log);
        Int32 Create(CLASSE perfil);
        Int32 Edit(CLASSE perfil, LOG log);
        Int32 Edit(CLASSE perfil);
        Int32 Delete(CLASSE perfil, LOG log);

        CLASSE CheckExist(CLASSE conta, Int32 idAss);
        CLASSE GetItemById(Int32 id);
        CLASSE GetBySigla(String sigla);
        List<CLASSE> GetAllItens(Int32 idAss);
        List<CLASSE> GetAllItensAdm(Int32 idAss);

        List<GRUPO_DOCUMENTO> GetAllGrupos(Int32 idAss);
        List<NIVEL_SEGURANCA> GetAllNivel(Int32 idAss);
        List<CLASSE> ExecuteFilter(Int32? grupo, Int32? seg, String nome, String descricao, String sigla, Int32 idAss);

    }
}
