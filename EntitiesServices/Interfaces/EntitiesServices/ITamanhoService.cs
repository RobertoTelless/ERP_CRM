using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface ITamanhoService : IServiceBase<TAMANHO>
    {
        Int32 Create(TAMANHO perfil, LOG log);
        Int32 Create(TAMANHO perfil);
        Int32 Edit(TAMANHO perfil, LOG log);
        Int32 Edit(TAMANHO perfil);
        Int32 Delete(TAMANHO perfil, LOG log);

        TAMANHO CheckExist(TAMANHO conta, Int32 idAss);
        TAMANHO GetItemById(Int32 id);
        List<TAMANHO> GetAllItens(Int32 idAss);
        List<TAMANHO> GetAllItensAdm(Int32 idAss);
    }
}
