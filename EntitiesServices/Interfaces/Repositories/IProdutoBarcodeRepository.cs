using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IProdutoBarcodeRepository : IRepositoryBase<PRODUTO_BARCODE>
    {
        List<PRODUTO_BARCODE> GetAllItens();
        PRODUTO_BARCODE GetItemById(Int32 id);
        PRODUTO_BARCODE GetByProd(Int32 prod);
    }
}
