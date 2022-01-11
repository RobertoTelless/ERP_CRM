using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;
using ApplicationServices.Interfaces;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;
using System.Text.RegularExpressions;

namespace ApplicationServices.Services
{
    public class ProdutoTabelaPrecoAppService : AppServiceBase<PRODUTO_TABELA_PRECO>, IProdutotabelaPrecoAppService
    {
        private readonly IProdutoTabelaPrecoService _baseService;

        public ProdutoTabelaPrecoAppService(IProdutoTabelaPrecoService baseService) : base(baseService)
        {
            _baseService = baseService;
        }

        public PRODUTO_TABELA_PRECO GetItemById(Int32 id)
        {
            PRODUTO_TABELA_PRECO item = _baseService.GetItemById(id);
            return item;
        }

        public PRODUTO_TABELA_PRECO CheckExist(PRODUTO_TABELA_PRECO item, Int32 idAss)
        {
            PRODUTO_TABELA_PRECO obj = _baseService.CheckExist(item, idAss);
            return obj;
        }

        public PRODUTO_TABELA_PRECO GetByProdFilial(Int32 prod, Int32 fili)
        {
            return _baseService.GetByProdFilial(prod, fili);
        }

        public Int32 ValidateCreate(PRODUTO_TABELA_PRECO item)
        {
            try
            {
                // Verifica existencia prévia

                // Completa objeto

                // Persiste
                Int32 volta = _baseService.Create(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(PRODUTO_TABELA_PRECO item, PRODUTO_TABELA_PRECO itemAntes)
        {
            try
            {
                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(PRODUTO_TABELA_PRECO item)
        {
            try
            {
                // Persiste
                return _baseService.Delete(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
