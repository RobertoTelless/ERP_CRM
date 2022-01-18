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
    public class ServicoTabelaPrecoAppService : AppServiceBase<SERVICO_TABELA_PRECO>, IServicoTabelaPrecoAppService
    {
        private readonly IServicoTabelaPrecoService _baseService;

        public ServicoTabelaPrecoAppService(IServicoTabelaPrecoService baseService) : base(baseService)
        {
            _baseService = baseService;
        }

        public SERVICO_TABELA_PRECO GetByServFilial(Int32 id, Int32 fili)
        {
            return _baseService.GetByServFilial(id, fili);
        }

        public Int32 ValidateCreateLista(List<SERVICO_TABELA_PRECO> lista)
        {
            foreach (SERVICO_TABELA_PRECO item in lista)
            {
                try
                {
                    // Completa objeto
                    item.SETP_IN_ATIVO = 1;

                    // Persiste
                    Int32 volta = _baseService.Create(item);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

            return 0;
        }

        public Int32 ValidateCreate(SERVICO_TABELA_PRECO item)
        {
            try
            {
                // Completa objeto
                item.SETP_IN_ATIVO = 1;

                // Persiste
                Int32 volta = _baseService.Create(item);
                return 0;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(SERVICO_TABELA_PRECO item, Int32 id)
        {
            try
            {

                // Persiste
                return _baseService.Edit(item, id);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(SERVICO_TABELA_PRECO item, Int32 id)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.SETP_IN_ATIVO = 0;

                // Persiste
                return _baseService.Edit(item, id);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(SERVICO_TABELA_PRECO item, Int32 id)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.SETP_IN_ATIVO = 1;

                // Persiste
                return _baseService.Edit(item, id);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
