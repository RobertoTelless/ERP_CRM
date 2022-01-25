using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface ITransportadoraService : IServiceBase<TRANSPORTADORA>
    {
        Int32 Create(TRANSPORTADORA perfil, LOG log);
        Int32 Create(TRANSPORTADORA perfil);
        Int32 Edit(TRANSPORTADORA perfil, LOG log);
        Int32 Edit(TRANSPORTADORA perfil);
        Int32 Delete(TRANSPORTADORA perfil, LOG log);

        TRANSPORTADORA CheckExist(TRANSPORTADORA conta, Int32 idAss);
        List<UF> GetAllUF();
        UF GetUFbySigla(String sigla);
        TRANSPORTADORA GetItemById(Int32 id);
        TRANSPORTADORA GetByEmail(String email, Int32 idAss);
        List<TRANSPORTADORA> GetAllItens(Int32 idAss);
        List<TRANSPORTADORA> GetAllItensAdm(Int32 idAss);
        List<FILIAL> GetAllFilial(Int32 idAss);
        TRANSPORTADORA_ANEXO GetAnexoById(Int32 id);
        List<TRANSPORTADORA> ExecuteFilter(Int32? veic, Int32? tran, String nome, String cnpj, String email, String cidade, String uf, Int32 idAss);
        List<TIPO_VEICULO> GetAllTipoVeiculo(Int32 idAss);
        List<TIPO_TRANSPORTE> GetAllTipoTransporte(Int32 idAss);
    }
}
