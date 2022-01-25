using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ITransportadoraAppService : IAppServiceBase<TRANSPORTADORA>
    {
        Int32 ValidateCreate(TRANSPORTADORA perfil, USUARIO usuario);
        Int32 ValidateEdit(TRANSPORTADORA perfil, TRANSPORTADORA perfilAntes, USUARIO usuario);
        Int32 ValidateEdit(TRANSPORTADORA item, TRANSPORTADORA itemAntes);
        Int32 ValidateDelete(TRANSPORTADORA perfil, USUARIO usuario);
        Int32 ValidateReativar(TRANSPORTADORA perfil, USUARIO usuario);

        List<UF> GetAllUF();
        UF GetUFbySigla(String sigla);
        List<TRANSPORTADORA> GetAllItens(Int32 idAss);
        List<TRANSPORTADORA> GetAllItensAdm(Int32 idAss);
        TRANSPORTADORA GetItemById(Int32 id);
        TRANSPORTADORA GetByEmail(String email, Int32 idAss);
        TRANSPORTADORA CheckExist(TRANSPORTADORA conta, Int32 idAss);
        List<FILIAL> GetAllFilial(Int32 idAss);
        TRANSPORTADORA_ANEXO GetAnexoById(Int32 id);
        Int32 ExecuteFilter(Int32? veic, Int32? tran, String nome, String cnpj, String email, String cidade, String uf, Int32 idAss, out List<TRANSPORTADORA> objeto);
        List<TIPO_VEICULO> GetAllTipoVeiculo(Int32 idAss);
        List<TIPO_TRANSPORTE> GetAllTipoTransporte(Int32 idAss);
    }
}
