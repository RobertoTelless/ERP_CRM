using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IEco_PrestadorVeiculoCertificadoRepository : IRepositoryBase<PRESTADOR_VEICULO_CERTIFICADO>
    {
        List<PRESTADOR_VEICULO_CERTIFICADO> GetAllItens();
        PRESTADOR_VEICULO_CERTIFICADO GetItemById(Int32 id);
    }
}
