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
    public class FormularioRespostaAppService : AppServiceBase<FORMULARIO_RESPOSTA>, IFormularioRespostaAppService
    {
        private readonly IFormularioRespostaService _baseService;
        private readonly IConfiguracaoService _confService;

        public FormularioRespostaAppService(IFormularioRespostaService baseService, IConfiguracaoService confService) : base(baseService)
        {
            _baseService = baseService;
            _confService = confService;
        }

        public List<FORMULARIO_RESPOSTA> GetAllItens()
        {
            List<FORMULARIO_RESPOSTA> lista = _baseService.GetAllItens();
            return lista;
        }

        public List<UF> GetAllUF()
        {
            List<UF> lista = _baseService.GetAllUF();
            return lista;
        }

        public UF GetUFbySigla(String sigla)
        {
            return _baseService.GetUFbySigla(sigla);
        }
        public FORMULARIO_RESPOSTA GetItemById(Int32 id)
        {
            FORMULARIO_RESPOSTA item = _baseService.GetItemById(id);
            return item;
        }

        public Int32 ExecuteFilter(String nome, String email, String celular, String cidade, Int32? uf, out List<FORMULARIO_RESPOSTA> objeto)
        {
            try
            {
                objeto = new List<FORMULARIO_RESPOSTA>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(nome, email, celular, cidade, uf);
                if (objeto.Count == 0)
                {
                    volta = 1;
                }
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreate(FORMULARIO_RESPOSTA item)
        {
            try
            {
                // Completa objeto
                item.FORE_IN_ATIVO = 1;
                item.FORE_IN_STATUS = 1;
 
                // Persiste
                Int32 volta = _baseService.Create(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(FORMULARIO_RESPOSTA item, FORMULARIO_RESPOSTA itemAntes, USUARIO usuario)
        {
            try
            {
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(FORMULARIO_RESPOSTA item, FORMULARIO_RESPOSTA itemAntes)
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

        public Int32 ValidateDelete(FORMULARIO_RESPOSTA item, USUARIO usuario)
        {
            try
            {
                // Acerta campos
                item.FORE_IN_ATIVO = 0;

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(FORMULARIO_RESPOSTA item, USUARIO usuario)
        {
            try
            {
                // Acerta campos
                item.FORE_IN_ATIVO = 1;

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
