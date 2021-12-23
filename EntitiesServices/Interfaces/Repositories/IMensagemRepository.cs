using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IMensagemRepository : IRepositoryBase<MENSAGENS>
    {
        MENSAGENS CheckExist(MENSAGENS item, Int32 idAss);
        MENSAGENS GetItemById(Int32 id);
        List<MENSAGENS> GetAllItens(Int32 idAss);
        List<MENSAGENS> GetAllItensAdm(Int32 idAss);
        List<MENSAGENS> ExecuteFilter(DateTime? criacao, DateTime? envio, String campanha, String texto, Int32? tipo, Int32 idAss);
    }
}
