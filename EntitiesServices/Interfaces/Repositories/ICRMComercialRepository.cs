using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ICRMComercialRepository : IRepositoryBase<CRM_COMERCIAL>
    {
        CRM_COMERCIAL CheckExist(CRM_COMERCIAL item, Int32 idUsu, Int32 idAss);
        List<CRM_COMERCIAL> GetByDate(DateTime data, Int32 idAss);
        List<CRM_COMERCIAL> GetByUser(Int32 user);
        List<CRM_COMERCIAL> GetTarefaStatus(Int32 tipo, Int32 idAss);
        CRM_COMERCIAL GetItemById(Int32 id);
        List<CRM_COMERCIAL> GetAllItens(Int32 idAss);
        List<CRM_COMERCIAL> GetAllItensAdm(Int32 idAss);
        List<CRM_COMERCIAL> ExecuteFilter(Int32? status, DateTime? inicio, DateTime? prevista, String numero, String nota, Int32? estrela, String nome, String busca, Int32 idAss);
        List<CRM_COMERCIAL> GetAtrasados(Int32 idAss);
        List<CRM_COMERCIAL> GetEncerrados(Int32 idAss);
        List<CRM_COMERCIAL> GetCancelados(Int32 idAss);
        List<CRM_COMERCIAL> GetAllItensAdmUser(Int32 id, Int32 idAss);
        List<CRM_COMERCIAL> ExecuteFilterDash(String nmr, DateTime? dtFinal, String nome, Int32? usu, Int32? status, Int32 idAss);
    }
}
