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
    public class TransportadoraAppService : AppServiceBase<TRANSPORTADORA>, ITransportadoraAppService
    {
        private readonly ITransportadoraService _baseService;

        public TransportadoraAppService(ITransportadoraService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public List<TRANSPORTADORA> GetAllItens(Int32 idAss)
        {
            List<TRANSPORTADORA> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<UF> GetAllUF()
        {
            return _baseService.GetAllUF();
        }

        public UF GetUFbySigla(String sigla)
        {
            return _baseService.GetUFbySigla(sigla);
        }

        public List<TRANSPORTADORA> GetAllItensAdm(Int32 idAss)
        {
            List<TRANSPORTADORA> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public TRANSPORTADORA GetItemById(Int32 id)
        {
            TRANSPORTADORA item = _baseService.GetItemById(id);
            return item;
        }

        public List<TIPO_VEICULO> GetAllTipoVeiculo(Int32 idAss)
        {
            return _baseService.GetAllTipoVeiculo(idAss);
        }

        public List<TIPO_TRANSPORTE> GetAllTipoTransporte(Int32 idAss)
        {
            return _baseService.GetAllTipoTransporte(idAss);
        }

        public TRANSPORTADORA GetByEmail(String email, Int32 idAss)
        {
            TRANSPORTADORA item = _baseService.GetByEmail(email, idAss);
            return item;
        }

        public TRANSPORTADORA CheckExist(TRANSPORTADORA conta, Int32 idAss)
        {
            TRANSPORTADORA item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public List<FILIAL> GetAllFilial(Int32 idAss)
        {
            List<FILIAL> lista = _baseService.GetAllFilial(idAss);
            return lista;
        }

        public TRANSPORTADORA_ANEXO GetAnexoById(Int32 id)
        {
            TRANSPORTADORA_ANEXO lista = _baseService.GetAnexoById(id);
            return lista;
        }

        public Int32 ExecuteFilter(Int32? veic, Int32? tran, String nome, String cnpj, String email, String cidade, String uf, Int32 idAss, out List<TRANSPORTADORA> objeto)
        {
            try
            {
                objeto = new List<TRANSPORTADORA>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(veic, tran, nome, cnpj, email, cidade, uf, idAss);
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

        public Int32 ValidateCreate(TRANSPORTADORA item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia
                if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.TRAN_IN_ATIVO = 1;
                item.ASSI_CD_ID = usuario.ASSI_CD_ID;

                // Checa endereço
                if (String.IsNullOrEmpty(item.TRAN_NM_ENDERECO))
                {
                    item.TRAN_NM_ENDERECO = "-";
                }
                if (String.IsNullOrEmpty(item.TRAN_NM_BAIRRO))
                {
                    item.TRAN_NM_BAIRRO = "-";
                }
                if (String.IsNullOrEmpty(item.TRAN_NM_CIDADE))
                {
                    item.TRAN_NM_CIDADE = "-";
                }
                if (String.IsNullOrEmpty(item.TRAN_SG_UF))
                {
                    item.TRAN_SG_UF = "-";
                }
                if (String.IsNullOrEmpty(item.TRAN_NR_CEP))
                {
                    item.TRAN_NR_CEP = "-";
                }

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "AddTRAN",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<TRANSPORTADORA>(item)
                };

                // Persiste
                Int32 volta = _baseService.Create(item, log);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(TRANSPORTADORA item, TRANSPORTADORA itemAntes, USUARIO usuario)
        {
            try
            {
                // Checa endereço
                if (String.IsNullOrEmpty(item.TRAN_NM_ENDERECO))
                {
                    item.TRAN_NM_ENDERECO = "-";
                }
                if (String.IsNullOrEmpty(item.TRAN_NM_BAIRRO))
                {
                    item.TRAN_NM_BAIRRO = "-";
                }
                if (String.IsNullOrEmpty(item.TRAN_NM_CIDADE))
                {
                    item.TRAN_NM_CIDADE = "-";
                }
                if (String.IsNullOrEmpty(item.TRAN_SG_UF))
                {
                    item.TRAN_SG_UF = "-";
                }
                if (String.IsNullOrEmpty(item.TRAN_NR_CEP))
                {
                    item.TRAN_NR_CEP = "-";
                }

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "EditTRAN",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<TRANSPORTADORA>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<TRANSPORTADORA>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(TRANSPORTADORA item, TRANSPORTADORA itemAntes)
        {
            try
            {
                // Checa endereço
                if (String.IsNullOrEmpty(item.TRAN_NM_ENDERECO))
                {
                    item.TRAN_NM_ENDERECO = "-";
                }
                if (String.IsNullOrEmpty(item.TRAN_NM_BAIRRO))
                {
                    item.TRAN_NM_BAIRRO = "-";
                }
                if (String.IsNullOrEmpty(item.TRAN_NM_CIDADE))
                {
                    item.TRAN_NM_CIDADE = "-";
                }
                if (String.IsNullOrEmpty(item.TRAN_SG_UF))
                {
                    item.TRAN_SG_UF = "-";
                }
                if (String.IsNullOrEmpty(item.TRAN_NR_CEP))
                {
                    item.TRAN_NR_CEP = "-";
                }


                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(TRANSPORTADORA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.TRAN_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelTRAN",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<TRANSPORTADORA>(item)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(TRANSPORTADORA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.TRAN_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatTRAN",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<TRANSPORTADORA>(item)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
